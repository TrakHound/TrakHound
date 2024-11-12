// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _messageQueuesRoute = "message-queue";
        private const string _messageQueuesMappingsRoute = "mappings";
        private const string _messageQueuesContentRoute = "content";


        public async Task<TrakHoundMessageQueueMapping> GetMessageQueueMapping(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute, _messageQueuesMappingsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return (await ApiClient.QueryJson<IEnumerable<TrakHoundMessageQueueMapping>>(url, parameters))?.FirstOrDefault();
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundMessageQueueMapping>> GetMessageQueueMappings(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute, _messageQueuesMappingsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundMessageQueueMapping>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundMessageQueueMapping>> GetMessageQueueMappings(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute, _messageQueuesMappingsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundMessageQueueMapping>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<IEnumerable<TrakHoundMessageQueue>> PullFromMessageQueue(string objectPath, int count = 1, bool acknowledge = true, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute);
                url = Url.Combine(url, "pull");

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["count"] = count.ToString();
                parameters["acknowledge"] = acknowledge.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundMessageQueue>>(url, parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<TrakHoundMessageQueue>> SubscribeMessageQueue(string objectPath, bool acknowledge = true, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["acknowledge"] = acknowledge.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<TrakHoundMessageQueue>(route, parameters);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<byte[]>> SubscribeMessageQueueContent(string objectPath, bool acknowledge = true, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute, _messageQueuesContentRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["acknowledge"] = acknowledge.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeBytes(route, parameters);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<T>> SubscribeMessageQueueContent<T>(string objectPath, bool acknowledge = true, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute, _messageQueuesContentRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["acknowledge"] = acknowledge.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<T>(route, parameters);
            }

            return null;
        }


        public async Task<bool> PublishMessageQueueMapping(string objectPath, string queueId, string contentType = "application/octet-stream", bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(queueId))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute, _messageQueuesMappingsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["queueId"] = queueId;
                parameters["contentType"] = contentType;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishMessageQueueMappings(IEnumerable<TrakHoundMessageQueueMappingEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute, $"{_messageQueuesMappingsRoute}/batch");

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, entries, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }


        public async Task<bool> PublishMessageQueueContent(string objectPath, string content, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(content))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute, _messageQueuesContentRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var bodyBytes = System.Text.Encoding.UTF8.GetBytes(content);

                var response = await ApiClient.Publish(url, bodyBytes, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishMessageQueueContent(string objectPath, byte[] content, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !content.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute, _messageQueuesContentRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, content, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishMessageQueueContent(string objectPath, Stream content, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && content != null)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messageQueuesRoute, _messageQueuesContentRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, content, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }
    }
}
