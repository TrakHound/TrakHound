// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectMessageQueueClient
    {
        public async Task<ITrakHoundConsumer<TrakHoundAcknowledgeResult<ITrakHoundObjectMessageQueueEntity>>> SubscribeByObjectUuid(string objectUuid, bool acknowledge = true, string routerId = null)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundAcknowledgeResult<ITrakHoundObjectMessageQueueEntity>>> PullByObjectUuid(string objectUuid, int count = 1, bool acknowledge = true, string routerId = null)
        {
            return null;
        }


        public async Task<bool> AcknowledgeByObjectUuid(string objectUuid, IEnumerable<string> deliveryIds, string routerId = null)
        {
            return false;
        }

        public async Task<bool> RejectByObjectUuid(string objectUuid, IEnumerable<string> deliveryIds, string routerId = null)
        {
            return false;
        }
    }
}
