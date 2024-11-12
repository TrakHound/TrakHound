// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<IEnumerable<TrakHoundSet>> GetSets(string objectPath, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<TrakHoundSet>> GetSets(IEnumerable<string> objectPaths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<Dictionary<string, IEnumerable<string>>> GetSetValues(string objectPath, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<Dictionary<string, IEnumerable<string>>> GetSetValues(IEnumerable<string> objectPaths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundSet>>> SubscribeSets(string objectPath, string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<TrakHoundSet>>> SubscribeSets(IEnumerable<string> objectPaths, string routerId = null);


        Task<bool> PublishSet(string objectPath, string value, bool async = false, string routerId = null);

        Task<bool> PublishSet(string objectPath, IEnumerable<string> values, bool async = false, string routerId = null);

        Task<bool> PublishSets(IEnumerable<TrakHoundSetEntry> entries, bool async = false, string routerId = null);


        Task<bool> DeleteSet(string objectPath, bool async = false, string routerId = null);

        Task<bool> DeleteSet(string objectPath, string value, bool async = false, string routerId = null);
    }
}
