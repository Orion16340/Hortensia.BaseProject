using Hortensia.Framing.IO.CustomTypes;
using Hortensia.Framing.IO.Interfaces;
using System;
using System.IO;

namespace Hortensia.Framing.IO
{
	public class CustomDataReader : ICustomDataReader, IDataReader, IDisposable
	{
		private static int INT_SIZE;

		private static int SHORT_SIZE;

		private static int SHORT_MAX_VALUE;

		private static int UNSIGNED_SHORT_MAX_VALUE;

		private static int CHUNCK_BIT_SIZE;

		private static int MAX_ENCODING_LENGTH;

		private static int MASK_10000000;

		private static int MASK_01111111;

		private IDataReader _data;

		public long BytesAvailable
		{
			get
			{
				return this._data.BytesAvailable;
			}
		}

		public byte[] Data
		{
			get
			{
				return this._data.Data;
			}
		}

		public long Position
		{
			get
			{
				return this._data.Position;
			}
		}

		static CustomDataReader()
		{
			CustomDataReader.INT_SIZE = 32;
			CustomDataReader.SHORT_SIZE = 16;
			CustomDataReader.SHORT_MAX_VALUE = 32767;
			CustomDataReader.UNSIGNED_SHORT_MAX_VALUE = 65536;
			CustomDataReader.CHUNCK_BIT_SIZE = 7;
			CustomDataReader.MAX_ENCODING_LENGTH = (int)Math.Ceiling((double)CustomDataReader.INT_SIZE / (double)CustomDataReader.CHUNCK_BIT_SIZE);
			CustomDataReader.MASK_10000000 = 128;
			CustomDataReader.MASK_01111111 = 127;
		}

		public CustomDataReader(IDataReader reader)
		{
			this._data = reader;
		}

		public CustomDataReader(byte[] data)
		{
			this._data = new BigEndianReader(data);
		}

		public void Dispose()
		{
			this._data.Dispose();
		}

		public bool ReadBoolean()
		{
			return this._data.ReadBoolean();
		}

		public byte ReadByte()
		{
			return this._data.ReadByte();
		}

		public byte[] ReadBytes(int n)
		{
			return this._data.ReadBytes(n);
		}

		public char ReadChar()
		{
			return this._data.ReadChar();
		}

		public double ReadDouble()
		{
			return this._data.ReadDouble();
		}

		public float ReadFloat()
		{
			return this._data.ReadFloat();
		}

		public int ReadInt()
		{
			return this._data.ReadInt();
		}

		private static CustomInt64 readInt64(IDataReader input)
		{
			CustomInt64 customInt64;
			uint num = 0;
			CustomInt64 customInt641 = new CustomInt64();
			int num1 = 0;
			while (true)
			{
				num = input.ReadByte();
				if (num1 == 28)
				{
					if (num < 128)
					{
						customInt641.low = customInt641.low | num << (num1 & 31);
						customInt641.high = num >> 4;
						customInt64 = customInt641;
						break;
					}
					else
					{
						num &= 127;
						customInt641.low = customInt641.low | num << (num1 & 31);
						customInt641.high = num >> 4;
						num1 = 3;
						while (true)
						{
							num = input.ReadByte();
							if (num1 < 32)
							{
								if (num < 128)
								{
									break;
								}
								customInt641.high = customInt641.high | (num & 127) << (num1 & 31);
							}
							num1 += 7;
						}
						customInt641.high = customInt641.high | num << (num1 & 31);
						customInt64 = customInt641;
						break;
					}
				}
				else if (num < 128)
				{
					customInt641.low = customInt641.low | num << (num1 & 31);
					customInt64 = customInt641;
					break;
				}
				else
				{
					customInt641.low = customInt641.low | (num & 127) << (num1 & 31);
					num1 += 7;
				}
			}
			return customInt64;
		}

		public long ReadLong()
		{
			return this._data.ReadLong();
		}

		public sbyte ReadSByte()
		{
			return this._data.ReadSByte();
		}

		public short ReadShort()
		{
			return this._data.ReadShort();
		}

		public uint ReadUInt()
		{
			return this._data.ReadUInt();
		}

		private CustomUInt64 readUInt64(IDataReader input)
		{
			CustomUInt64 customUInt64;
			uint num = 0;
			CustomUInt64 customUInt641 = new CustomUInt64();
			int num1 = 0;
			while (true)
			{
				num = input.ReadByte();
				if (num1 == 28)
				{
					if (num < 128)
					{
						customUInt641.low = customUInt641.low | num << (num1 & 31);
						customUInt641.high = num >> 4;
						customUInt64 = customUInt641;
						break;
					}
					else
					{
						num &= 127;
						customUInt641.low = customUInt641.low | num << (num1 & 31);
						customUInt641.high = num >> 4;
						num1 = 3;
						while (true)
						{
							num = input.ReadByte();
							if (num1 < 32)
							{
								if (num < 128)
								{
									break;
								}
								customUInt641.high = customUInt641.high | (num & 127) << (num1 & 31);
							}
							num1 += 7;
						}
						customUInt641.high = customUInt641.high | num << (num1 & 31);
						customUInt64 = customUInt641;
						break;
					}
				}
				else if (num < 128)
				{
					customUInt641.low = customUInt641.low | num << (num1 & 31);
					customUInt64 = customUInt641;
					break;
				}
				else
				{
					customUInt641.low = customUInt641.low | (num & 127) << (num1 & 31);
					num1 += 7;
				}
			}
			return customUInt64;
		}

		public ulong ReadULong()
		{
			return this._data.ReadULong();
		}

		public ushort ReadUShort()
		{
			return this._data.ReadUShort();
		}

		public string ReadUTF()
		{
			return this._data.ReadUTF();
		}

		public string ReadUTFBytes(ushort len)
		{
			return this._data.ReadUTFBytes(len);
		}

		public int ReadVarInt()
		{
			int num = 0;
			int num1 = 0;
			int cHUNCKBITSIZE = 0;
			bool mASK10000000 = false;
			while (cHUNCKBITSIZE < CustomDataReader.INT_SIZE)
			{
				num = this._data.ReadByte();
				mASK10000000 = (num & CustomDataReader.MASK_10000000) == CustomDataReader.MASK_10000000;
				num1 = (cHUNCKBITSIZE <= 0 ? num1 + (num & CustomDataReader.MASK_01111111) : num1 + ((num & CustomDataReader.MASK_01111111) << (cHUNCKBITSIZE & 31)));
				cHUNCKBITSIZE += CustomDataReader.CHUNCK_BIT_SIZE;
				if (!mASK10000000)
				{
					return num1;
				}
			}
			throw new Exception("Too much data");
		}

		public long ReadVarLong()
		{
			return CustomDataReader.readInt64(this._data).toNumber();
		}

		public short ReadVarShort()
		{
			int num = 0;
			short uNSIGNEDSHORTMAXVALUE = 0;
			int cHUNCKBITSIZE = 0;
			bool mASK10000000 = false;
			while (cHUNCKBITSIZE < CustomDataReader.SHORT_SIZE)
			{
				num = this._data.ReadByte();
				mASK10000000 = (num & CustomDataReader.MASK_10000000) == CustomDataReader.MASK_10000000;
				uNSIGNEDSHORTMAXVALUE = (cHUNCKBITSIZE <= 0 ? (short)(uNSIGNEDSHORTMAXVALUE + (num & CustomDataReader.MASK_01111111)) : (short)(uNSIGNEDSHORTMAXVALUE + ((num & CustomDataReader.MASK_01111111) << (cHUNCKBITSIZE & 31))));
				cHUNCKBITSIZE += CustomDataReader.CHUNCK_BIT_SIZE;
				if (!mASK10000000)
				{
					if (uNSIGNEDSHORTMAXVALUE > CustomDataReader.SHORT_MAX_VALUE)
					{
						uNSIGNEDSHORTMAXVALUE = (short)(uNSIGNEDSHORTMAXVALUE - CustomDataReader.UNSIGNED_SHORT_MAX_VALUE);
					}
					return uNSIGNEDSHORTMAXVALUE;
				}
			}
			throw new Exception("Too much data");
		}

		public uint ReadVarUhInt()
		{
			int num = 0;
			uint num1 = 0;
			int cHUNCKBITSIZE = 0;
			bool mASK10000000 = false;
			while (cHUNCKBITSIZE < CustomDataReader.INT_SIZE)
			{
				num = this._data.ReadByte();
				mASK10000000 = (num & CustomDataReader.MASK_10000000) == CustomDataReader.MASK_10000000;
				num1 = (cHUNCKBITSIZE <= 0 ? (uint)((ulong)num1 + (ulong)(num & MASK_01111111)) : (uint)((int)num1 + ((num & MASK_01111111) << (int)(cHUNCKBITSIZE & 31))));
				cHUNCKBITSIZE += CustomDataReader.CHUNCK_BIT_SIZE;
				if (!mASK10000000)
				{
					return num1;
				}
			}
			throw new Exception("Too much data");
		}

		public ulong ReadVarUhLong()
		{
			return this.readUInt64(this._data).toNumber();
		}

		public ushort ReadVarUhShort()
		{
			int num = 0;
			ushort uNSIGNEDSHORTMAXVALUE = 0;
			int cHUNCKBITSIZE = 0;
			bool mASK10000000 = false;
			while (cHUNCKBITSIZE < CustomDataReader.SHORT_SIZE)
			{
				num = this._data.ReadByte();
				mASK10000000 = (num & CustomDataReader.MASK_10000000) == CustomDataReader.MASK_10000000;
				uNSIGNEDSHORTMAXVALUE = (cHUNCKBITSIZE <= 0 ? (ushort)(uNSIGNEDSHORTMAXVALUE + (num & CustomDataReader.MASK_01111111)) : (ushort)(uNSIGNEDSHORTMAXVALUE + ((num & CustomDataReader.MASK_01111111) << (cHUNCKBITSIZE & 31))));
				cHUNCKBITSIZE += CustomDataReader.CHUNCK_BIT_SIZE;
				if (!mASK10000000)
				{
					if (uNSIGNEDSHORTMAXVALUE > CustomDataReader.SHORT_MAX_VALUE)
					{
						uNSIGNEDSHORTMAXVALUE = (ushort)(uNSIGNEDSHORTMAXVALUE - CustomDataReader.UNSIGNED_SHORT_MAX_VALUE);
					}
					return uNSIGNEDSHORTMAXVALUE;
				}
			}
			throw new Exception("Too much data");
		}

		public void Seek(int offset, SeekOrigin seekOrigin)
		{
			this._data.Seek(offset, seekOrigin);
		}

		public void SkipBytes(int n)
		{
			this._data.SkipBytes(n);
		}
	}
}