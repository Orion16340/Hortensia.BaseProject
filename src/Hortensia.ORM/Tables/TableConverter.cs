using Hortensia.ORM.Attributes;
using Hortensia.ORM.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hortensia.ORM.Tables
{
    public static class TableConverter
    {
        private static readonly Dictionary<DatabaseOperation, string> Requests = new()
        {
            { DatabaseOperation.CREATE_TABLE, "CREATE TABLE if not exists {0} ({1})" },
            { DatabaseOperation.DROP_TABLE, "DROP TABLE IF EXISTS {0}" },
            { DatabaseOperation.DELETE_TABLE, "DELETE FROM {0}" },
            { DatabaseOperation.SELECT, "SELECT * FROM `{0}`" },
            { DatabaseOperation.SELECT_WHERE, "SELECT * FROM `{0}` WHERE {1}" },
            { DatabaseOperation.COUNT_CONDITIONAL, "SELECT COUNT(*) FROM `{0}` WHERE {1}" },
            { DatabaseOperation.COUNT, "SELECT COUNT(*) FROM `{0}`" },
            { DatabaseOperation.INSERT, "INSERT INTO `{0}` VALUES {1}" },
            { DatabaseOperation.UPDATE, "UPDATE `{0}` SET {1} WHERE {2} = {3}" },
            { DatabaseOperation.REMOVE, "DELETE FROM `{0}` WHERE `{1}` = {2}" }
        };

        public static string GetOperation(DatabaseOperation type)
            => Requests[type];

        public static string ConvertProperty(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<TypeOverrideAttribute>();
            var sizeAttribute = property.GetCustomAttribute<SizeAttribute>();

            if (attribute != null)
                return attribute.NewType;

            if (property.GetCustomAttribute<ProtoSerializeAttribute>() != null)
                return "BLOB";

            string result = string.Empty;

            if (sizeAttribute != null)
                result += property.PropertyType switch
                {
                    Type type when type == typeof(sbyte) || type == typeof(byte) || type == typeof(bool) => $"TINYINT({sizeAttribute.Value})",
                    Type type when type == typeof(ushort) || type == typeof(short) => $"SMALLINT({sizeAttribute.Value})",
                    Type type when type == typeof(uint) || type == typeof(int) => $"INT({sizeAttribute.Value})",
                    Type type when type == typeof(ulong) || type == typeof(long) => $"BIGINT({sizeAttribute.Value})",
                    Type type when type == typeof(double) => "FLOAT",
                    Type type when (type == typeof(string) || type.IsEnum || type == typeof(List<>) || type.IsArray) && property.GetCustomAttribute<ProtoSerializeAttribute>() == null => $"VARCHAR({sizeAttribute.Value})",
                    Type type when type == typeof(DateTime) => "DATETIME",
                    _ => "",
                };
            else
            {
                result += property.PropertyType switch
                {
                    Type type when type == typeof(sbyte) || type == typeof(byte) || type == typeof(bool) => "TINYINT(4)",
                    Type type when type == typeof(ushort) || type == typeof(short) => "SMALLINT(6)",
                    Type type when type == typeof(uint) || type == typeof(int) => "INT(11)",
                    Type type when type == typeof(ulong) || type == typeof(long) => "BIGINT(20)",
                    Type type when type == typeof(double) => "FLOAT",
                    Type type when (type == typeof(string) || type.IsEnum || type == typeof(List<>) || type.IsArray) && property.GetCustomAttribute<ProtoSerializeAttribute>() == null => "VARCHAR(255)",
                    Type type when type == typeof(DateTime) => "DATETIME",
                    _ => "",
                };
            }

            if (property.Name == "Id")
                result += " AUTO_INCREMENT";

            return result;
        }
    }
}
