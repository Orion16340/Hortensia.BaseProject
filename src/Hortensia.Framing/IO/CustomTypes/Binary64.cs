using System;

namespace Hortensia.Framing.IO.CustomTypes
{
	public class Binary64
	{
		public uint low;

		protected uint internalHigh;

		public Binary64(uint low = 0, uint high = 0)
		{
			this.low = low;
			this.internalHigh = high;
		}

		public void @add(uint n)
		{
			uint num = this.low + n;
			this.internalHigh += (uint)((double)((float)num) / 4294967296);
			this.low = num;
		}

		public void bitwiseNot()
		{
			this.low = ~this.low;
			this.internalHigh = ~this.internalHigh;
		}

		public uint div(uint n)
		{
			uint num = 0;
			num = this.internalHigh % n;
			uint num1 = (this.low % n + num * 6) % n;
			this.internalHigh /= n;
			uint num2 = (uint)(((double)((float)num) * 4294967296 + (double)((float)this.low)) / (double)((float)n));
			this.internalHigh += (uint)((double)((float)num2) / 4294967296);
			this.low = num2;
			return num1;
		}

		public void mul(uint n)
		{
			uint num = this.low * n;
			this.internalHigh *= n;
			this.internalHigh += (uint)((double)((float)num) / 4294967296);
			this.low *= n;
		}
	}
}