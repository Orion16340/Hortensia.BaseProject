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
    public interface IDatabaseManager
    {
        IDatabaseManager CloseConnection();
        void CreateAllTablesIfNotExists();
        void CreateTableIfNotExists(Type type);
        void CreateTableIfNotExists<T>() where T : IRecord;
        void DeleteTable(string tableName);
        void DeleteTable<T>() where T : IRecord;
        void DropAllTablesIfExists();
        void DropTableIfExists(string tableName);
        void DropTableIfExists(Type type);
        void DropTableIfExists<T>() where T : IRecord;
        IDatabaseManager ExecuteNonQuery(DatabaseOperation operation, params object[] args);
        IDatabaseManager InitializeAutoSave(TimerConfigurationEntry configuration);
        IDatabaseManager InitializeBackup(string directory);
        IDatabaseManager InitializeDatabase(DatabaseConfiguration configuration, Assembly repositoryAssembly);
        IDatabaseManager LoadTable<T>() where T : IRecord;
        IDatabaseManager LoadTables();
        T Query<T>(string fieldName, string fieldValue) where T : IRecord;
        IDatabaseManager RegisterTable<T>() where T : IRecord;
        IDatabaseManager Set();
        IDatabaseManager StartBackup(string filePath);
        MySqlConnection UseConnection();
    }

    public class DatabaseManager : IDatabaseManager
    {
        public static ReaderWriterLockSlim SyncRoot = new();
        private MySqlConnection Connection { get; set; }
        private List<Type> Tables { get; set; }
        private IContextHandler IOTaskPool { get; set; }

        public DatabaseManager(IContextHandler pool)
            => IOTaskPool = pool;

        public IDatabaseManager InitializeDatabase(DatabaseConfiguration configuration, Assembly repositoryAssembly)
        {
            if (Connection != null)
                throw new Exception("There is already an instance of DatabaseManager.");

            Connection = new MySqlConnection(configuration.ToString());
            Tables = Array.FindAll(repositoryAssembly.GetTypes(), x => x.GetInterface("IRecord") != null).ToList();

            return this;
        }

        public IDatabaseManager RegisterTable<T>() where T : IRecord
        {
            Tables.Add(typeof(T));
            return this;
        }

        public IDatabaseManager Set()
        {
            ServiceLocator.Provider.GetService<ITableManager>().Initialize(Tables.ToArray());
            return this;
        }

        public MySqlConnection UseConnection()
        {
            Connection.Close();

            if (!Connection.Ping())
            {
                Connection.Close();
                Connection.Open();
            }

            return Connection;
        }

        public IDatabaseManager CloseConnection()
        {
            Connection.Close();

            return this;
        }

        public IDatabaseManager InitializeBackup(string directory)
        {
            ServiceLocator.Provider.GetService<IBackupManager>().Initialize(directory);
            return this;
        }

        public IDatabaseManager InitializeAutoSave(TimerConfigurationEntry configuration)
        {
            IOTaskPool.ExecutePeriodically(ServiceLocator.Provider.GetService<ISaveManager>().Save, configuration);
            IOTaskPool.Start();

            return this;
        }

        public IDatabaseManager StartBackup(string filePath)
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

        public IDatabaseManager ExecuteNonQuery(DatabaseOperation operation, params object[] args)
        {
            SyncRoot.EnterWriteLock();

            try
            {
                string query = string.Format(TableConverter.GetOperation(operation), args);
                using var command = new MySqlCommand(query, Connection);
                command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                ServiceLocator.Provider.GetService<ILogger>().LogError(ex.Message);
            }

            finally { SyncRoot.ExitWriteLock(); }

            return this;
        }

        public IDatabaseManager LoadTables()
        {
            var orderedTables = new Type[Tables.Count()];

            var dontCatch = new List<Type>();

            foreach (var tableType in Tables)
            {
                var definition = ServiceLocator.Provider.GetService<ITableManager>().GetDefinition(tableType);
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
        private IDatabaseManager LoadTable(Type type)
        {
            var reader = new DatabaseReader(type);
            var tableName = reader.TableName;

            if (DatabaseReader.NOTIFY_PROGRESS)
                ServiceLocator.Provider.GetService<ILogger>().LogDatabase("Table " + tableName.FirstLetterUpper() + " loaded..");
            reader.Read(UseConnection());

            return this;
        }
        public IDatabaseManager LoadTable<T>() where T : IRecord
        {
            LoadTable(typeof(T));

            return this;
        }

        public T Query<T>(string fieldName, string fieldValue) where T : IRecord
        {
            var reader = new DatabaseReader(typeof(T));
            var where = $"{fieldName}='{fieldValue}'";
            return (T)reader.ReadFirst(UseConnection(), where);
        }

        public void DropAllTablesIfExists()
        {
            foreach (var type in Tables)
            {
                var definition = ServiceLocator.Provider.GetService<ITableManager>().GetDefinition(type);
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
            var definition = ServiceLocator.Provider.GetService<ITableManager>().GetDefinition(type);
            DropTableIfExists(definition.TableAttribute.TableName);
        }
        public void DropTableIfExists<T>() where T : IRecord
        {
            DropTableIfExists(ServiceLocator.Provider.GetService<ITableManager>().GetDefinition(typeof(T)).TableAttribute.TableName);
        }
        public void DeleteTable<T>() where T : IRecord
        {
            var definition = ServiceLocator.Provider.GetService<ITableManager>().GetDefinition(typeof(T));
            DeleteTable(definition.TableAttribute.TableName);
        }
        public void DeleteTable(string tableName)
        {
            ExecuteNonQuery(DatabaseOperation.DELETE_TABLE, tableName);
        }
        public void CreateTableIfNotExists(Type type)
        {
            var definition = ServiceLocator.Provider.GetService<ITableManager>().GetDefinition(type);

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
