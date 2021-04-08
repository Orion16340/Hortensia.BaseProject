using Hortensia.Auth.Network;
using Hortensia.Core;
using Hortensia.Core.Extensions;
using Hortensia.Framing;
using Hortensia.ORM;
using Hortensia.ORM.Configuration;
using Hortensia.Protocol.Custom;
using Hortensia.Synchronizer.Commands;
using Hortensia.Synchronizer.Records.Accounts;
using Hortensia.Synchronizer.Records.World;
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
        private readonly ConsoleCommandsManager _consoleCommandsManager;

        public LifeTime(ILogger logger, IConfiguration configuration, AuthServer authServer, DatabaseManager databaseManager, SaveManager saveManager, IFrameManager frameManager, ConsoleCommandsManager consoleCommandsManager)
        {
            _logger = logger;
            _configuration = configuration;
            _authServer = authServer;
            _database = databaseManager;
            _saveManager = saveManager;
            _frameManager = frameManager;
            _consoleCommandsManager = consoleCommandsManager;

            AppDomain.CurrentDomain.ProcessExit += Stop;
        }

        public void Start()
        {
            var databaseConfig = _configuration.GetSection("DatabaseConfiguration").Get<DatabaseConfiguration>();

            _frameManager
                .InitializeTypes(typeof(RoleEnum).Assembly)
                .InitializeMessages(typeof(RoleEnum).Assembly, typeof(AuthServer).Assembly);

            _database
                .InitializeDatabase(databaseConfig, typeof(DatabaseManager).Assembly)
                // TODO : Inused for moment but is fonctionally
                //.InitializeBackup("Backups")
                .InitializeAutoSave(databaseConfig.SaveConfiguration)
                .RegisterTable<AccountRecord>()
                .RegisterTable<BannedIPRecord>()
                .RegisterTable<WorldCharactersRecord>()
                .RegisterTable<WorldRecord>()
                .Set()
                .LoadTables();

            _consoleCommandsManager.Initialize(typeof(AuthServer).Assembly);

            _authServer.Run();

            _consoleCommandsManager.ReadCommand();
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
