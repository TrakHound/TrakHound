// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;

namespace TrakHound
{
    public static class TrakHoundTemp
    {
        private const string _directory = "_temp";

        public static string BaseDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory;


        public static string GetDirectory()
        {
            return Path.Combine(BaseDirectory, _directory);
        }

        public static string CreateDirectory()
        {
            try
            {
                var dir = Path.Combine(GetDirectory(), Guid.NewGuid().ToString());
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                return dir;
            }
            catch { }

            return null;
        }

        public static void Clear()
        {
            var dir = GetDirectory();

            try
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
            }
            catch { }
        }
    }
}
