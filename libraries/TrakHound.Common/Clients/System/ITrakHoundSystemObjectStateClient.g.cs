// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectStateClient : ITrakHoundEntityClient<ITrakHoundObjectStateEntity>
    {
        Task<IEnumerable<ITrakHoundObjectStateEntity>> LatestByObject(
            string path,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStateEntity>> LatestByObject(
            IEnumerable<string> paths,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStateEntity>> LatestByObjectUuid(
            string objectUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStateEntity>> LatestByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStateEntity>> QueryByObject(
            string path,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStateEntity>> QueryByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStateEntity>> QueryByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStateEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStateEntity>> LastByObject(
            string path,
            long timestamp,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStateEntity>> LastByObject(
            IEnumerable<string> paths,
            long timestamp,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStateEntity>> LastByObjectUuid(
            string objectUuid,
            long timestamp,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStateEntity>> LastByObjectUuid(
            IEnumerable<string> objectUuids,
            long timestamp,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

    }
}