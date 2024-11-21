// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectObservationClient : ITrakHoundEntityClient<ITrakHoundObjectObservationEntity>
    {
        Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObject(
            string path,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObject(
            IEnumerable<string> paths,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObjectUuid(
            string objectUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObject(
            string path,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObject(
            string path,
            long timestamp,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObject(
            IEnumerable<string> paths,
            long timestamp,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObjectUuid(
            string objectUuid,
            long timestamp,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObjectUuid(
            IEnumerable<string> objectUuids,
            long timestamp,
            string routerId = null);

        Task<IEnumerable<TrakHoundAggregate>> AggregateByObject(
            string path,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundAggregate>> AggregateByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundAggregate>> AggregateByObjectUuid(
            string objectUuid,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundAggregate>> AggregateByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObject(
            string path,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObjectUuid(
            string objectUuid,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundCount>> CountByObject(
            string path,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundCount>> CountByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundCount>> CountByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundCount>> CountByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<bool> DeleteByObjectUuid(
            string objectUuid,
            string routerId = null);

        Task<bool> DeleteByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null);

    }
}