// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundMessageQueueMapping> GetMessageQueueMapping(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundMessageQueueMapping>> GetMessageQueueMappings(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundMessageQueueMapping>> GetMessageQueueMappings(IEnumerable<string> objectPaths, string routerId = null);


        Task<IEnumerable<TrakHoundMessageQueue>> PullFromMessageQueue(string objectPath, int count = 1, bool acknowledge = true, string routerId = null);


        Task<ITrakHoundConsumer<TrakHoundMessageQueue>> SubscribeMessageQueue(string objectPath, bool acknowledge = true, string routerId = null);

        Task<ITrakHoundConsumer<byte[]>> SubscribeMessageQueueContent(string objectPath, bool acknowledge = true, string routerId = null);

        Task<ITrakHoundConsumer<T>> SubscribeMessageQueueContent<T>(string objectPath, bool acknowledge = true, string routerId = null);


        Task<bool> PublishMessageQueueMapping(string objectPath, string queueId, string contentType = "application/octet-stream", bool async = false, string routerId = null);

        Task<bool> PublishMessageQueueMappings(IEnumerable<TrakHoundMessageQueueMappingEntry> entries, bool async = false, string routerId = null);


        Task<bool> PublishMessageQueueContent(string objectPath, string content, string routerId = null);

        Task<bool> PublishMessageQueueContent(string objectPath, byte[] content, string routerId = null);

        Task<bool> PublishMessageQueueContent(string objectPath, Stream content, string routerId = null);
    }
}
