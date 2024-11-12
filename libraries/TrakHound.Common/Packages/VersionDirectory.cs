// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Packages
{
    struct VersionDirectory
    {
        public Version Version { get; set; }

        public string Path { get; set; }


        public VersionDirectory(Version version, string path)
        {
            Version = version;
            Path = path;
        }
    }
}
