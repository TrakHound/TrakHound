// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace TrakHound
{
    public class Assemblies
    {
        private static Assembly[] _assemblies;


        /// <summary>
        /// Load DLL Assembly files located in AppDomain BaseDirectory (used to load "plugin modules")
        /// </summary>
        public static IEnumerable<Assembly> Get()
        {
            if (_assemblies == null)
            {
                try
                {
                    // Load Assemblies located in Base Directoy
                    var assemblyDir = AppDomain.CurrentDomain.BaseDirectory;
                    var dllFiles = Directory.GetFiles(assemblyDir, "*.dll");
                    if (!dllFiles.IsNullOrEmpty())
                    {
                        foreach (var dllFile in dllFiles)
                        {
                            try
                            {
                                // Load Assembly form DLL file
                                var bytes = File.ReadAllBytes(dllFile);

                                // Add as byte[] (this does not lock the DLL file and allows it to be uninstalled)
                                Assembly.Load(bytes);
                            }
                            catch { }
                        }
                    }

                    _assemblies = AppDomain.CurrentDomain.GetAssemblies();
                }
                catch { }
            }

            return _assemblies;
        }

        public static IEnumerable<Assembly> Load(string path = null, bool searchSubDirectories = false)
        {
            try
            {
                var assemblies = new List<Assembly>();

                // Load Assemblies located in Base Directoy (by default)
                var assemblyDir = AppDomain.CurrentDomain.BaseDirectory;
                if (!string.IsNullOrEmpty(path)) assemblyDir = path;

                var option = searchSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                var dllFiles = Directory.GetFiles(assemblyDir, "*.dll", option);
                if (!dllFiles.IsNullOrEmpty())
                {
                    var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

                    foreach (var dllFile in dllFiles)
                    {
                        try
                        {
                            var assemblyFile = Assembly.LoadFile(dllFile);
                            if (assemblyFile != null)
                            {
                                var existingAssembly = loadedAssemblies.FirstOrDefault(o => o.FullName == assemblyFile.FullName && o.GetName().Version == assemblyFile.GetName().Version);
                                if (existingAssembly != null)
                                {
                                    assemblies.Add(existingAssembly);
                                }
                                else
                                {
                                    var assembly = Assembly.LoadFrom(dllFile);
                                    if (assembly != null) assemblies.Add(assembly);
                                }
                            }
                        }
                        catch { }
                    }
                }

                return assemblies;
            }
            catch { }

            return null;
        }

        public static IEnumerable<Type> GetTypes<T>(string path = null, bool searchSubDirectories = false)
        {
            var types = new List<Type>();

            var assemblies = Load(path, searchSubDirectories);
            if (!assemblies.IsNullOrEmpty())
            {
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var assemblyTypes = assembly.GetTypes();
                        var foundTypes = assemblyTypes.Where(x => typeof(T).IsAssignableFrom(x) && !x.IsGenericType && !x.IsInterface && !x.IsAbstract);
                        if (!foundTypes.IsNullOrEmpty())
                        {
                            foreach (var foundType in foundTypes)
                            {
                                types.Add(foundType);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return types;
        }

        public static Type GetType<T>(string path = null, bool searchSubDirectories = false)
        {
            var assemblies = Load(path, searchSubDirectories);
            if (!assemblies.IsNullOrEmpty())
            {
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var assemblyTypes = assembly.GetTypes();
                        var foundType = assemblyTypes.FirstOrDefault(x => typeof(T).IsAssignableFrom(x) && !x.IsGenericType && !x.IsInterface && !x.IsAbstract);
                        if (foundType != null)
                        {
                            return foundType;
                        }
                    }
                    catch { }
                }
            }

            return null;
        }

        public static Type GetTypeFromFile<T>(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    if (File.Exists(path))
                    {
                        var assembly = Assembly.LoadFrom(path);
                        if (assembly != null)
                        {
                            var assemblyTypes = assembly.GetTypes();
                            var foundType = assemblyTypes.FirstOrDefault(x => typeof(T).IsAssignableFrom(x) && !x.IsGenericType && !x.IsInterface && !x.IsAbstract);
                            if (foundType != null)
                            {
                                return foundType;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static IEnumerable<Type> GetTypesFromFile<T>(AssemblyLoadContext context, string path)
        {
            var types = new List<Type>();

            if (context != null && !string.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                {
                    var assembly = context.LoadFromAssemblyPath(path);
                    if (assembly != null)
                    {
                        // Load Dependencies
                        var dir = Path.GetDirectoryName(path);

                        var resolver = new AssemblyDependencyResolver(dir);
                        resolver.ResolveAssemblyToPath(assembly.GetName());

                        var assemblyTypes = assembly.GetTypes();

                        var foundTypes = assemblyTypes.Where(x => typeof(T).IsAssignableFrom(x) && !x.IsGenericType && !x.IsInterface && !x.IsAbstract);
                        if (!foundTypes.IsNullOrEmpty())
                        {
                            types.AddRange(foundTypes);
                        }
                    }
                }
            }

            return types;
        }
    }
}
