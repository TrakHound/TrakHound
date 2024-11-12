// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundString> GetString(string objectPath, string routerId = null);

        Task<string> GetStringValue(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundString>> GetStrings(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundString>> GetStrings(IEnumerable<string> objectPaths, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundString>>> SubscribeStrings(string objectPath, string routerId = null);


        Task<bool> PublishString(string objectPath, string value, bool async = false, string routerId = null);

        Task<bool> PublishStrings(IEnumerable<TrakHoundStringEntry> entries, bool async = false, string routerId = null);

        Task<bool> DeleteString(string objectPath, string routerId = null);
    }
}
