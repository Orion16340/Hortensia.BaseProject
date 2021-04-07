using System;

namespace Hortensia.ORM.Attributes
{
    public class TableAttribute : Attribute
    {
        public string TableName;
        public bool Load;
        public short ReadingOrder;

        public TableAttribute(string tableName, short readingOrder = -1, bool load = true)
        {
            this.TableName = tableName;
            this.Load = load;
            this.ReadingOrder = readingOrder;
        }
    }
}
