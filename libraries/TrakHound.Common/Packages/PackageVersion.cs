// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Packages
{
    struct PackageVersion
    {
        public Version Version { get; set; }

        public string VersionValue { get; set; }

        public string PackageId { get; set; }


        public PackageVersion(Version version, string packageId, string packageVersion)
        {
            Version = version;
            PackageId = packageId;
            VersionValue = packageVersion;
        }
    }
}
