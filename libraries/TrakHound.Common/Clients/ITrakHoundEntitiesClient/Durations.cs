// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundDuration> GetDuration(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundDuration>> GetDurations(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundDuration>> GetDurations(IEnumerable<string> objectPaths, string routerId = null);


        Task<bool> PublishDuration(string objectPath, TimeSpan value, bool async = false, string routerId = null);

        Task<bool> PublishDurations(IEnumerable<TrakHoundDurationEntry> entries, bool async = false, string routerId = null);
    }
}
