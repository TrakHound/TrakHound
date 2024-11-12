// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Http
{
    public class TrakHoundHttpAuthenticationCredentials
    {
        public string Login { get; set; }
        public string Hash { get; set; } // SHA-256 Hashed Password


        public TrakHoundHttpAuthenticationCredentials() { }

        public TrakHoundHttpAuthenticationCredentials(string login, string hash)
        {
            Login = login;
            Hash = hash;
        }
    }
}
