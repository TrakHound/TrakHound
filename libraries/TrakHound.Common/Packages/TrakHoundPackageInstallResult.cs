// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Packages
{
    public struct TrakHoundPackageInstallResult
    {
        public bool Success { get; set; }

        public string PackageId { get; set; }

        public string PackageVersion { get; set; }

        public string Message { get; set; }
    }
}
