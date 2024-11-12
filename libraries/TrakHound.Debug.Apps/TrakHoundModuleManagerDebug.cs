// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Modules
{
    public class TrakHoundModuleManagerDebug<TModule> : ITrakHoundModuleManager
    {
        public IEnumerable<string> PackageCategories { get; }

        public Func<string, string> FilterFunction { get; }

        public IEnumerable<ITrakHoundModule> Modules { get; }

        public IEnumerable<Type> ModuleTypes { get; }

        public EventHandler<ITrakHoundModule> ModuleAdded { get; set; }

        public EventHandler<ITrakHoundModule> ModuleRemoved { get; set; }

        public EventHandler<Exception> ModuleLoadError { get; set; }


        public TrakHoundModuleManagerDebug() { }


        public ITrakHoundModule Get(string packageId, string packageVersion) => null;

        public IEnumerable<Type> GetTypes(string packageId, string packageVersion) => null;
    }
}
