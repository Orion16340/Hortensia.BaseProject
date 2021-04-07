using Hortensia.Auth.Network;
using Hortensia.Core;
using Hortensia.ORM;
using Hortensia.ORM.Configuration;
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

        public LifeTime(ILogger logger, IConfiguration configuration, AuthServer authServer, DatabaseManager databaseManager, SaveManager saveManager)
        {
            _logger = logger;
            _configuration = configuration;
            _authServer = authServer;
            _database = databaseManager;
            _saveManager = saveManager;

            AppDomain.CurrentDomain.ProcessExit += Stop;
        }

        public void Start()
        {
            var databaseConfig = _configuration.GetSection("DatabaseConfiguration").Get<DatabaseConfiguration>();

            _database
                .InitializeDatabase(databaseConfig, typeof(DatabaseManager).Assembly, false)
                .InitializeBackup("Backups")
                .InitializeAutoSave(databaseConfig.SaveConfiguration)
                .RegisterTable<Account>()
                .LoadTables();

            _authServer.Start();
        }

        public void Stop(object sender, EventArgs args)
        {
            _logger.LogInformation("Hortensia stoping.. saving changes..");
            _saveManager.Save();
            Thread.Sleep(1000);
        }
    }
}
