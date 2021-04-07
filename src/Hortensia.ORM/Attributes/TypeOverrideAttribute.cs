using System;

namespace Hortensia.ORM.Attributes
{
    public class TypeOverrideAttribute : Attribute
    {
        public string NewType
        {
            get;
            set;
        }
        public TypeOverrideAttribute(string newType)
        {
            this.NewType = newType;
        }
    }
}
