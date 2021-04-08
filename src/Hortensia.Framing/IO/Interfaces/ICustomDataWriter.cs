using Hortensia.Framing.IO.Interfaces;
using System;

namespace Hortensia.Framing.IO
{
	public interface ICustomDataWriter : IDataWriter
	{
		void WriteVarInt(int value);

		void WriteVarLong(long value);

		void WriteVarShort(short value);

		void WriteVarUhInt(uint value);

		void WriteVarUhLong(ulong value);

		void WriteVarUhShort(ushort value);
	}
}