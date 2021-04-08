using Hortensia.Framing.IO;

namespace Hortensia.Framing
{
    public abstract class NetworkMessage
    {
        public const byte BIT_RIGHT_SHIFT_LEN_PACKET_ID = 2;
        public const byte BIT_MASK = 3;

        private static uint GLOBAL_INSTANCE = 0;
        public abstract uint MessageId { get; }

        public abstract void Serialize(ICustomDataWriter writer);
        public abstract void Deserialize(ICustomDataReader reader);

        public void UnPack(ICustomDataReader reader)
            => Deserialize(reader);

        public void Pack(ICustomDataWriter writer)
        {
            CustomDataWriter customWriter = new();
            Serialize(customWriter);

            var size = customWriter.Data.Length;

            uint typeLen = ComputeTypeLen((uint)size);

            writer.WriteShort((short)SubComputeStaticHeader(MessageId, typeLen));
            writer.WriteUInt(GLOBAL_INSTANCE++);

            switch (typeLen)
            {
                case 1:
                    writer.WriteByte((byte)size);
                    break;
                case 2:
                    writer.WriteUShort((ushort)size);
                    break;
                case 3:
                    writer.WriteByte((byte)(size >> 16 & 255));
                    writer.WriteUShort((ushort)(size & 65535));
                    break;
            }
            
            writer.WriteBytes(customWriter.Data);
            customWriter.Dispose();
        }

        public static uint ComputeTypeLen(uint len)
            => len switch
            {
                uint value when value > 65535 => 3,
                uint value when value > 255 => 2,
                uint value when value > 0 => 1,
                _ => 0,
            };

        private static uint SubComputeStaticHeader(uint messageId, uint typeLen)
            => messageId << BIT_RIGHT_SHIFT_LEN_PACKET_ID | typeLen;

        public override string ToString()
            => this.GetType().Name;
    }
}
