// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using TrakHound.Packages;

namespace TrakHound.Modules
{
    public class TrakHoundModuleProvider : ITrakHoundModuleProvider
    {
        private readonly TrakHoundPackageManager _packageManager;
        private readonly Dictionary<string, ITrakHoundModuleManager> _moduleManagers = new Dictionary<string, ITrakHoundModuleManager>();
        private object _lock = new object();


        public TrakHoundModuleProvider(TrakHoundPackageManager packageManager)
        {
            _packageManager = packageManager;
        }


        public ITrakHoundModuleManager Get<TModule>(string packageCategory, TrakHoundModuleContext context = null, bool loadByDependencies = true)
        {
            if (!string.IsNullOrEmpty(packageCategory))
            {
                var packageCategories = new string[] { packageCategory };
                return Get<TModule>(packageCategories, context, loadByDependencies);
            }

            return null;
        }

        public ITrakHoundModuleManager Get<TModule>(IEnumerable<string> packageCategories, TrakHoundModuleContext context = null, bool loadByDependencies = true)
        {
            if (!packageCategories.IsNullOrEmpty())
            {
                var key = CreateKey<TModule>(packageCategories);
                if (key != null)
                {
                    ITrakHoundModuleManager moduleManager;
                    lock (_lock) moduleManager = _moduleManagers.GetValueOrDefault(key);

                    if (moduleManager == null)
                    {
                        moduleManager = new TrakHoundModuleManager<TModule>(_packageManager, packageCategories, context: context, loadByDependencies: loadByDependencies);

                        lock (_lock)
                        {
                            _moduleManagers.Remove(key);
                            _moduleManagers.Add(key, moduleManager);
                        }
                    }

                    return moduleManager;
                }
            }

            return null;
        }

        private string CreateKey<TModule>(IEnumerable<string> packageCategories)
        {
            if (!packageCategories.IsNullOrEmpty())
            {
                var moduleType = typeof(TModule);
                var moduleKey = $"{moduleType.Assembly.Location}:{moduleType.AssemblyQualifiedName}";

                var keys = new List<string>();
                keys.Add(moduleKey);
                keys.AddRange(packageCategories.OrderBy(o => o));
                return string.Join(":", keys).ToMD5Hash();
            }

            return null;
        }
    }
}
