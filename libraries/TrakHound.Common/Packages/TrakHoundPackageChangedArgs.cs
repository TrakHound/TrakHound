// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Packages
{
    public struct TrakHoundPackageChangedArgs
    {
        public string PackageCategory { get; set; }

        public string PackageId { get; set; }

        public string PackageVersion { get; set; }


        public TrakHoundPackageChangedArgs(string packageCategory, string packageId, string packageVersion)
        {
            PackageCategory = packageCategory;
            PackageId = packageId;
            PackageVersion = packageVersion;
        }
    }
}
