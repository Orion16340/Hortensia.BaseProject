using Hortensia.Core;
using Hortensia.Framing.Network;
using Hortensia.Synchronizer.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;

namespace Hortensia.Synchronizer.Network
{
    public abstract class NetworkServer : IDisposable
    {
        private readonly INetworkOptions _options;
        public readonly ILogger _logger;
        public IPEndPoint EndPoint { get; private set; }
        public Socket Socket { get; private set; }

        public SynchronizedCollection<INetworkClient> Clients;

        protected NetworkServer(ILogger logger)
        {
            _options = ServiceLocator.Provider.GetService<INetworkOptions>();
            _logger = logger;
            Clients = new();

            EndPoint = new IPEndPoint(IPAddress.Parse(_options.IP), _options.Port);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Run()
        {
            try
            {
                Socket.Bind(EndPoint);
                Socket.Listen(_options.Backlog);
            }
            catch (Exception ex)
            {
                OnServerFailedToStart(ex);
                return;
            }

            AcceptCallback(null);
            OnServerStarted();
        }

        private void AcceptCallback(SocketAsyncEventArgs args)
        {
            if (args != null)
                args.AcceptSocket = null;

            else
            {
                args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventCompleted);
            }

            if (!Socket.AcceptAsync(args))
                ProcessAccept(args);
        }

        public void Dispose()
        {
            if (Socket != null)
            {
                OnServerStoping();
            }
        }

        private void AcceptEventCompleted(object sender, SocketAsyncEventArgs args)
        {
            ProcessAccept(args);
        }

        private void ProcessAccept(SocketAsyncEventArgs args)
        {
            var socket = args.AcceptSocket;

            if (Clients.Count > _options.MaxConcurrentConnections || CanConnectClient(socket))
                socket.Dispose();
            else
                OnSocketConnected(args.AcceptSocket);

            AcceptCallback(args);
        }

        public event Action ServerStarted;
        public event Action<Exception> ServerFailedToStart;
        public event Action ServerStoping;
        public event Action<Socket> SocketConnected;

        private void OnServerStarted() => ServerStarted?.Invoke();
        private void OnServerFailedToStart(Exception ex) => ServerFailedToStart?.Invoke(ex);
        private void OnServerStoping() => ServerStoping?.Invoke();
        private void OnSocketConnected(Socket client) => SocketConnected?.Invoke(client);

        public bool CanConnectClient(Socket socket)
        {
            return CountClientsWithSameIP(socket) < _options.MaxConnectionsPairIP;
        }

        public int CountClientsWithSameIP(Socket socket)
        {
            return Clients.Where(x => x.IP == socket.RemoteEndPoint.AddressFamily.ToString()).Count();
        }

        public override string ToString() => $"<{EndPoint.Address}:{EndPoint.Port}>";
    }
}
