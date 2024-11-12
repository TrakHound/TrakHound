// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Licenses
{
    public class TrakHoundLicenseInformation
    {
        public string LicenseCode { get; set; }

        public string ProductId { get; set; }

        public string DeviceId { get; set; }

        public int AssignmentCount { get; set; }

        public int AssignmentLimit { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
