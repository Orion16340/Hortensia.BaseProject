using Hortensia.Core;
using Hortensia.Framing;
using Hortensia.Synchronizer.Network;
using Hortensia.Synchronizer.Parameters;
using System.Net.Sockets;

namespace Hortensia.Auth.Network
{
    public class AuthClient : NetworkClient
    {
        public AuthClient(IFrameManager frameManager, ILogger logger) : base(frameManager, logger)
        {
        }

        public AuthClient(Socket socket) : base(socket)
        {
            base.MessageReceived += (message) =>
            {
                _logger.LogProtocol($"{this} receiv {message}");
            };

            base.Disconnect += () =>
            {

            };
        }
    }
}
