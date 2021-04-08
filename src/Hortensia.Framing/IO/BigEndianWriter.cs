using Hortensia.Framing.IO.Interfaces;
using System;
using System.IO;
using System.Text;

namespace Hortensia.Framing.IO
{
	public class BigEndianWriter : IDataWriter, IDisposable
	{
		private BinaryWriter m_writer;

		public Stream BaseStream
		{
			get
			{
				return this.m_writer.BaseStream;
			}
		}

		public long BytesAvailable
		{
			get
			{
				long length = this.m_writer.BaseStream.Length - this.m_writer.BaseStream.Position;
				return length;
			}
		}

		public byte[] Data
		{
			get
			{
				long position = this.m_writer.BaseStream.Position;
				byte[] numArray = new byte[(int)checked((IntPtr)this.m_writer.BaseStream.Length)];
				this.m_writer.BaseStream.Position = (long)0;
				this.m_writer.BaseStream.Read(numArray, 0, (int)this.m_writer.BaseStream.Length);
				this.m_writer.BaseStream.Position = position;
				return numArray;
			}
		}

		public int Position
		{
			get
			{
				return JustDecompileGenerated_get_Position();
			}
			set
			{
				JustDecompileGenerated_set_Position(value);
			}
		}

		public int JustDecompileGenerated_get_Position()
		{
			return (int)this.m_writer.BaseStream.Position;
		}

		public void JustDecompileGenerated_set_Position(int value)
		{
			this.m_writer.BaseStream.Position = (long)value;
		}

		public BigEndianWriter()
		{
			this.m_writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
		}

		public BigEndianWriter(Stream stream)
		{
			this.m_writer = new BinaryWriter(stream, Encoding.UTF8);
		}

		public void Clear()
		{
			this.m_writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
		}

		public void Dispose()
		{
			this.m_writer.Flush();
			this.m_writer.Dispose();
			this.m_writer = null;
		}

		public void Seek(int offset)
		{
			this.Seek(offset, SeekOrigin.Begin);
		}

		public void Seek(int offset, SeekOrigin seekOrigin)
		{
			this.m_writer.BaseStream.Seek((long)offset, seekOrigin);
		}

		private void WriteBigEndianBytes(byte[] endianBytes)
		{
			for (int i = (int)endianBytes.Length - 1; i >= 0; i--)
			{
				this.m_writer.Write(endianBytes[i]);
			}
		}

		public void WriteBoolean(bool @bool)
		{
			if (!@bool)
			{
				this.m_writer.Write((byte)0);
			}
			else
			{
				this.m_writer.Write((byte)1);
			}
		}

		public void WriteByte(byte @byte)
		{
			this.m_writer.Write(@byte);
		}

		public void WriteBytes(byte[] data)
		{
			this.m_writer.Write(data);
		}

		public void WriteChar(char @char)
		{
			this.WriteBigEndianBytes(BitConverter.GetBytes(@char));
		}

		public void WriteDouble(double @double)
		{
			this.WriteBigEndianBytes(BitConverter.GetBytes(@double));
		}

		public void WriteFloat(float @float)
		{
			this.m_writer.Write(@float);
		}

		public void WriteInt(int @int)
		{
			this.WriteBigEndianBytes(BitConverter.GetBytes(@int));
		}

		public void WriteLong(long @long)
		{
			this.WriteBigEndianBytes(BitConverter.GetBytes(@long));
		}

		public void WriteSByte(sbyte @byte)
		{
			this.m_writer.Write(@byte);
		}

		public void WriteShort(short @short)
		{
			this.WriteBigEndianBytes(BitConverter.GetBytes(@short));
		}

		public void WriteSingle(float single)
		{
			this.WriteBigEndianBytes(BitConverter.GetBytes(single));
		}

		public void WriteUInt(uint @uint)
		{
			this.WriteBigEndianBytes(BitConverter.GetBytes(@uint));
		}

		public void WriteULong(ulong @ulong)
		{
			this.WriteBigEndianBytes(BitConverter.GetBytes(@ulong));
		}

		public void WriteUShort(ushort @ushort)
		{
			this.WriteBigEndianBytes(BitConverter.GetBytes(@ushort));
		}

		public void WriteUTF(string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			ushort length = (ushort)((int)bytes.Length);
			this.WriteUShort(length);
			for (int i = 0; i < length; i++)
			{
				this.m_writer.Write(bytes[i]);
			}
		}

		public void WriteUTFBytes(string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			int length = (int)bytes.Length;
			for (int i = 0; i < length; i++)
			{
				this.m_writer.Write(bytes[i]);
			}
		}
	}
}