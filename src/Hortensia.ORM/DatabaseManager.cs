using Hortensia.Core.Extensions;
using Hortensia.Core.Threads;
using Hortensia.Core.Threads.Timers;
using Hortensia.ORM.Attributes;
using Hortensia.ORM.Configuration;
using Hortensia.ORM.Enums;
using Hortensia.ORM.Interfaces;
using Hortensia.ORM.Tables;
using Hortensia.ORM.Tables.IO;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Hortensia.Core;
using MySQLBackupNetCore;

namespace Hortensia.ORM
{
    public class DatabaseManager
    {
        public static ReaderWriterLockSlim SyncRoot = new();
        private MySqlConnection Connection { get; set; }
        public List<Type> Tables { get; private set; }
        public IContextHandler IOTaskPool { get; private set; }

        public DatabaseManager(IContextHandler pool)
            => IOTaskPool = pool;

        public DatabaseManager InitializeDatabase(DatabaseConfiguration configuration, Assembly repositoryAssembly)
        {
            if (Connection != null)
                throw new Exception("There is already an instance of DatabaseManager.");

            Connection = new MySqlConnection(configuration.ToString());
            Tables = Array.FindAll(repositoryAssembly.GetTypes(), x => x.GetInterface("IRecord") != null).ToList();

            return this;
        }

        public DatabaseManager RegisterTable<T>() where T : IRecord
        {
            Tables.Add(typeof(T));
            return this;
        }

        public DatabaseManager Set()
        {
            ServiceLocator.Provider.GetService<TableManager>().Initialize(Tables.ToArray());
            return this;
        }

        public MySqlConnection UseConnection()
        {
            if (!Connection.Ping())
            {
                Connection.Close();
                Connection.Open();
            }

            return Connection;
        }

        public DatabaseManager CloseConnection()
        {
            Connection.Close();

            return this;
        }

        public DatabaseManager InitializeBackup(string directory)
        {
            ServiceLocator.Provider.GetService<BackupManager>().Initialize(directory);
            return this;
        }

        public DatabaseManager InitializeAutoSave(TimerConfigurationEntry configuration)
        {
            IOTaskPool.ExecutePeriodically(ServiceLocator.Provider.GetService<SaveManager>().Save, configuration);
            IOTaskPool.Start();

            return this;
        }

        public DatabaseManager StartBackup(string filePath)
        {
            try
            {
                var connection = UseConnection().Clone() as MySqlConnection;
                using var command = new MySqlCommand();
                using var backup = new MySqlBackup(command);

                command.Connection = connection;
                connection.Open();
                backup.ExportToFile(filePath);
                connection.Close();
            }
            catch (MySqlException ex)
            {
                ServiceLocator.Provider.GetService<ILogger>().LogError(ex.Message);
            }

            return this;
        }

        public DatabaseManager ExecuteNonQuery(DatabaseOperation operation, params object[] args)
        {
            SyncRoot.EnterWriteLock();

            try
            {
                string query = string.Format(TableConverter.GetOperation(operation), args);
                using var command = new MySqlCommand(query, Connection);
                command.ExecuteNonQuery();
            }
            catch(MySqlException ex)
            {
                ServiceLocator.Provider.GetService<ILogger>().LogError(ex.Message);
            }

            finally { SyncRoot.ExitWriteLock(); }

            return this;
        }

        public DatabaseManager LoadTables()
        {
            var orderedTables = new Type[Tables.Count()];

            var dontCatch = new List<Type>();

            foreach (var tableType in Tables)
            {
                var definition = ServiceLocator.Provider.GetService<TableManager>().GetDefinition(tableType);
                var attribute = definition.TableAttribute;

                if (attribute.Load)
                {
                    if (attribute.ReadingOrder >= 0)
                        orderedTables[attribute.ReadingOrder] = tableType;
                }
                else
                    dontCatch.Add(tableType);
            }
            foreach (var table in Tables)
            {
                if (orderedTables.Contains(table) || dontCatch.Contains(table))
                    continue;

                for (var i = Tables.Count() - 1; i >= 0; i--)
                {
                    if (orderedTables[i] == null)
                    {
                        orderedTables[i] = table;
                        break;
                    }
                }
            }
            foreach (var type in orderedTables)
            {
                if (type != null)
                    LoadTable(type);
            }

            return this;
        }
        private DatabaseManager LoadTable(Type type)
        {
            var reader = new DatabaseReader(type);
            var tableName = reader.TableName;

            if (DatabaseReader.NOTIFY_PROGRESS)
                ServiceLocator.Provider.GetService<ILogger>().LogDatabase("Table " + tableName.FirstLetterUpper() + " loaded..");
            reader.Read(UseConnection());

            return this;
        }
        public DatabaseManager LoadTable<T>() where T : IRecord
        {
            LoadTable(typeof(T));

            return this;
        }

        public void DropAllTablesIfExists()
        {
            foreach (var type in Tables)
            {
                var definition = ServiceLocator.Provider.GetService<TableManager>().GetDefinition(type);
                TableAttribute attribute = definition.TableAttribute;
                DropTableIfExists(attribute.TableName);
            }
        }
        public void DropTableIfExists(string tableName)
        {
            ExecuteNonQuery(DatabaseOperation.DROP_TABLE, tableName);
        }
        public void DropTableIfExists(Type type)
        {
            var definition = ServiceLocator.Provider.GetService<TableManager>().GetDefinition(type);
            DropTableIfExists(definition.TableAttribute.TableName);
        }
        public void DropTableIfExists<T>() where T : IRecord
        {
            DropTableIfExists(ServiceLocator.Provider.GetService<TableManager>().GetDefinition(typeof(T)).TableAttribute.TableName);
        }
        public void DeleteTable<T>() where T : IRecord
        {
            var definition = ServiceLocator.Provider.GetService<TableManager>().GetDefinition(typeof(T));
            DeleteTable(definition.TableAttribute.TableName);
        }
        public void DeleteTable(string tableName)
        {
            ExecuteNonQuery(DatabaseOperation.DELETE_TABLE, tableName);
        }
        public void CreateTableIfNotExists(Type type)
        {
            var definition = ServiceLocator.Provider.GetService<TableManager>().GetDefinition(type);

            string tableName = definition.TableAttribute.TableName;

            PropertyInfo primaryProperty = definition.PrimaryProperty;

            string str = string.Empty;

            foreach (var property in definition.Properties)
            {
                string pType = TableConverter.ConvertProperty(property);
                str += property.Name + " " + pType + ",";
            }

            if (primaryProperty != null)
                str += "PRIMARY KEY (" + primaryProperty.Name + ")";
            else
                str = str.Remove(str.Length - 1, 1);

            ExecuteNonQuery(DatabaseOperation.CREATE_TABLE, tableName, str);

        }
        public void CreateAllTablesIfNotExists()
        {
            foreach (var type in Tables)
            {
                CreateTableIfNotExists(type);
            }
        }
        public void CreateTableIfNotExists<T>() where T : IRecord
        {
            CreateTableIfNotExists(typeof(T));
        }
    }
}
