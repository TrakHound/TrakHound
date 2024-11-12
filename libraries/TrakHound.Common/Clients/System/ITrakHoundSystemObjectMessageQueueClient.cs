// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectMessageQueueClient
    {
        //Task<IEnumerable<ITrakHoundObjectMessageQueueEntity>> QueryByObject(
        //    string path,
        //    string routerId = null);

        //Task<IEnumerable<ITrakHoundObjectMessageQueueEntity>> QueryByObject(
        //    IEnumerable<string> paths,
        //    string routerId = null);

        //Task<ITrakHoundObjectMessageQueueEntity> QueryByObjectUuid(
        //    string objectUuid,
        //    string routerId = null);

        //Task<IEnumerable<ITrakHoundObjectMessageQueueEntity>> QueryByObjectUuid(
        //    IEnumerable<string> objectUuids,
        //    string routerId = null);

        //Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageQueueEntity>>> SubscribeByObject(
        //    IEnumerable<string> paths,
        //    int interval = 0,
        //    int take = 1000,
        //    string consumerId = null,
        //    string routerId = null);

        //Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageQueueEntity>>> SubscribeByObjectUuid(
        //    IEnumerable<string> objectUuids,
        //    int interval = 0,
        //    int take = 1000,
        //    string consumerId = null,
        //    string routerId = null);

        //Task<IEnumerable<TrakHoundAcknowledgeResult<ITrakHoundObjectMessageQueueEntity>>> PullByObjectUuid(string objectUuid, int count = 1, bool acknowledge = true, string routerId = null);

        //Task<ITrakHoundConsumer<TrakHoundAcknowledgeResult<ITrakHoundObjectMessageQueueEntity>>> SubscribeByObjectUuid(string objectUuid, bool acknowledge = true, string routerId = null);


        //Task<bool> AcknowledgeByObjectUuid(string objectUuid, IEnumerable<string> deliveryIds, string routerId = null);

        //Task<bool> RejectByObjectUuid(string objectUuid, IEnumerable<string> deliveryIds, string routerId = null);
    }
}
