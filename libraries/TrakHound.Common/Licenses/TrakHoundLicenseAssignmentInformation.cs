// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Licenses
{
    public class TrakHoundLicenseAssignmentInformation
    {
        public string AssignmentId { get; set; }

        public string ProductId { get; set; }

        public string DeviceId { get; set; }

        public string LicenseCode { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Assigned { get; set; }
    }
}
