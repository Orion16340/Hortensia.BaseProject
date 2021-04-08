using Hortensia.Framing.IO.CustomTypes;
using Hortensia.Framing.IO.Interfaces;
using System;
using System.IO;

namespace Hortensia.Framing.IO
{
	public class CustomDataWriter : ICustomDataWriter, IDataWriter, IDisposable
	{
		private static int INT_SIZE;

		private static int SHORT_MIN_VALUE;

		private static int SHORT_MAX_VALUE;

		private static int UNSIGNED_SHORT_MAX_VALUE;

		private static int CHUNCK_BIT_SIZE;

		private static int MAX_ENCODING_LENGTH;

		private static int MASK_10000000;

		private static int MASK_01111111;

		private IDataWriter _data;

		public byte[] Data
		{
			get
			{
				return this._data.Data;
			}
		}

		public int Position
		{
			get
			{
				return this._data.Position;
			}
		}

		static CustomDataWriter()
		{
			CustomDataWriter.INT_SIZE = 32;
			CustomDataWriter.SHORT_MIN_VALUE = -32768;
			CustomDataWriter.SHORT_MAX_VALUE = 32767;
			CustomDataWriter.UNSIGNED_SHORT_MAX_VALUE = 65536;
			CustomDataWriter.CHUNCK_BIT_SIZE = 7;
			CustomDataWriter.MAX_ENCODING_LENGTH = (int)Math.Ceiling((double)CustomDataWriter.INT_SIZE / (double)CustomDataWriter.CHUNCK_BIT_SIZE);
			CustomDataWriter.MASK_10000000 = 128;
			CustomDataWriter.MASK_01111111 = 127;
		}

		public CustomDataWriter()
		{
			this._data = new BigEndianWriter();
		}

		public CustomDataWriter(Stream stream)
		{
			this._data = new BigEndianWriter(stream);
		}

		public void Clear()
		{
			this._data.Clear();
		}

		public void Dispose()
		{
			if (this._data is BigEndianWriter)
			{
				(this._data as BigEndianWriter).Dispose();
			}
		}

		public void Seek(int offset)
		{
			this._data.Seek(offset);
		}

		public void WriteBoolean(bool @bool)
		{
			this._data.WriteBoolean(@bool);
		}

		public void WriteByte(byte @byte)
		{
			this._data.WriteByte(@byte);
		}

		public void WriteBytes(byte[] data)
		{
			this._data.WriteBytes(data);
		}

		public void WriteChar(char @char)
		{
			this._data.WriteChar(@char);
		}

		public void WriteDouble(double @double)
		{
			this._data.WriteDouble(@double);
		}

		public void WriteFloat(float @float)
		{
			this._data.WriteFloat(@float);
		}

		public void WriteInt(int @int)
		{
			this._data.WriteInt(@int);
		}

		private static void writeint32(IDataWriter output, uint value)
		{
			while (value >= 128)
			{
				output.WriteByte((byte)(value & 127 | 128));
				value >>= 7;
			}
			output.WriteByte((byte)value);
		}

		public void WriteLong(long @long)
		{
			this._data.WriteLong(@long);
		}

		public void WriteSByte(sbyte @byte)
		{
			this._data.WriteSByte(@byte);
		}

		public void WriteShort(short @short)
		{
			this._data.WriteShort(@short);
		}

		public void WriteSingle(float single)
		{
			this._data.WriteSingle(single);
		}

		public void WriteUInt(uint @uint)
		{
			this._data.WriteUInt(@uint);
		}

		public void WriteULong(ulong @ulong)
		{
			this._data.WriteULong(@ulong);
		}

		public void WriteUShort(ushort @ushort)
		{
			this._data.WriteUShort(@ushort);
		}

		public void WriteUTF(string str)
		{
			this._data.WriteUTF(str);
		}

		public void WriteUTFBytes(string str)
		{
			this._data.WriteUTFBytes(str);
		}

		public void WriteVarInt(int value)
		{
			if ((value < 0 ? true : value > CustomDataWriter.MASK_01111111))
			{
				int mASK01111111 = 0;
				int cHUNCKBITSIZE = value;
				while (true)
				{
					if ((cHUNCKBITSIZE == 0 ? true : cHUNCKBITSIZE == -1))
					{
						break;
					}
					mASK01111111 = cHUNCKBITSIZE & CustomDataWriter.MASK_01111111;
					cHUNCKBITSIZE = cHUNCKBITSIZE >> (CustomDataWriter.CHUNCK_BIT_SIZE & 31);
					if (cHUNCKBITSIZE > 0)
					{
						mASK01111111 |= CustomDataWriter.MASK_10000000;
					}
					this._data.WriteByte((byte)mASK01111111);
				}
			}
			else
			{
				this._data.WriteByte((byte)value);
			}
		}

		public void WriteVarLong(long value)
		{
			uint i = 0;
			CustomInt64 customInt64 = CustomInt64.fromNumber(value);
			if (customInt64.high != 0)
			{
				for (i = 0; i < 4; i++)
				{
					this._data.WriteByte((byte)(customInt64.low & 127 | 128));
					customInt64.low >>= 7;
				}
				if ((customInt64.high & 2147483640) != 0)
				{
					this._data.WriteByte((byte)((customInt64.high << 4 | customInt64.low) & 127 | 128));
					CustomDataWriter.writeint32(this._data, customInt64.high >> 3);
				}
				else
				{
					this._data.WriteByte((byte)(customInt64.high << 4 | customInt64.low));
				}
			}
			else
			{
				CustomDataWriter.writeint32(this._data, customInt64.low);
			}
		}

		public void WriteVarShort(short value)
		{
			if ((value > CustomDataWriter.SHORT_MAX_VALUE ? true : value < CustomDataWriter.SHORT_MIN_VALUE))
			{
				throw new Exception("Forbidden value");
			}
			int mASK01111111 = 0;
			if ((value < 0 ? true : value > CustomDataWriter.MASK_01111111))
			{
				int cHUNCKBITSIZE = value & 65535;
				while (true)
				{
					if ((cHUNCKBITSIZE == 0 ? true : cHUNCKBITSIZE == -1))
					{
						break;
					}
					mASK01111111 = cHUNCKBITSIZE & CustomDataWriter.MASK_01111111;
					cHUNCKBITSIZE = cHUNCKBITSIZE >> (CustomDataWriter.CHUNCK_BIT_SIZE & 31);
					if (cHUNCKBITSIZE > 0)
					{
						mASK01111111 |= CustomDataWriter.MASK_10000000;
					}
					this._data.WriteByte((byte)mASK01111111);
				}
			}
			else
			{
				this._data.WriteByte((byte)value);
			}
		}

		public void WriteVarUhInt(uint value)
		{
			if ((ulong)value > (ulong)MASK_01111111)
			{
				uint mASK01111111 = 0;
				uint cHUNCKBITSIZE = value;
				while (cHUNCKBITSIZE != 0)
				{
					mASK01111111 = (uint)(cHUNCKBITSIZE & MASK_01111111);
					cHUNCKBITSIZE = cHUNCKBITSIZE >> (CustomDataWriter.CHUNCK_BIT_SIZE & 31);
					if (cHUNCKBITSIZE != 0)
					{
						mASK01111111 |= (uint)CustomDataWriter.MASK_10000000;
					}
					this._data.WriteByte((byte)mASK01111111);
				}
			}
			else
			{
				this._data.WriteByte((byte)value);
			}
		}

		public void WriteVarUhLong(ulong value)
		{
			this.WriteVarLong((long)value);
		}

		public void WriteVarUhShort(ushort value)
		{
			if ((value > CustomDataWriter.UNSIGNED_SHORT_MAX_VALUE ? true : value < CustomDataWriter.SHORT_MIN_VALUE))
			{
				throw new Exception("Forbidden value");
			}
			int mASK01111111 = 0;
			if ((value < 0 ? true : value > CustomDataWriter.MASK_01111111))
			{
				int cHUNCKBITSIZE = value & 65535;
				while (cHUNCKBITSIZE != 0)
				{
					mASK01111111 = cHUNCKBITSIZE & CustomDataWriter.MASK_01111111;
					cHUNCKBITSIZE = cHUNCKBITSIZE >> (CustomDataWriter.CHUNCK_BIT_SIZE & 31);
					if (cHUNCKBITSIZE > 0)
					{
						mASK01111111 |= CustomDataWriter.MASK_10000000;
					}
					this._data.WriteByte((byte)mASK01111111);
				}
			}
			else
			{
				this._data.WriteByte((byte)value);
			}
		}
	}
}