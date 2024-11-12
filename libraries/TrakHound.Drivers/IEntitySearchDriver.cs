// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver specifically for a Keyword Search for TrakHound Entities with the specified Query.
    /// </summary>
    public interface IEntitySearchDriver : ITrakHoundDriver 
    {
        Task<TrakHoundResponse<string>> Search(string query, long skip = 0, long take = 0);
    }
}
