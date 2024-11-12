// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;

namespace TrakHound.Volumes
{
    public class TrakHoundVolumeProvider : ITrakHoundVolumeProvider
    {
        private const string _directoryName = "volumes";

        private readonly string _basePath;


        public TrakHoundVolumeProvider(string basePath = null)
        {
            _basePath = !string.IsNullOrEmpty(basePath) ? basePath : AppContext.BaseDirectory;
        }


        public ITrakHoundVolume GetVolume(string id)
        {
            if (!string.IsNullOrEmpty(_basePath) && !string.IsNullOrEmpty(id))
            {
                try
                {
                    var path = Path.Combine(_basePath, _directoryName, id);
                    if (!Directory.Exists(_basePath)) Directory.CreateDirectory(path);

                    return new TrakHoundVolume(id, path);
                }
                catch { }
            }

            return null;
        }

        public bool DeleteVolume(string id)
        {
            if (!string.IsNullOrEmpty(_basePath) && !string.IsNullOrEmpty(id))
            {
                try
                {
                    var path = Path.Combine(_basePath, _directoryName, id);
                    if (Directory.Exists(_basePath)) Directory.Delete(path, true);

                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}
