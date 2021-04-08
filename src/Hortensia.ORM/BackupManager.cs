using Hortensia.Core.Extensions;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Hortensia.Core;

namespace Hortensia.ORM
{
    public class BackupManager
    {
        private const string BackupFileExtension = ".sql";

        private string BackupDirectory { get; set; }
        private bool IsInitialized { get; set; } = false;

        public void Initialize(string directory)
        {
            BackupDirectory = directory;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            IsInitialized = true;
        }

        public void Backup()
        {
            if (!IsInitialized)
                return;

            DatabaseManager.SyncRoot.EnterReadLock();

            try
            {
                var date = DateTime.Now.ToFileNameDate();
                ServiceLocator.Provider.GetService<DatabaseManager>().StartBackup(BackupDirectory + "/" + date + BackupFileExtension);
                ServiceLocator.Provider.GetService<ILogger>().LogDatabase($"Dump on <{date}>");
            }
            catch (Exception ex)
            {
                ServiceLocator.Provider.GetService<ILogger>().LogError(ex.Message);
            }
            finally { DatabaseManager.SyncRoot.ExitReadLock(); }
        }
    }
}
