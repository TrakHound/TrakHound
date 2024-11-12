// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Instance.Services
{
    public class AdminTokenService
    {
        private readonly Dictionary<string, AdminAuthenticationToken> _tokens = new Dictionary<string, AdminAuthenticationToken>();
        private readonly object _lock = new object();


        public AdminAuthenticationToken Validate(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                lock (_lock)
                {
                    return _tokens.GetValueOrDefault(token);
                }
            }

            return null;
        }

        public void Add(AdminAuthenticationToken token)
        {
            if (token != null && !string.IsNullOrEmpty(token.Token))
            {
                lock (_lock)
                {
                    _tokens.Remove(token.Token);
                    _tokens.Add(token.Token, token);
                }
            }
        }

        public void Remove(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                lock (_lock)
                {
                    _tokens.Remove(token);
                }
            }
        }
    }
}
