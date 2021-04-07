using ProtoBuf;
using System;
using System.IO;

namespace Hortensia.ORM.Extensions
{
    public static class ProtobufExtensions
    {
        public static T Deserialize<T>(byte[] buffer) where T : class
        {
            using var memoryStream = new MemoryStream(buffer);
            return Serializer.Deserialize<T>(memoryStream);
        }

        public static object Deserialize(byte[] buffer, Type type)
        {
            using var memoryStream = new MemoryStream(buffer);
            return Serializer.Deserialize(type, memoryStream);
        }

        public static byte[] Serialize(object record)
        {
            if (record == null)
                throw new Exception();

            using var memoryStream = new MemoryStream();
            Serializer.Serialize<object>(memoryStream, record);

            return memoryStream.ToArray();
        }
    }
}
