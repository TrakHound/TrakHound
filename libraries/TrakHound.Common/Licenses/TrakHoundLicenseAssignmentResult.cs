// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Licenses
{
    public struct TrakHoundLicenseAssignmentResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }


        public string ProductId { get; set; }

        public string DeviceId { get; set; }

        public string AssignmentId { get; set; }
    }
}
