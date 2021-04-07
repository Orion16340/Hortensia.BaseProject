using Hortensia.ORM.Attributes;
using Hortensia.ORM.Enums;
using Hortensia.ORM.Extensions;
using Hortensia.ORM.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Hortensia.Core;

namespace Hortensia.ORM.Tables.IO
{
    public class DatabaseWriter
    {
        private string TableName
        {
            get;
            set;
        }
        private PropertyInfo[] AddProperties
        {
            get;
            set;
        }
        private PropertyInfo[] UpdateProperties
        {
            get;
            set;
        }
        private PropertyInfo PrimaryProperty
        {
            get;
            set;
        }
        private Dictionary<Type, MethodInfo> CustomSerializationMethods
        {
            get;
            set;
        }
        private Type Type
        {
            get;
            set;
        }
        public bool HasNoUpdateProperties
        {
            get;
            set;
        }
        public DatabaseWriter(Type type)
        {
            this.Type = type;
            var definition = ServiceLocator.Provider.GetService<TableManager>().GetDefinition(type);
            this.AddProperties = GetAddProperties(type);
            this.UpdateProperties = GetUpdateProperties(type);
            this.TableName = definition.TableAttribute.TableName;
            this.PrimaryProperty = definition.PrimaryProperty;
            this.HasNoUpdateProperties = UpdateProperties.Length == 0;
            this.CustomSerializationMethods = definition.CustomSerializationMethods;
        }
        public void Use(IRecord[] elements, DatabaseAction action)
        {
            lock (DatabaseManager.SyncRoot)
            {
                switch (action)
                {
                    case DatabaseAction.Add:
                        this.AddElements(elements);
                        return;

                    case DatabaseAction.Update:
                        this.UpdateElements(elements);
                        return;
                    case DatabaseAction.Remove:
                        this.DeleteElements(elements);
                        return;

                }
            }
        }

        private void AddElements(IRecord[] elements)
        {
            var command = new MySqlCommand(string.Empty, ServiceLocator.Provider.GetService<DatabaseManager>().UseConnection());

            List<string> final = new List<string>();

            for (int i = 0; i < elements.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("(");

                foreach (var property in AddProperties)
                {
                    sb.Append(string.Format("?{0}{1},", property.Name, i));
                    MySqlParameter mySQLParam = new MySqlParameter("?" + property.Name + i, ConvertObject(property, elements[i]));
                    command.Parameters.Add(mySQLParam);
                }

                sb = sb.Remove(sb.Length - 1, 1);
                sb.Append(")");

                final.Add(sb.ToString());
            }

            command.CommandText = string.Format(TableConverter.GetOperation(DatabaseOperation.INSERT), TableName, string.Format("{0}", string.Join(",", final)));

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add element to database (" + TableName + ") " +
                     ex.Message);
            }
        }

        private void UpdateElements(IRecord[] elements)
        {
            if (HasNoUpdateProperties)
            {
                throw new Exception("Unable to update elements. " + TableName + " has no update property.");
            }


            var command = new MySqlCommand(string.Empty, ServiceLocator.Provider.GetService<DatabaseManager>().UseConnection());

            List<string> final = new List<string>();

            for (int i = 0; i < elements.Length; i++)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var property in UpdateProperties)
                {
                    sb.Append(string.Format("{2} = ?{0}{1},", property.Name, i, property.Name));
                    MySqlParameter mySQLParam = new MySqlParameter("?" + property.Name + i, ConvertObject(property, elements[i]));
                    command.Parameters.Add(mySQLParam);
                }

                sb = sb.Remove(sb.Length - 1, 1);

                final.Add(sb.ToString());

                var finalText = string.Format("{0}", string.Join(",", final));

                string arg1 = TableName;
                string arg2 = finalText;
                command.CommandText = string.Format(TableConverter.GetOperation(DatabaseOperation.UPDATE), arg1, arg2, PrimaryProperty.Name, elements[i].Id.ToString());

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable update database element (" + TableName + ") " +
                         ex.Message);
                }
            }



        }
        private void DeleteElements(IEnumerable<IRecord> elements)
        {
            foreach (var element in elements)
            {
                var commandString = string.Format(TableConverter.GetOperation(DatabaseOperation.REMOVE), TableName, PrimaryProperty.Name, PrimaryProperty.GetValue(element));

                using (var command = new MySqlCommand(commandString, ServiceLocator.Provider.GetService<DatabaseManager>().UseConnection()))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private object ConvertObject(PropertyInfo property, IRecord element)
        {
            var value = property.GetValue(element);

            if (value == null)
            {
                return null;
            }
            if (property.PropertyType == typeof(DateTime))
            {
                value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (property.PropertyType == typeof(Boolean))
            {
                value = Convert.ToByte(value);
            }
            else if (property.CustomAttributes.Count() > 0 && property.GetCustomAttribute<ProtoSerializeAttribute>() != null)
            {
                byte[] content = ProtobufExtensions.Serialize(value);
                return content;
            }
            else if (CustomSerializationMethods.ContainsKey(property.PropertyType))
            {
                value = CustomSerializationMethods[property.PropertyType].Invoke(null, new object[] { value });
            }
            else if (value is Enum)
            {
                value = value.ToString();
            }
            else if (property.PropertyType.IsGenericType)
            {
                List<object> results = new List<object>();

                Type genericType = property.PropertyType.GetGenericTypeDefinition();

                if (genericType == typeof(List<>))
                {
                    var values = (IList)value;

                    foreach (var v in values)
                    {
                        results.Add(v);
                    }
                    return string.Join(",", results);
                }
                else
                {
                    throw new Exception("Unhandled generic type " + property.PropertyType.Name);
                }
            }

            return value;
        }

        public static PropertyInfo[] GetUpdateProperties(Type type)
        {
            return type.GetProperties().Where(property => property.GetCustomAttribute(typeof(NotMappedAttribute)) == null
            && property.GetCustomAttribute(typeof(UpdateAttribute), false) != null).OrderBy(x => x.MetadataToken).ToArray();
        }
        public static PropertyInfo[] GetAddProperties(Type type)
        {
            return type.GetProperties().Where(property => property.GetCustomAttribute(typeof(NotMappedAttribute)) == null).OrderBy(x => x.MetadataToken).ToArray();
        }

        public static void Update<T>(T item) where T : IRecord
        {
            ServiceLocator.Provider.GetService<TableManager>().GetWriter(typeof(T)).Use(new IRecord[] { item }, DatabaseAction.Update);
        }

        public static void Update<T>(IEnumerable<T> items) where T : IRecord
        {
            ServiceLocator.Provider.GetService<TableManager>().GetWriter(typeof(T)).Use(items.Cast<IRecord>().ToArray(), DatabaseAction.Update);
        }

        public static void Insert<T>(T item) where T : IRecord
        {
            ServiceLocator.Provider.GetService<TableManager>().GetWriter(typeof(T)).Use(new IRecord[] { item }, DatabaseAction.Add);
        }
        public static void Insert<T>(IEnumerable<T> items) where T : IRecord
        {
            ServiceLocator.Provider.GetService<TableManager>().GetWriter(typeof(T)).Use(items.Cast<IRecord>().ToArray(), DatabaseAction.Add);
        }

        public static void Remove<T>(T item) where T : IRecord
        {
            ServiceLocator.Provider.GetService<TableManager>().GetWriter(typeof(T)).Use(new IRecord[] { item }, DatabaseAction.Remove);
        }
        public static void Remove<T>(IEnumerable<T> items) where T : IRecord
        {
            ServiceLocator.Provider.GetService<TableManager>().GetWriter(typeof(T)).Use(items.Cast<IRecord>().ToArray(), DatabaseAction.Remove);

        }
        public static void CreateTable(Type type)
        {
            ServiceLocator.Provider.GetService<DatabaseManager>().CreateTableIfNotExists(type);
        }
    }
}
