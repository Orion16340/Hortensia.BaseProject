using System;

namespace Hortensia.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ContainerAttribute : Attribute
    {
        public ContainerAttribute() { }
    }
}
