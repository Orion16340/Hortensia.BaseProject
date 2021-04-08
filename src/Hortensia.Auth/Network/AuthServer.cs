
using Hortensia.Core;
using Hortensia.Synchronizer.Network;
using Hortensia.Synchronizer.Parameters;
using Microsoft.Extensions.Configuration;

namespace Hortensia.Auth.Network
{
    public class AuthServer : NetworkServer
    {
        public AuthServer(ILogger logger) : base(logger)
        {
            base.ServerStarted += () =>
            {
                _logger.LogInformation($"AuthServer started at {this}");
            };

            base.ServerFailedToStart += (ex) =>
            {
                _logger.LogError($"AuthServer failed to start : {ex}");
            };

            base.SocketConnected += (socket) =>
            {
                var client = new AuthClient(socket);
                Clients.Add(client);

                _logger.LogInformation($"{client} connected to AuthServer");
            };
        }
    }
}
