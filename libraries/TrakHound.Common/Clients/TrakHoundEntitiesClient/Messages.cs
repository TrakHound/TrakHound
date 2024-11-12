// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Entities;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _messagesRoute = "message";
        private const string _messagesMappingsRoute = "mappings";
        private const string _messagesContentRoute = "content";


        public async Task<TrakHoundMessageMapping> GetMessageMapping(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messagesRoute, _messagesMappingsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return (await ApiClient.QueryJson<IEnumerable<TrakHoundMessageMapping>>(url, parameters))?.FirstOrDefault();
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundMessageMapping>> GetMessageMappings(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messagesRoute, _messagesMappingsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundMessageMapping>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundMessageMapping>> GetMessageMappings(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messagesRoute, _messagesMappingsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundMessageMapping>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<TrakHoundMessage>> SubscribeMessageContent(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _messagesRoute, _messagesContentRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var apiConsumer = await ApiClient.Subscribe(route, parameters);
                if (apiConsumer != null)
                {
                    var outputConsumer = new TrakHoundConsumer<TrakHoundApiResponse, TrakHoundMessage>(apiConsumer);
                    outputConsumer.OnReceived = (response) =>
                    {
                        var message = new TrakHoundMessage();
                        message.Object = response.GetParameter("Object");
                        message.ObjectUuid = TrakHoundPath.GetUuid(message.Object);
                        message.BrokerId = response.GetParameter("BrokerId");
                        message.Topic = response.GetParameter("Topic");
                        message.Retain = response.GetParameter<bool>("Retain");
                        message.QoS = response.GetParameter<int>("Qos");
                        message.Timestamp = response.GetParameter<DateTime>("Timestamp");
                        message.Content = response.Content;
                        message.ContentType = response.ContentType;
                        return message;
                    };
                    return outputConsumer;
                }
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<TrakHoundMessage>> SubscribeMessageContent(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _messagesRoute, _messagesContentRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var apiConsumer = await ApiClient.Subscribe(route, objectPaths);
                if (apiConsumer != null)
                {
                    var outputConsumer = new TrakHoundConsumer<TrakHoundApiResponse, TrakHoundMessage>(apiConsumer);
                    outputConsumer.OnReceived = (response) =>
                    {
                        var message = new TrakHoundMessage();
                        message.Object = response.GetParameter("Object");
                        message.ObjectUuid = TrakHoundPath.GetUuid(message.Object);
                        message.BrokerId = response.GetParameter("BrokerId");
                        message.Topic = response.GetParameter("Topic");
                        message.Retain = response.GetParameter<bool>("Retain");
                        message.QoS = response.GetParameter<int>("Qos");
                        message.Timestamp = response.GetParameter<DateTime>("Timestamp");
                        message.Content = response.Content;
                        message.ContentType = response.ContentType;
                        return message;
                    };
                    return outputConsumer;
                }
            }

            return null;
        }


        public async Task<bool> PublishMessageMapping(string objectPath, string brokerId, string topic, string contentType = "application/octet-stream", bool retain = false, int qos = 0, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(brokerId) && !string.IsNullOrEmpty(topic))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messagesRoute, _messagesMappingsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["brokerId"] = brokerId;
                parameters["topic"] = topic;
                parameters["contentType"] = contentType;
                parameters["retain"] = retain.ToString();
                parameters["qos"] = qos.ToString();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishMessageMappings(IEnumerable<TrakHoundMessageMappingEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messagesRoute, $"{_messagesMappingsRoute}/batch");

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, entries, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }


        public async Task<bool> PublishMessageContent(string objectPath, string content, bool retain = false, int qos = 0, DateTime? timestamp = null, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(content))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messagesRoute, _messagesContentRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["retain"] = retain.ToString();
                parameters["qos"] = qos.ToString();
                if (timestamp.HasValue) parameters["timestamp"] = timestamp.Value.ToUnixTime().ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var bodyBytes = content.ToUtf8Bytes();

                var response = await ApiClient.Publish(url, bodyBytes, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishMessageContent(string objectPath, byte[] content, bool retain = false, int qos = 0, DateTime? timestamp = null, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !content.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messagesRoute, _messagesContentRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["retain"] = retain.ToString();
                parameters["qos"] = qos.ToString();
                if (timestamp.HasValue) parameters["timestamp"] = timestamp.Value.ToUnixTime().ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, content, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishMessageContent(string objectPath, Stream content, bool retain = false, int qos = 0, DateTime? timestamp = null, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && content != null)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _messagesRoute, _messagesContentRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["retain"] = retain.ToString();
                parameters["qos"] = qos.ToString();
                if (timestamp.HasValue) parameters["timestamp"] = timestamp.Value.ToUnixTime().ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, content, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }
    }
}
