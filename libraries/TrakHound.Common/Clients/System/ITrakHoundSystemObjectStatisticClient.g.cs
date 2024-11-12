// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectStatisticClient : ITrakHoundEntityClient<ITrakHoundObjectStatisticEntity>
    {
        Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObject(
            string path,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObject(
            string path,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            string routerId = null);

        Task<IEnumerable<TrakHoundCount>> CountByObject(
            string path,
            long start,
            long stop,
            long span,
            string routerId = null);

        Task<IEnumerable<TrakHoundCount>> CountByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<TrakHoundCount> CountByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            long span,
            string routerId = null);

        Task<IEnumerable<TrakHoundCount>> CountByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long span,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>> SubscribeByObjectUuid(
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