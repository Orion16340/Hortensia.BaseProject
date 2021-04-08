using System;
using System.IO;

namespace Hortensia.Framing.IO.Interfaces
{
	public interface IDataReader : IDisposable
	{
		long BytesAvailable
		{
			get;
		}

		byte[] Data
		{
			get;
		}

		long Position
		{
			get;
		}

		bool ReadBoolean();

		byte ReadByte();

		byte[] ReadBytes(int n);

		char ReadChar();

		double ReadDouble();

		float ReadFloat();

		int ReadInt();

		long ReadLong();

		sbyte ReadSByte();

		short ReadShort();

		uint ReadUInt();

		ulong ReadULong();

		ushort ReadUShort();

		string ReadUTF();

		string ReadUTFBytes(ushort len);

		void Seek(int offset, SeekOrigin seekOrigin);

		void SkipBytes(int n);
	}
}