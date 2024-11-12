// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<IEnumerable<TrakHoundGroup>> GetGroups(string groupPath, string routerId = null);

        Task<IEnumerable<TrakHoundGroup>> GetGroups(IEnumerable<string> groupPaths, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundGroup>>> SubscribeGroups(string groupPath, string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<TrakHoundGroup>>> SubscribeGroupsByMember(string memberPath, string routerId = null);


        Task<bool> PublishGroup(string groupPath, string memberPath, bool async = false, string routerId = null);

        Task<bool> PublishGroups(IEnumerable<TrakHoundGroupEntry> entries, bool async = false, string routerId = null);


        Task<bool> DeleteGroup(string groupPath, string memberPath, string routerId = null);
    }
}
