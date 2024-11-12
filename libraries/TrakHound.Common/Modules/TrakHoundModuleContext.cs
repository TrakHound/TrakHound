// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Modules
{
    public class TrakHoundModuleContext
    {
        private readonly Dictionary<string, TrakHoundModuleLoadContext> _contexts = new Dictionary<string, TrakHoundModuleLoadContext>();
        private readonly object _lock = new object();
        private readonly string _contextId;


        public string Id => _contextId;


        public TrakHoundModuleContext(string contextId = null)
        {
            _contextId = !string.IsNullOrEmpty(contextId) ? contextId : Guid.NewGuid().ToString();
        }


        public TrakHoundModuleLoadContext AddLoadContext(string loadContextId, string assemblyPath)
        {
            if (!string.IsNullOrEmpty(loadContextId) && !string.IsNullOrEmpty(assemblyPath))
            {
                lock (_lock)
                {
                    if (!_contexts.ContainsKey(loadContextId))
                    {
                        //Console.WriteLine($"{loadContextId} => No Cache Found");

                        var context = new TrakHoundModuleLoadContext(loadContextId, assemblyPath, true);

                        //_contexts.Remove(loadContextId);
                        _contexts.Add(loadContextId, context);

                        return context;
                    }
                    else
                    {
                        //Console.WriteLine($"{loadContextId} => Cache Found");

                        return _contexts.GetValueOrDefault(loadContextId);
                    }
                }
            }

            return null;
        }

        public TrakHoundModuleLoadContext GetLoadContext(string loadContextId)
        {
            if (!string.IsNullOrEmpty(loadContextId))
            {
                lock (_lock)
                {
                    if (_contexts.ContainsKey(loadContextId))
                    {
                        return _contexts.GetValueOrDefault(loadContextId);
                    }
                }
            }

            return null;
        }

        public void RemoveLoadContext(string loadContextId)
        {
            if (!string.IsNullOrEmpty(loadContextId))
            {
                lock (_lock)
                {
                    if (_contexts.ContainsKey(loadContextId))
                    {
                        _contexts.Remove(loadContextId);
                    }
                }
            }
        }
    }
}
