using Hortensia.Core;
using Hortensia.Framing;
using Hortensia.Framing.IO;
using Hortensia.Framing.Network;
using Hortensia.Synchronizer.Parameters;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Hortensia.Synchronizer.Records.Accounts;

namespace Hortensia.Synchronizer.Network
{
    public abstract class NetworkClient : INetworkClient
    {
        public readonly ILogger _logger;
        private readonly IFrameManager _frameManager;
        private readonly INetworkOptions _options;

        public Socket Socket { get; private set; }
        public IPEndPoint EndPoint => Socket.RemoteEndPoint as IPEndPoint;
        public string IP => EndPoint.Address.ToString();
        public bool Connected => Socket != null && Socket.Connected;

        public AccountRecord Account { get; set; }

        public NetworkClient(Socket socket)
        {
            Socket = socket;
            BeginReceiv();
        }

        protected NetworkClient(IFrameManager frameManager, ILogger logger)
        {
            _logger = logger;
            _frameManager = frameManager;
            _options = _options = ServiceLocator.Provider.GetService<NetworkOptions>();
        }

        private void BeginReceiv()
        {
            var socket = Socket;

            if (socket != null)
                socket.BeginReceive(_options.Buffer, 0, _options.Buffer.Length, 0, new AsyncCallback(OnReceived), null);
        }

        public void OnReceived(IAsyncResult result)
        {
            if (Socket != null)
            {
                int num = 0;
                try
                {
                    num = Socket.EndReceive(result);
                    if (num == 0)
                    {
                        Dispose();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError( ex.Message);
                    Dispose();
                    return;
                }

                OnReceivData(num);
                BeginReceiv();
            }
        }

        private void OnReceivData(int size)
        {
            if (_options.Buffer.Max() != 0)
            {
                _options.BufferEndPosition += size;

                if (_options.BufferEndPosition > _options.BufferLength)
                    throw new Exception("Too large amount of data.");
            }
            else
                _options.BufferEndPosition = size;

            while (_options.BufferEndPosition > 0)
            {
                CustomDataReader customDataReader = new CustomDataReader(_options.Buffer);

                NetworkMessage networkMessage = _frameManager.BuildMessage(customDataReader);

                if (networkMessage == null)
                    _logger.LogWarning($"{this} Received Unknown Data");

                else
                    OnMessageReceived(networkMessage);

                byte[] numArray = new byte[_options.BufferLength];
                Array.Copy(_options.Buffer, customDataReader.Position, numArray, (long)0, (long)(int)numArray.Length - customDataReader.Position);
                _options.Buffer = numArray;
                _options.BufferEndPosition -= (int)customDataReader.Position;
                customDataReader.Dispose();
            }
        }

        public void Dispose()
        {
            if (Socket != null)
            {
                OnDisconnect();

                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
                Socket.Dispose();
                Socket = null;
            }
        }

        public event Action<NetworkMessage> MessageReceived;
        public event Action Disconnect;

        private void OnMessageReceived(NetworkMessage message) => MessageReceived?.Invoke(message);
        private void OnDisconnect() => Disconnect?.Invoke();

        public override string ToString() => $"Client <{EndPoint.Address}:{EndPoint.Port}>";
    }
}
