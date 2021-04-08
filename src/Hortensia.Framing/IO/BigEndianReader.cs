using Hortensia.Framing.IO.Interfaces;
using System;
using System.IO;
using System.Text;

namespace Hortensia.Framing.IO
{
	public class BigEndianReader : IDataReader, IDisposable
	{
		private BinaryReader m_reader;

		public Stream BaseStream
		{
			get
			{
				return this.m_reader.BaseStream;
			}
		}

		public long BytesAvailable
		{
			get
			{
				long length = this.m_reader.BaseStream.Length - this.m_reader.BaseStream.Position;
				return length;
			}
		}

		public byte[] Data
		{
			get
			{
				long position = this.BaseStream.Position;
				byte[] numArray = new byte[(int)checked((IntPtr)this.BaseStream.Length)];
				this.BaseStream.Position = (long)0;
				this.BaseStream.Read(numArray, 0, (int)this.BaseStream.Length);
				this.BaseStream.Position = position;
				return numArray;
			}
		}

		public long Position
		{
			get
			{
				return this.m_reader.BaseStream.Position;
			}
		}

		public BigEndianReader()
		{
			this.m_reader = new BinaryReader(new MemoryStream(), Encoding.UTF8);
		}

		public BigEndianReader(Stream stream)
		{
			this.m_reader = new BinaryReader(stream, Encoding.UTF8);
		}

		public BigEndianReader(byte[] tab)
		{
			this.m_reader = new BinaryReader(new MemoryStream(tab), Encoding.UTF8);
		}

		public void Add(byte[] data, int offset, int count)
		{
			long position = this.m_reader.BaseStream.Position;
			this.m_reader.BaseStream.Position = this.m_reader.BaseStream.Length;
			this.m_reader.BaseStream.Write(data, offset, count);
			this.m_reader.BaseStream.Position = position;
		}

		public void Dispose()
		{
			this.m_reader.Dispose();
			this.m_reader = null;
		}

		private byte[] ReadBigEndianBytes(int count)
		{
			byte[] numArray = new byte[count];
			for (int i = count - 1; i >= 0; i--)
			{
				numArray[i] = (byte)this.BaseStream.ReadByte();
			}
			return numArray;
		}

		public bool ReadBoolean()
		{
			return this.m_reader.ReadByte() == 1;
		}

		public byte ReadByte()
		{
			return this.m_reader.ReadByte();
		}

		public byte[] ReadBytes(int n)
		{
			return this.m_reader.ReadBytes(n);
		}

		public BigEndianReader ReadBytesInNewBigEndianReader(int n)
		{
			return new BigEndianReader(this.m_reader.ReadBytes(n));
		}

		public char ReadChar()
		{
			return (char)this.ReadUShort();
		}

		public double ReadDouble()
		{
			return BitConverter.ToDouble(this.ReadBigEndianBytes(8), 0);
		}

		public float ReadFloat()
		{
			return BitConverter.ToSingle(this.ReadBigEndianBytes(4), 0);
		}

		public int ReadInt()
		{
			return BitConverter.ToInt32(this.ReadBigEndianBytes(4), 0);
		}

		public long ReadLong()
		{
			return BitConverter.ToInt64(this.ReadBigEndianBytes(8), 0);
		}

		public sbyte ReadSByte()
		{
			return this.m_reader.ReadSByte();
		}

		public short ReadShort()
		{
			return BitConverter.ToInt16(this.ReadBigEndianBytes(2), 0);
		}

		public float ReadSingle()
		{
			return BitConverter.ToSingle(this.ReadBigEndianBytes(4), 0);
		}

		public uint ReadUInt()
		{
			return BitConverter.ToUInt32(this.ReadBigEndianBytes(4), 0);
		}

		public ulong ReadULong()
		{
			return BitConverter.ToUInt64(this.ReadBigEndianBytes(8), 0);
		}

		public ushort ReadUShort()
		{
			return BitConverter.ToUInt16(this.ReadBigEndianBytes(2), 0);
		}

		public string ReadUTF()
		{
			byte[] numArray = this.ReadBytes((int)this.ReadUShort());
			return Encoding.UTF8.GetString(numArray);
		}

		public string ReadUTF7BitLength()
		{
			byte[] numArray = this.ReadBytes(this.ReadInt());
			return Encoding.UTF8.GetString(numArray);
		}

		public string ReadUTFBytes(ushort len)
		{
			byte[] numArray = this.ReadBytes((int)len);
			return Encoding.UTF8.GetString(numArray);
		}

		public void Seek(int offset, SeekOrigin seekOrigin = 0)
		{
			this.m_reader.BaseStream.Seek((long)offset, seekOrigin);
		}

		public void SkipBytes(int n)
		{
			for (int i = 0; i < n; i++)
			{
				this.m_reader.ReadByte();
			}
		}
	}
}