// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using TrakHound.Security;

namespace TrakHound.Blazor
{
    public class TrakHoundRequestParameterCollection : ITrakHoundIdentityParameterCollection
    {
        private readonly HttpRequest _httpRequest;
        private readonly HttpResponse _httpResponse;


        public TrakHoundRequestParameterCollection(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            _httpRequest = httpRequest;
            _httpResponse = httpResponse;
        }


        public async Task<string> GetValue(string key)
        {
            // Get Header Parameters
            if (!_httpRequest.Headers.IsNullOrEmpty())
            {
                var values = _httpRequest.Headers[key];
                if (!values.IsNullOrEmpty())
                {
                    return values.FirstOrDefault();
                }
            }

            // Get Query Parameters
            if (!_httpRequest.Query.IsNullOrEmpty())
            {
                var values = _httpRequest.Query[key];
                if (!values.IsNullOrEmpty())
                {
                    return values.FirstOrDefault();
                }
            }

            return null;
        }

        public async Task Add(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                _httpResponse.Headers.Append(key, value);
            }
        }

        // Not supported for Request
        public Task Remove(string key) => Task.CompletedTask;

        // Not supported for Request
        public Task Clear() => Task.CompletedTask;
    }
}
