// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundBoolean> GetBoolean(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundBoolean>> GetBooleans(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundBoolean>> GetBooleans(IEnumerable<string> objectPaths, string routerId = null);

        Task<bool> GetBooleanValue(string objectPath, string routerId = null);

        Task<Dictionary<string, bool>> GetBooleanValues(string objectPath, string routerId = null);

        Task<Dictionary<string, bool>> GetBooleanValues(IEnumerable<string> objectPaths, string routerId = null);


        Task<bool> PublishBoolean(string objectPath, bool value, bool async = false, string routerId = null);

        Task<bool> PublishBooleans(IEnumerable<TrakHoundBooleanEntry> entries, bool async = false, string routerId = null);
    }
}
