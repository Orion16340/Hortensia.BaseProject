
using Hortensia.Core;
using Microsoft.Extensions.Configuration;

namespace Hortensia.Auth.ExempleDI
{
    public class Exemple
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public Exemple(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void Start()
        {
            _logger.LogInformation(_configuration["Hortensia"]);
        }
    }
}
