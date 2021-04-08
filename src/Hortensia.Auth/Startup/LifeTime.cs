using Hortensia.Auth.Network;
using Hortensia.Core;
using Hortensia.Core.Extensions;
using Hortensia.Framing;
using Hortensia.ORM;
using Hortensia.ORM.Configuration;
using Hortensia.Protocol.Custom;
using Hortensia.Synchronizer.Records;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;

namespace Hortensia.Auth
{
    public class LifeTime
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly AuthServer _authServer;
        private readonly DatabaseManager _database;
        private readonly SaveManager _saveManager;
        private readonly IFrameManager _frameManager;

        public LifeTime(ILogger logger, IConfiguration configuration, AuthServer authServer, DatabaseManager databaseManager, SaveManager saveManager, IFrameManager frameManager)
        {
            _logger = logger;
            _configuration = configuration;
            _authServer = authServer;
            _database = databaseManager;
            _saveManager = saveManager;
            _frameManager = frameManager;

            AppDomain.CurrentDomain.ProcessExit += Stop;
        }

        public void Start()
        {
            var databaseConfig = _configuration.GetSection("DatabaseConfiguration").Get<DatabaseConfiguration>();

            _frameManager
                .InitializeTypes(typeof(RoleEnum).Assembly)
                .InitializeMessages(typeof(RoleEnum).Assembly, typeof(AuthServer).Assembly);

            _database
                .InitializeDatabase(databaseConfig, typeof(DatabaseManager).Assembly, false)
                .InitializeBackup("Backups")
                .InitializeAutoSave(databaseConfig.SaveConfiguration)
                .RegisterTable<Account>()
                .LoadTables();


            _authServer.Run();
        }

        public void Stop(object sender, EventArgs args)
        {
            _logger.LogInformation("Hortensia stoping.. saving changes..");
            _saveManager.Save();
            _authServer.Clients.FindAndAction(x => x.Dispose());
            _authServer.Dispose();
            Thread.Sleep(5000);
        }
    }
}
