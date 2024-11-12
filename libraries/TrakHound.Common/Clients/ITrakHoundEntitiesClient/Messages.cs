// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundMessageMapping> GetMessageMapping(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundMessageMapping>> GetMessageMappings(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundMessageMapping>> GetMessageMappings(IEnumerable<string> objectPaths, string routerId = null);


        Task<ITrakHoundConsumer<TrakHoundMessage>> SubscribeMessageContent(string objectPath, string routerId = null);

        Task<ITrakHoundConsumer<TrakHoundMessage>> SubscribeMessageContent(IEnumerable<string> objectPaths, string routerId = null);


        Task<bool> PublishMessageMapping(string objectPath, string brokerId, string topic, string contentType = "application/octet-stream", bool retain = false, int qos = 0, bool async = false, string routerId = null);

        Task<bool> PublishMessageMappings(IEnumerable<TrakHoundMessageMappingEntry> entries, bool async = false, string routerId = null);


        Task<bool> PublishMessageContent(string objectPath, string content, bool retain = false, int qos = 0, DateTime? timestamp = null, string routerId = null);

        Task<bool> PublishMessageContent(string objectPath, byte[] content, bool retain = false, int qos = 0, DateTime? timestamp = null, string routerId = null);

        Task<bool> PublishMessageContent(string objectPath, Stream content, bool retain = false, int qos = 0, DateTime? timestamp = null, string routerId = null);
    }
}
