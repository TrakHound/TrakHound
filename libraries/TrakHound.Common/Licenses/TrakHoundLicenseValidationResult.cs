// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Licenses
{
    public struct TrakHoundLicenseValidationResult
    {
        public bool IsValid { get; set; }

        public string Message { get; set; }


        public string ProductId { get; set; }

        public string DeviceId { get; set; }

        public DateTime ExpirationDate { get; set; }

        public bool IsPermanent { get; set; }
    }
}
