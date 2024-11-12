// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundSource> GetSource(string uuid, string routerId = null);

        Task<IEnumerable<TrakHoundSource>> GetSources(IEnumerable<string> uuids, string routerId = null);


        Task<IEnumerable<TrakHoundSource>> GetSourceChain(string uuid, string routerId = null);


        Task<bool> PublishSource(TrakHoundSourceEntry entry, string routerId = null);

        Task<bool> PublishSources(IEnumerable<TrakHoundSourceEntry> entries, string routerId = null);
    }
}
