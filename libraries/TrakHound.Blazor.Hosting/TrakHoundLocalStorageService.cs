// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Blazored.LocalStorage;
using TrakHound.Apps;

namespace TrakHound.Blazor
{
    public class TrakHoundLocalStorageService : ITrakHoundAppLocalStorageService
    {
        private readonly ILocalStorageService _localStorage;


        public TrakHoundLocalStorageService(ILocalStorageService localStorage) 
        {
            _localStorage = localStorage;
        }

        public ValueTask SetItem<TData>(string key, TData data)
        {
            return _localStorage.SetItemAsync(key, data);
        }

        public ValueTask<TData> GetItem<TData>(string key)
        {
            return _localStorage.GetItemAsync<TData>(key);
        }

        public ValueTask RemoveItem(string key)
        {
            return _localStorage.RemoveItemAsync(key);
        }
    }
}
