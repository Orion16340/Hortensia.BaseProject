using Hortensia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hortensia.Synchronizer.Commands
{
    public interface IConsoleCommandsManager
    {
        ConsoleCommandsManager Initialize(Assembly assembly);
        void ReadCommand();
    }

    public class ConsoleCommandsManager : IConsoleCommandsManager
    {
        private readonly Dictionary<string, MethodInfo> m_commands = new Dictionary<string, MethodInfo>();

        private ILogger _logger;

        public ConsoleCommandsManager()
        {
            _logger = ServiceLocator.Provider.GetService<ILogger>();
        }

        private void Handle(string input)
        {
            string[] strArrays = input.Split(null);
            string lower = strArrays.First<string>().ToLower();
            if (lower != "help")
            {
                KeyValuePair<string, MethodInfo> keyValuePair = this.m_commands.FirstOrDefault((KeyValuePair<string, MethodInfo> x) => x.Key == lower);
                if (keyValuePair.Value == null)
                {
                    _logger.LogWarning("{0} is not a valid command. ('help' to get a list of commands)", lower);
                }
                else
                {
                    ParameterInfo[] parameters = keyValuePair.Value.GetParameters();
                    string[] array = strArrays.Skip<string>(1).ToArray<string>();
                    object[] objArray = new object[(int)array.Length];
                    if ((int)parameters.Length == (int)objArray.Length)
                    {
                        try
                        {
                            for (int i = 0; i < objArray.Length; i++)
                            {
                                objArray[i] = Convert.ChangeType(array[i], parameters[i].ParameterType);
                            }
                        }
                        catch
                        {
                            string[] key = new string[] { "Invalid parameters for command ", keyValuePair.Key, " (", null, null };
                            key[3] = string.Join(",",
                                from x in parameters
                                select string.Concat(x.ParameterType.Name, " ", x.Name));
                            key[4] = ")";
                            _logger.LogWarning(string.Concat(key));
                            return;
                        }
                        try
                        {
                            keyValuePair.Value.Invoke(null, objArray);
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(exception.Message);
                        }
                    }
                    else
                    {
                        object[] key1 = new object[] { "Command ", keyValuePair.Key, " required ", (int)parameters.Length, " parameters. (", null, null };
                        key1[5] = string.Join(",",
                            from x in parameters
                            select string.Concat(x.ParameterType.Name, " ", x.Name));
                        key1[6] = ")";
                        _logger.LogWarning(string.Concat(key1));
                    }
                }
            }
            else
            {
                this.Help();
            }
        }

        private void Help()
        {
            _logger.LogInformation("Commands :");
            foreach (KeyValuePair<string, MethodInfo> mCommand in this.m_commands)
            {
                _logger.LogInformation(string.Concat("-", mCommand.Key));
            }
        }

        public ConsoleCommandsManager Initialize(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < (int)types.Length; i++)
            {
                MethodInfo[] methods = types[i].GetMethods();
                for (int j = 0; j < (int)methods.Length; j++)
                {
                    MethodInfo methodInfo = methods[j];
                    ConsoleCommandAttribute customAttribute = methodInfo.GetCustomAttribute<ConsoleCommandAttribute>();
                    if (customAttribute != null)
                    {
                        this.m_commands.Add(customAttribute.Name, methodInfo);
                    }
                }
            }
            _logger.LogInformation(m_commands.Count + " command(s) loaded..");

            return this;
        }

        public void ReadCommand()
        {
            while (true)
            {
                string str = Console.ReadLine();
                if (str != string.Empty)
                {
                    this.Handle(str);
                }
            }
        }
    }
}
