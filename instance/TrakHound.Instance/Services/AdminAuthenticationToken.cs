// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Instance.Services
{
    public class AdminAuthenticationToken
    {
        public string Token { get; set; }

        public string Username { get; set; }

        public long Timestamp { get; set; }
    }
}
