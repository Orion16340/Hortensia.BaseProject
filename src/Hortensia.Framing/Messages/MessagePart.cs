using Hortensia.Framing.IO;
using System;

namespace Hortensia.Framing
{
    public class MessagePart
	{
		private byte[] m_data;
		private byte[] m_completeData;

		public byte[] CompleteData
		{
			get => m_completeData;
            private set => m_completeData = value;
        }

		public byte[] Data
		{
			get => m_data;
            private set => m_data = value;
        }

		public int? Header
		{
			get;
			private set;
		}

		public bool IsValid
		{
			get
			{
				int num;
				bool flag;
				int num1;

				if (Header.HasValue)
				{
					int? length = Length;
					if (length.HasValue)
					{
						length = Length;
						int length1 = (int)this.Data.Length;
						if (length.GetValueOrDefault() == length1)
						{
							num1 = (length.HasValue ? 1 : 0);
						}
						else
						{
							num1 = 0;
						}
						num = num1;
						flag = num != 0;
						return flag;
					}
				}
				num = 0;
				flag = num != 0;
				return flag;
			}
		}

		public int? Length
		{
			get;
			private set;
		}

		public int? LengthBytesCount
		{
			get
			{
				int? nullable;
				int? nullable1;
				int? header = this.Header;
				if (header.HasValue)
				{
					int? header1 = this.Header;
					if (header1.HasValue)
						nullable1 = new int?(header1.GetValueOrDefault() & 3);

					else
					{
						header = null;
						nullable1 = header;
					}
					nullable = nullable1;
				}
				else
				{
					header = null;
					nullable = header;
				}
				return nullable;
			}
		}

		public int? MessageId
		{
			get
			{
                if (!Header.HasValue)
                    return null;

                return (Header >> 2) & 16383;
            }
		}

		public MessagePart()
		{
		}

        public bool Build(ICustomDataReader reader)
        {
            int? lengthBytesCount;
            long? nullable2;
            int num2;
            int num4;
            long? nullable3;
            if (!this.IsValid)
            {
                this.m_completeData = reader.Data;
                int num = (reader.BytesAvailable < 2L) ? 0 : ((this.Header == null) ? 1 : 0);
                if (num != 0)
                {
                    this.Header = new int?(reader.ReadShort());
                }
                reader.ReadUInt();
                if (this.LengthBytesCount != null)
                {
                    long? nullable1;
                    long bytesAvailable = reader.BytesAvailable;
                    lengthBytesCount = this.LengthBytesCount;
                    if (lengthBytesCount != null)
                    {
                        nullable1 = new long?((long)lengthBytesCount.GetValueOrDefault());
                    }
                    else
                    {
                        nullable3 = null;
                        nullable1 = nullable3;
                    }
                    nullable2 = nullable1;
                    if (((bytesAvailable >= nullable2.GetValueOrDefault()) ? ((nullable2 != null) ? 1 : 0) : 0) != 0)
                    {
                        num2 = (this.Length == null) ? 1 : 0;
                        goto TR_0039;
                    }
                }
                num2 = 0;
            }
            else
            {
                return true;
            }
        TR_0039:
            if (num2 != 0)
            {
                int num8;
                lengthBytesCount = this.LengthBytesCount;
                int num7 = 0;
                if (((lengthBytesCount.GetValueOrDefault() < num7) ? ((lengthBytesCount != null) ? 1 : 0) : 0) != 0)
                {
                    num8 = 1;
                }
                else
                {
                    lengthBytesCount = this.LengthBytesCount;
                    int num9 = 3;
                    num8 = (lengthBytesCount.GetValueOrDefault() > num9) ? ((lengthBytesCount != null) ? 1 : 0) : 0;
                }
                if (num8 != 0)
                {
                    throw new Exception("Malformated Message Header, invalid bytes number to read message length (inferior to 0 or superior to 3)");
                }
                this.Length = 0;
                int num10 = this.LengthBytesCount.Value - 1;
                while (true)
                {
                    int? nullable7;
                    if (num10 < 0)
                    {
                        break;
                    }
                    lengthBytesCount = this.Length;
                    int num11 = reader.ReadByte() << ((num10 * 8) & 0x1f);
                    if (lengthBytesCount != null)
                    {
                        nullable7 = new int?(lengthBytesCount.GetValueOrDefault() | num11);
                    }
                    else
                    {
                        nullable7 = null;
                    }
                    this.Length = nullable7;
                    num10--;
                }
            }
            int num3 = (this.Data != null) ? 0 : ((this.Length != null) ? 1 : 0);
            if (num3 != 0)
            {
                long? nullable5;
                lengthBytesCount = this.Length;
                int num12 = 0;
                if ((lengthBytesCount.GetValueOrDefault() == num12) && (lengthBytesCount != null))
                {
                    this.Data = new byte[0];
                }
                long bytesAvailable = reader.BytesAvailable;
                lengthBytesCount = this.Length;
                if (lengthBytesCount != null)
                {
                    nullable5 = new long?((long)lengthBytesCount.GetValueOrDefault());
                }
                else
                {
                    nullable3 = null;
                    nullable5 = nullable3;
                }
                nullable2 = nullable5;
                if ((bytesAvailable >= nullable2.GetValueOrDefault()) && (nullable2 != null))
                {
                    ICustomDataReader reader2 = reader;
                    this.Data = reader2.ReadBytes(this.Length.Value);
                }
                else
                {
                    long? nullable8;
                    lengthBytesCount = this.Length;
                    if (lengthBytesCount != null)
                    {
                        nullable8 = new long?((long)lengthBytesCount.GetValueOrDefault());
                    }
                    else
                    {
                        nullable3 = null;
                        nullable8 = nullable3;
                    }
                    nullable2 = nullable8;
                    long num16 = reader.BytesAvailable;
                    if ((nullable2.GetValueOrDefault() > num16) && (nullable2 != null))
                    {
                        this.Data = reader.ReadBytes((int)reader.BytesAvailable);
                    }
                }
            }
            if ((this.Data == null) || (this.Length == null))
            {
                num4 = 0;
            }
            else
            {
                lengthBytesCount = this.Length;
                num4 = (this.Data.Length < lengthBytesCount.GetValueOrDefault()) ? ((lengthBytesCount != null) ? 1 : 0) : 0;
            }
            if (num4 != 0)
            {
                long? nullable6;
                int length = 0;
                long num20 = this.Data.Length + reader.BytesAvailable;
                lengthBytesCount = this.Length;
                if (lengthBytesCount != null)
                {
                    nullable6 = new long?((long)lengthBytesCount.GetValueOrDefault());
                }
                else
                {
                    nullable3 = null;
                    nullable6 = nullable3;
                }
                nullable2 = nullable6;
                if ((num20 < nullable2.GetValueOrDefault()) && (nullable2 != null))
                {
                    length = (int)reader.BytesAvailable;
                }
                else
                {
                    long? nullable9;
                    long num22 = this.Data.Length + reader.BytesAvailable;
                    lengthBytesCount = this.Length;
                    if (lengthBytesCount != null)
                    {
                        nullable9 = new long?((long)lengthBytesCount.GetValueOrDefault());
                    }
                    else
                    {
                        nullable3 = null;
                        nullable9 = nullable3;
                    }
                    nullable2 = nullable9;
                    if ((num22 >= nullable2.GetValueOrDefault()) && (nullable2 != null))
                    {
                        length = this.Length.Value - this.Data.Length;
                    }
                }
                if (length != 0)
                {
                    int destinationIndex = this.Data.Length;
                    Array.Resize<byte>(ref this.m_data, this.Data.Length + length);
                    Array.Copy(reader.ReadBytes(length), 0, this.Data, destinationIndex, length);
                }
            }
            return this.IsValid;
        }
    }
}
