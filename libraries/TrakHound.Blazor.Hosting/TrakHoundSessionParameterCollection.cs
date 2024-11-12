// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using TrakHound.Security;

namespace TrakHound.Blazor
{
    public class TrakHoundSessionParameterCollection : ITrakHoundIdentityParameterCollection
    {
        private readonly ISession _httpSession;


        public TrakHoundSessionParameterCollection(ISession httpSession)
        {
            _httpSession = httpSession;
        }


        public async Task<string> GetValue(string key)
        {
            if (_httpSession != null)
            {
                try
                {
                    await _httpSession.LoadAsync(CancellationToken.None);

                    return _httpSession.GetString(key);
                }
                catch { }
            }

            return null;
        }

        public async Task Add(string key, string value)
        {
            if (_httpSession != null && !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                try
                {
                    await _httpSession.LoadAsync(CancellationToken.None);

                    _httpSession.SetString(key, value);
                }
                catch { }
            }
        }

        public async Task Remove(string key)
        {
            if (_httpSession != null && !string.IsNullOrEmpty(key))
            {
                try
                {
                    await _httpSession.LoadAsync(CancellationToken.None);

                    _httpSession.Remove(key);
                }
                catch { }
            }
        }

        public async Task Clear()
        {
            if (_httpSession != null)
            {
                try
                {
                    await _httpSession.LoadAsync(CancellationToken.None);

                    _httpSession.Clear();
                }
                catch { }
            }
        }
    }
}
