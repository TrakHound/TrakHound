// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Reflection;
using System.Runtime.Loader;

namespace TrakHound.Modules
{
    public class TrakHoundModuleLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;


        public TrakHoundModuleLoadContext(string contextId, string modulePath, bool isCollectible = false) : base(contextId, isCollectible)
        {
            _resolver = new AssemblyDependencyResolver(modulePath);
        }


        protected override Assembly Load(AssemblyName assemblyName)
        {
            string referencedAssemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (referencedAssemblyPath != null)
            {
                var assembly = LoadFromAssemblyPath(referencedAssemblyPath);
                if (assembly != null)
                {
                    return assembly;
                }
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
