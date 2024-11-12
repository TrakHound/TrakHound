// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TrakHound.Volumes
{
    public class TrakHoundVolume : ITrakHoundVolume
    {
        private readonly string _id;
        private readonly string _basePath;
        private readonly List<ITrakHoundVolumeListener> _listeners = new List<ITrakHoundVolumeListener>();
        private readonly object _lock = new object();


        public string Id => _id;


        public TrakHoundVolume(string id, string basePath)
        {
            _id = id;
            _basePath = basePath;
        }

        public void Dispose()
        {
            lock (_lock)
            {
                foreach (var listener in _listeners) listener.Dispose();
                _listeners.Clear();
            }
        }


        public async Task<string[]> ListFiles(string path = null, bool includeSubdirectories = false)
        {
            try
            {
                var fullPath = !string.IsNullOrEmpty(path) && path != "/" && path != "\\" ? Path.Combine(_basePath, path) : _basePath;
                if (Directory.Exists(fullPath))
                {
                    var filePaths = Directory.GetFiles(fullPath);
                    if (!filePaths.IsNullOrEmpty())
                    {
                        var relativePaths = new List<string>();

                        foreach (var filePath in filePaths)
                        {
                            relativePaths.Add(Path.GetRelativePath(_basePath, filePath));
                        }

                        return relativePaths.ToArray();
                    }
                }
            }
            catch { }

            return null;
        }


        public async Task<string> ReadString(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var fullPath = Path.Combine(_basePath, path);
                    if (File.Exists(fullPath))
                    {
                        return await File.ReadAllTextAsync(fullPath);
                    }
                }
                catch { }
            }

            return null;
        }

        public async Task<T> ReadJson<T>(string path)
        {
            var json = await ReadString(path);
            return Json.Convert<T>(json);
        }

        public async Task<byte[]> ReadBytes(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var fullPath = Path.Combine(_basePath, path);
                    if (File.Exists(fullPath))
                    {
                        return await File.ReadAllBytesAsync(fullPath);
                    }
                }
                catch { }
            }

            return null;
        }


        public async Task<bool> WriteString(string path, string content)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(content))
            {
                try
                {
                    var fullPath = Path.Combine(_basePath, path);

                    var directory = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    await File.WriteAllTextAsync(fullPath, content);
                    return true;
                }
                catch { }
            }

            return false;
        }

        public async Task<bool> WriteJson(string path, object content)
        {
            if (!string.IsNullOrEmpty(path) && content != null)
            {
                var json = Json.Convert(content, indented: true);
                return await WriteString(path, json);
            }

            return false;
        }

        public async Task<bool> WriteBytes(string path, byte[] content)
        {
            if (!string.IsNullOrEmpty(path) && !content.IsNullOrEmpty())
            {
                try
                {
                    var fullPath = Path.Combine(_basePath, path);

                    var directory = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    await File.WriteAllBytesAsync(fullPath, content);
                    return true;
                }
                catch { }
            }

            return false;
        }


        public async Task<bool> Delete(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var fullPath = Path.Combine(_basePath, path);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        return true;
                    }
                }
                catch { }
            }

            return false;
        }


        public ITrakHoundVolumeListener CreateListener(string path, string filter = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var listener = new TrakHoundVolumeListener(_basePath, path, filter);
                lock (_lock) _listeners.Add(listener);
                return listener;
            }

            return null;
        }
    }
}
