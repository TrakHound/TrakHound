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
        Task<IEnumerable<TrakHoundEvent>> GetEvents(string objectPath, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);

        Task<IEnumerable<TrakHoundEvent>> GetEvents(string objectPath, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);

        Task<IEnumerable<TrakHoundEvent>> GetEvents(string objectPath, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);


        Task<TrakHoundEvent> GetLatestEvent(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundEvent>> GetLatestEvents(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundEvent>> GetLatestEvents(IEnumerable<string> objectPaths, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundEvent>>> SubscribeEvents(string objectPath, string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<TrakHoundEvent>>> SubscribeEvents(IEnumerable<string> objectPaths, string routerId = null);


        Task<bool> PublishEvent(string objectPath, string targetPath, DateTime? timestamp = null, bool async = false, string routerId = null);

        Task<bool> PublishEvents(IEnumerable<TrakHoundEventEntry> entries, bool async = false, string routerId = null);
    }
}
