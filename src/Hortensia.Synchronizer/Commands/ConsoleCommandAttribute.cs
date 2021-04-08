using System;

namespace Hortensia.Synchronizer.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
	public class ConsoleCommandAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public ConsoleCommandAttribute(string name)
		{
			this.Name = name;
		}
	}
}
