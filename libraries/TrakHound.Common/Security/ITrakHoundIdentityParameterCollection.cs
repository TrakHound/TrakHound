// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;

namespace TrakHound.Security
{
    public interface ITrakHoundIdentityParameterCollection
    {
        Task<string> GetValue(string key);

        Task Add(string key, string value);

        Task Remove(string key);

        Task Clear();
    }
}
