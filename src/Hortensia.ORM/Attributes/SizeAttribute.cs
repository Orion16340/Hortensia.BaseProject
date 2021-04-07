using System;

namespace Hortensia.ORM.Attributes
{
    public class SizeAttribute : Attribute
    {
        public int Value { get; set; }

        public SizeAttribute(int value)
            => Value = value;
    }
}
