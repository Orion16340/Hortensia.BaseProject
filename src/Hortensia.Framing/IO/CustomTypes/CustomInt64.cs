using System;

namespace Hortensia.Framing.IO.CustomTypes
{
	public class CustomInt64 : Binary64
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

		public CustomInt64() : base(0, 0)
		{
		}

		public CustomInt64(uint low = 0, uint high = 0) : base(low, high)
		{
		}

		public static CustomInt64 fromNumber(long n)
		{
			CustomInt64 customInt64 = new CustomInt64((uint)n, (uint)Math.Floor((double)n / 4294967296));
			return customInt64;
		}

		public long toNumber()
		{
			long num = (long)((double)((float)this.high) * 4294967296 + (double)((float)this.low));
			return num;
		}
	}
}