// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<IEnumerable<TrakHoundObservation>> GetObservations(string objectPath, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null);

        Task<IEnumerable<TrakHoundObservation>> GetObservations(string objectPath, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null);

        Task<IEnumerable<TrakHoundObservation>> GetObservations(IEnumerable<string> objectPaths, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null);

        Task<IEnumerable<TrakHoundObservation>> GetObservations(IEnumerable<string> objectPaths, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null);



        Task<Dictionary<string, IEnumerable<TrakHoundObservationValue>>> GetObservationValues(string objectPath, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null);
        
        Task<Dictionary<string, IEnumerable<TrakHoundObservationValue>>> GetObservationValues(string objectPath, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null);

        Task<Dictionary<string, IEnumerable<TrakHoundObservationValue>>> GetObservationValues(IEnumerable<string> objectPaths, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null);

        Task<Dictionary<string, IEnumerable<TrakHoundObservationValue>>> GetObservationValues(IEnumerable<string> objectPaths, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null);



        Task<TrakHoundObservation> GetLatestObservation(string objectPath, string routerId = null);

        Task<TValue> GetLatestObservationValue<TValue>(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundObservation>> GetLatestObservations(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundObservation>> GetLatestObservations(IEnumerable<string> objectPaths, string routerId = null);



        Task<ITrakHoundConsumer<IEnumerable<TrakHoundObservation>>> SubscribeObservations(string objectPath, string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<TrakHoundObservation>>> SubscribeObservations(IEnumerable<string> objectPaths, string routerId = null);



        Task<bool> PublishObservation(string objectPath, object value, TrakHoundObservationDataType? dataType = null, DateTime? timestamp = null, ulong batchId = 0, ulong sequence = 0, bool async = false, string routerId = null);

        Task<bool> PublishObservations(IEnumerable<TrakHoundObservationEntry> entries, bool async = false, string routerId = null);
    }
}
