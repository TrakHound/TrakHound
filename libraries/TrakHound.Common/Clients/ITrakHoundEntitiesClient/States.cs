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
        Task<IEnumerable<TrakHoundState>> GetStates(string objectPath, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null);

        Task<IEnumerable<TrakHoundState>> GetStates(string objectPath, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null);

        Task<IEnumerable<TrakHoundState>> GetStates(string objectPath, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null);


        Task<TrakHoundState> GetLatestState(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundState>> GetLatestStates(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundState>> GetLatestStates(IEnumerable<string> objectPaths, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundState>>> SubscribeStates(string objectPath, string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<TrakHoundState>>> SubscribeStates(IEnumerable<string> objectPaths, string routerId = null);


        Task<bool> PublishState(string objectPath, string definition, int ttl = 0, DateTime? timestamp = null, bool async = false, string routerId = null);

        Task<bool> PublishStates(IEnumerable<TrakHoundStateEntry> entries, bool async = false, string routerId = null);
    }
}
