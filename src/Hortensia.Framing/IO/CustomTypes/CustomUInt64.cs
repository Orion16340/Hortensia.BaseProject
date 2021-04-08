using System;

namespace Hortensia.Framing.IO.CustomTypes
{
	public class CustomUInt64 : Binary64
	{
		public uint high
		{
			get
			{
				return this.internalHigh;
			}
			set
			{
				this.internalHigh = value;
			}
		}

		public CustomUInt64() : base(0, 0)
		{
		}

		public CustomUInt64(uint low = 0, uint high = 0) : base(low, high)
		{
		}

		public static CustomUInt64 fromNumber(ulong n)
		{
			CustomUInt64 customUInt64 = new CustomUInt64((uint)n, (uint)Math.Floor((double)((float)n) / 4294967296));
			return customUInt64;
		}

		public ulong toNumber()
		{
			ulong num = (ulong)((double)((float)this.high) * 4294967296 + (double)((float)this.low));
			return num;
		}
	}
}