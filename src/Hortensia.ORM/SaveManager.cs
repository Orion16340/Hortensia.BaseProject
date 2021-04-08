using Hortensia.ORM.Enums;
using Hortensia.ORM.Interfaces;
using Hortensia.ORM.Tables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Hortensia.Core;

namespace Hortensia.ORM
{
    public class SaveManager
    {
        private ConcurrentDictionary<Type, SynchronizedCollection<IRecord>> _newElements = new();
        private ConcurrentDictionary<Type, SynchronizedCollection<IRecord>> _updateElements = new();
        private ConcurrentDictionary<Type, SynchronizedCollection<IRecord>> _removeElements = new();

        public void AddElement(IRecord element)
        {
            var type = element.GetType();

            if (_newElements.ContainsKey(type))
            {
                if (!_newElements[type].Contains(element))
                    _newElements[type].Add(element);
            }
            else
            {
                _newElements.TryAdd(type, new SynchronizedCollection<IRecord> { element });
            }
        }

        public void UpdateElement(IRecord element)
        {
            var type = element.GetType();

            lock (_updateElements)
            {
                if (_newElements.ContainsKey(type) && _newElements[type].Contains(element))
                    return;

                if (_updateElements.ContainsKey(type))
                {
                    if (!_updateElements[type].Contains(element))
                        _updateElements[type].Add(element);
                }
                else
                {
                    _updateElements.TryAdd(type, new SynchronizedCollection<IRecord> { element });
                }
            }
        }

        public void RemoveElement(IRecord element)
        {
            if (element == null)
                return;

            var type = element.GetType();

            if (_newElements.ContainsKey(type) && _newElements[type].Contains(element))
            {
                _newElements[type].Remove(element);
                return;
            }

            if (_updateElements.ContainsKey(type) && _updateElements[type].Contains(element))
                _updateElements[type].Remove(element);

            if (_removeElements.ContainsKey(type))
            {
                if (!_removeElements[type].Contains(element))
                    _removeElements[type].Add(element);
            }
            else
            {
                _removeElements.TryAdd(type, new SynchronizedCollection<IRecord> { element });
            }
        }


        public void Save()
        {
            ServiceLocator.Provider.GetService<BackupManager>().Backup();
            ServiceLocator.Provider.GetService<ILogger>().LogDatabase("Save Started !");

            var types = _removeElements.Keys.ToList();
            foreach (var type in types)
            {
                SynchronizedCollection<IRecord> elements;
                elements = _removeElements[type];

                if (elements.Count > 0)
                {
                    try
                    {
                        ServiceLocator.Provider.GetService<TableManager>().GetWriter(type).Use(elements.ToArray(), DatabaseAction.Remove);
                        _removeElements[type] = new SynchronizedCollection<IRecord>(_removeElements[type].Skip(elements.Count));
                    }
                    catch (Exception e)
                    {
                        ServiceLocator.Provider.GetService<ILogger>().LogError(e.Message);
                    }
                }

                ServiceLocator.Provider.GetService<BackupManager>().Backup();
            }

            types = _newElements.Keys.ToList();
            foreach (var type in types)
            {
                SynchronizedCollection<IRecord> elements;

                elements = _newElements[type];

                if (elements.Count > 0)
                {
                    try
                    {
                        ServiceLocator.Provider.GetService<TableManager>().GetWriter(type).Use(elements.ToArray(), DatabaseAction.Add);
                        _newElements[type] = new SynchronizedCollection<IRecord>(_newElements[type].Skip(elements.Count));
                    }
                    catch (Exception e)
                    {
                        ServiceLocator.Provider.GetService<ILogger>().LogError(e.Message);
                    }
                }
            }

            types = _updateElements.Keys.ToList();
            foreach (var type in types)
            {
                SynchronizedCollection<IRecord> elements;

                elements = _updateElements[type];

                if (elements.Count > 0)
                {
                    try
                    {
                        ServiceLocator.Provider.GetService<TableManager>().GetWriter(type).Use(elements.ToArray(), DatabaseAction.Update);
                        _updateElements[type] = new SynchronizedCollection<IRecord>(_updateElements[type].Skip(elements.Count));
                    }
                    catch (Exception e)
                    {
                        ServiceLocator.Provider.GetService<ILogger>().LogError(e.Message);
                    }
                }
            }

            ServiceLocator.Provider.GetService<ILogger>().LogDatabase("Save Ended !");
        }
    }
}
