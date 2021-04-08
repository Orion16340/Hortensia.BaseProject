using Hortensia.Framing;
using Hortensia.Core;
using Hortensia.Core.Extensions;
using Hortensia.Framing.IO;
using Hortensia.Framing.Network;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hortensia.Framing
{
    public interface IFrameManager
    {
        IFrameManager InitializeTypes(Assembly typesAssembly);
        IFrameManager InitializeMessages(Assembly messagesAssembly, Assembly handlersAssembly);
        NetworkMessage BuildMessage(ICustomDataReader reader);
        bool HandleMessage(NetworkMessage message, INetworkClient client);
    }

    public class FrameManager : IFrameManager
    {
        private readonly ILogger _logger;

        public const string TargetField = "Id";

        private readonly Type[] _handlerParameterTypes = new Type[] { typeof(NetworkMessage), typeof(INetworkClient) };

        private readonly Dictionary<uint, Delegate> _handlers = new();

        private readonly Dictionary<uint, Type> _messages = new();

        private readonly Dictionary<uint, Func<NetworkMessage>> _ctors = new();

        private static readonly Dictionary<uint, Func<object>> _types = new();

        public bool _messagesInitialized = false;
        private static bool _typesInitialized = false;

        public FrameManager()
        {
            _logger = ServiceLocator.Provider.GetService<ILogger>();
        }

        public IFrameManager InitializeTypes(Assembly typesAssembly)
        {
            var types = typesAssembly.GetTypes().Where(x => x.Namespace.Contains("Hortensia.Protocol.Types"));

            foreach (Type type in types)
            {
                FieldInfo field = type.GetField(TargetField);
                if (field != null)
                {
                    short id = (short)field.GetValue(type);
                    Expression body = Expression.New(type);
                    var cmp = Expression.Lambda<Func<object>>(body).Compile();
                    _types.Add((ushort)id, cmp);
                }
            }

            _typesInitialized = true;

            return this;
        }

        public static T GetInstance<T>(short id) where T : class
        {
            if (!_typesInitialized)
                throw new UnauthorizedAccessException("This function cannot be used until FrameManager.IntitializeTypes() has been called.");

            if (!_types.ContainsKey((uint)id))
                throw new Exception($"Type <id:{id}> doesn't exist");

            else
            {
                var instance = _types[(uint)id]() as T;
                return instance;
            }
        }

        public IFrameManager InitializeMessages(Assembly messagesAssembly, Assembly handlersAssembly)
        {
            foreach (var type in messagesAssembly.GetTypes().Where(x => x.Namespace.Contains("Hortensia.Protocol.Messages")))
            {
                FieldInfo field = type.GetField(TargetField);

                if (field != null)
                {
                    uint num = (uint)field.GetValue(type);

                    if (_messages.ContainsKey(num))
                        throw new AmbiguousMatchException(string.Format("MessageReceiver() => {0} item is already in the dictionary, old type is : {1}, new type is  {2}", num, _messages[num], type));

                    _messages.Add(num, type);

                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);

                    if (constructor == null)
                        throw new Exception(string.Format("'{0}' doesn't implemented a parameterless constructor", type));

                    _ctors.Add(num, value: constructor.CreateDelegate<Func<NetworkMessage>>());
                }
            }

            foreach (var item in handlersAssembly.GetTypes())
            {
                foreach (var subItem in item.GetMethods())
                {
                    var attribute = subItem.GetCustomAttribute(typeof(FrameMessageAttribute));
                    if (attribute != null)
                    {
                        ParameterInfo[] parameters = subItem.GetParameters();
                        Type methodParameters = subItem.GetParameters()[0].ParameterType;
                        if (methodParameters.BaseType != null)
                        {
                            try
                            {
                                Delegate target = subItem.CreateDelegate(_handlerParameterTypes);
                                FieldInfo field = methodParameters.GetField(TargetField);
                                _handlers.Add((uint)field.GetValue(Activator.CreateInstance(methodParameters)), target);
                            }
                            catch
                            {
                                throw new Exception("Cannot register " + subItem.Name + " has message handler...");
                            }
                        }
                    }
                }
            }

            _logger.LogInformation(_types.Count + " Type(s) & " + _messages.Count + " Message(s) loaded..");
            _logger.LogInformation(_handlers.Count + " Handler(s) loaded..");

            _messagesInitialized = true;

            return this;
        }

        public NetworkMessage BuildMessage(ICustomDataReader reader)
        {
            NetworkMessage networkMessage;
            MessagePart messagePart = new();

            if (!messagePart.Build(reader))
                networkMessage = null;

            else
            {
                try
                {
                    ICustomDataReader customDataReader = null;
                    ushort value = (ushort)messagePart.MessageId.Value;

                    if (_messages.ContainsKey(value))
                    {
                        NetworkMessage item = _ctors[value]();
                        if (item != null)
                        {
                            int? lengthBytesCount = messagePart.Data.Length;
                            reader.Seek((int)(2 + NetworkMessage.ComputeTypeLen((uint)messagePart.Data.Length)), SeekOrigin.Begin);
                            customDataReader = reader;

                            item.UnPack(customDataReader);
                            networkMessage = item;
                        }
                        else
                            networkMessage = null;
                    }
                    else
                        networkMessage = null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    networkMessage = null;
                }
            }
            return networkMessage;
        }

        public bool HandleMessage(NetworkMessage message, INetworkClient client)
        {
            if (!_messagesInitialized)
                throw new UnauthorizedAccessException("This function cannot be used until FrameManager.IntitializeMessage() has been called.");

            if (message == null)
            {
                client.Dispose();
                return false;
            }

            _handlers.TryGetValue(message.MessageId, out Delegate handler);

            if (handler != null)
            {
                try
                {
                    handler.DynamicInvoke(null, message, client);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unable to handle message {0} {1} : '{2}'", message.ToString(), handler.Method.Name, ex.InnerException.ToString());
                    return false;
                }
            }
            else
            {
                _logger.LogError("No Handler: ({0}) {1}", message.MessageId, message.ToString());
                return true;
            }
        }
    }
}
