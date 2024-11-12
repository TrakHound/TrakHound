// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Blazored.LocalStorage;
using TrakHound.Security;

namespace TrakHound.Blazor
{
    public class TrakHoundPersistentParameterCollection : ITrakHoundIdentityParameterCollection
    {
        private readonly ILocalStorageService _localStorageService;


        public TrakHoundPersistentParameterCollection(ILocalStorageService localStorageService) 
        {
            _localStorageService = localStorageService;
        }


        public async Task<string> GetValue(string key)
        {
            if (key != null)
            {
                try
                {
                    return await _localStorageService.GetItemAsStringAsync(key);
                }
                catch { }
            }

            return null;
        }

        public async Task Add(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                try
                {
                    await _localStorageService.SetItemAsStringAsync(key, value);
                }
                catch { }
            }
        }

        public async Task Remove(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                try
                {
                    await _localStorageService.RemoveItemAsync(key);
                }
                catch { }
            }
        }

        public async Task Clear()
        {
            try
            {
                await _localStorageService.ClearAsync();
            }
            catch { }
        }
    }
}
