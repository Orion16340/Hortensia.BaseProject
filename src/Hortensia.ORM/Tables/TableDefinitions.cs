using Hortensia.ORM.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hortensia.ORM.Tables
{
    public class TableDefinitions
    {
        public FieldInfo Container;

        public IList ContainerValue;

        public TableAttribute TableAttribute;

        public PropertyInfo[] Properties;

        public PropertyInfo PrimaryProperty;

        public Dictionary<Type, MethodInfo> CustomSerializationMethods;

        public Dictionary<Type, MethodInfo> CustomDeserializationMethods;

        public bool Load => TableAttribute.Load;
        public TableDefinitions(Type type)
        {
            var attribute = type.GetCustomAttribute<TableAttribute>();

            if (attribute == null)
                throw new Exception("Unable to find table attribute for table " + type.Name);

            var field = type.GetFields().FirstOrDefault(x => x.GetCustomAttribute(typeof(ContainerAttribute), false) != null);

            if (field == null)
            {
                if (attribute.Load)
                    throw new Exception("Unable to find container for table : " + type.Name);
            }
            else
            {
                this.Container = field;
                this.ContainerValue = (IList)Container.GetValue(null);
            }

            this.TableAttribute = attribute;

            this.Properties = type.GetProperties().Where(x => x.GetCustomAttribute(typeof(NotMappedAttribute), false) == null).OrderBy(x => x.MetadataToken).ToArray();

            this.PrimaryProperty = GetPrimaryProperty();

            this.CustomSerializationMethods = GetCustomSerializationMethods(type);

            this.CustomDeserializationMethods = GetCustomDeserializationMethods(type);

        }

        private Dictionary<Type, MethodInfo> GetCustomSerializationMethods(Type tableType)
        {
            Dictionary<Type, MethodInfo> results = new Dictionary<Type, MethodInfo>();

            foreach (var method in tableType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (method.GetCustomAttribute<CustomSerializeAttribute>() != null)
                {
                    var type = method.GetParameters()[0].ParameterType;
                    results.Add(type, method);
                }
            }

            return results;
        }

        private Dictionary<Type, MethodInfo> GetCustomDeserializationMethods(Type tableType)
        {
            Dictionary<Type, MethodInfo> results = new Dictionary<Type, MethodInfo>();

            foreach (var method in tableType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (method.GetCustomAttribute<CustomDeserializeAttribute>() != null)
                {
                    var type = method.ReturnType;
                    results.Add(type, method);
                }
            }

            return results;
        }

        private PropertyInfo GetPrimaryProperty()
        {
            var properties = Properties.Where(property => property.GetCustomAttribute(typeof(PrimaryAttribute), false) != null);

            if (properties.Count() != 1)
            {
                if (properties.Count() == 0)
                    throw new Exception(string.Format("The Table '{0}' hasn't got a primary property", TableAttribute.TableName));

                if (properties.Count() > 1)
                    throw new Exception(string.Format("The Table '{0}' has too much primary properties", TableAttribute.TableName));
            }

            return properties.First();
        }

    }
}
