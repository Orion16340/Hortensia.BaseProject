using Hortensia.ORM.Interfaces;
using Hortensia.ORM.Tables.IO;
using System;
using System.Collections.Generic;

namespace Hortensia.ORM.Tables
{
    public class TableManager
    {
        private readonly Dictionary<Type, TableDefinitions> _TableDefinitions = new();

        private readonly Dictionary<Type, DatabaseWriter> _writers = new();

        public void Initialize(Type[] tableTypes)
        {
            foreach (var type in tableTypes)
            {
                _TableDefinitions.Add(type, new TableDefinitions(type));
                _writers.Add(type, new DatabaseWriter(type));
            }

        }
        public void RemoveFromContainer(IRecord element)
        {
            var tableDefinition = _TableDefinitions[element.GetType()];

            if (tableDefinition.Load)
                tableDefinition.ContainerValue.Remove(element.Id);
        }

        public void AddToContainer(IRecord element)
        {
            var tableDefinition = _TableDefinitions[element.GetType()];

            if (tableDefinition.Load)
                tableDefinition.ContainerValue.Add(element);
        }
        public DatabaseWriter GetWriter(Type type)
            => _writers[type];

        public TableDefinitions GetDefinition(Type type)
            => _TableDefinitions[type];
    }
}
