using System;

namespace Hortensia.Framing.IO.Interfaces
{
	public interface IDataWriter
	{
		byte[] Data
		{
			get;
		}

		int Position
		{
			get;
		}

		void Clear();

		void Seek(int offset);

		void WriteBoolean(bool @bool);

		void WriteByte(byte @byte);

		void WriteBytes(byte[] data);

		void WriteChar(char @char);

		void WriteDouble(double @double);

		void WriteFloat(float @float);

		void WriteInt(int @int);

		void WriteLong(long @long);

		void WriteSByte(sbyte @byte);

		void WriteShort(short @short);

		void WriteSingle(float single);

		void WriteUInt(uint @uint);

		void WriteULong(ulong @ulong);

		void WriteUShort(ushort @ushort);

		void WriteUTF(string str);

		void WriteUTFBytes(string str);
	}
}