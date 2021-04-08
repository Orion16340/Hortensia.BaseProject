using Hortensia.ORM.Enums;
using Hortensia.ORM.Interfaces;
using Hortensia.ORM.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Hortensia.Core;
using Hortensia.Core.Threads;

namespace Hortensia.ORM
{
    public static class DataExtensions
    {
        public static void AddElement<T>(this T table) where T : IRecord
        {
            ServiceLocator.Provider.GetService<ISaveManager>().AddElement(table);
            ServiceLocator.Provider.GetService<ITableManager>().AddToContainer(table);
        }
        public static void RemoveElement<T>(this T table) where T : IRecord
        {
            ServiceLocator.Provider.GetService<ISaveManager>().RemoveElement(table);
            ServiceLocator.Provider.GetService<ITableManager>().RemoveFromContainer(table);
        }
        public static void UpdateElement<T>(this T table) where T : IRecord
        {
            ServiceLocator.Provider.GetService<ISaveManager>().UpdateElement(table);
        }
        public static void AddInstantElement<T>(this T table) where T : IRecord
        {
            ServiceLocator.Provider.GetService<ITableManager>().GetWriter(typeof(T)).Use(new IRecord[] { table }, DatabaseAction.Add);
            ServiceLocator.Provider.GetService<ITableManager>().AddToContainer(table);
        }
        public static void AddInstantElements(this IEnumerable<IRecord> tables, Type type)
        {
            ServiceLocator.Provider.GetService<ITableManager>().GetWriter(type).Use(tables.ToArray(), DatabaseAction.Add);

            foreach (var table in tables)
            {
                ServiceLocator.Provider.GetService<ITableManager>().AddToContainer(table);
            }
        }
        public static void UpdateInstantElement<T>(this T table) where T : IRecord
        {
            ServiceLocator.Provider.GetService<ITableManager>().GetWriter(typeof(T)).Use(new IRecord[] { table }, DatabaseAction.Update);
        }
        public static void UpdateInstantElements(this IEnumerable<IRecord> records, Type type)
        {
            ServiceLocator.Provider.GetService<ITableManager>().GetWriter(type).Use(records.ToArray(), DatabaseAction.Update);
        }

        public static void RemoveInstantElement<T>(this T table) where T : IRecord
        {
            ServiceLocator.Provider.GetService<ITableManager>().GetWriter(typeof(T)).Use(new IRecord[] { table }, DatabaseAction.Remove);
            ServiceLocator.Provider.GetService<ITableManager>().RemoveFromContainer(table);

        }
        public static void RemoveInstantElements(this IEnumerable<IRecord> tables, Type type)
        {
            ServiceLocator.Provider.GetService<ITableManager>().GetWriter(type).Use(tables.ToArray(), DatabaseAction.Remove);

            foreach (var table in tables)
            {
                ServiceLocator.Provider.GetService<ITableManager>().RemoveFromContainer(table);
            }
        }

        public static int DynamicPop<T>(this List<T> data) where T : IRecord
             => data.Count == 0 ? 1 : data.Count + 1;

        public static void EachData<T>(this List<T> data, Action<T> action) where T : IRecord
        {
            foreach (var item in data)
                action(item);
        }

        public static T FindWhere<T>(this List<T> data, Func<T, bool> action) where T : IRecord
            => (T)data.FirstOrDefault(action);

        public static IEnumerable<T> FindAllWhere<T>(this List<T> data, Func<T, bool> action) where T : IRecord
            => data.Where(action);

        public static IServiceCollection AddORMInfrastructure(this IServiceCollection services)
        {
            IContextHandler databasePool = new AsyncPool(150, "Database");
            var database = new DatabaseManager(databasePool);

            services
                    .AddSingleton<ITableManager, TableManager>()
                    .AddSingleton<IDatabaseManager>(database)
                    .AddSingleton<IBackupManager, BackupManager>()
                    .AddSingleton<ISaveManager, SaveManager>();

            return services;
        }
    }
}
