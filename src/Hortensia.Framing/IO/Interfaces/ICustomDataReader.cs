using Hortensia.Framing.IO.Interfaces;
using System;

namespace Hortensia.Framing.IO
{
	public interface ICustomDataReader : IDataReader, IDisposable
	{
		int ReadVarInt();

		long ReadVarLong();

		short ReadVarShort();

		uint ReadVarUhInt();

		ulong ReadVarUhLong();

		ushort ReadVarUhShort();
	}
}