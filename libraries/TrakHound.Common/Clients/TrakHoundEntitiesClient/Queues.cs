// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _queuesRoute = "queue";


        public async Task<IEnumerable<TrakHoundQueue>> GetQueues(string queuePath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(queuePath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _queuesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["queuePath"] = queuePath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundQueue>>(url, parameters);
            }

            return null;
        }


        public async Task<IEnumerable<TrakHoundQueue>> PullFromQueue(string queuePath, int count = 1, string routerId = null)
        {
            if (!string.IsNullOrEmpty(queuePath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _queuesRoute);
                url = Url.Combine(url, "pull");

                var parameters = new Dictionary<string, string>();
                parameters["queuePath"] = queuePath;
                parameters["count"] = count.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundQueue>>(url, parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundQueue>>> SubscribeQueues(string queuePath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(queuePath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _queuesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["queuePath"] = queuePath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var apiConsumer = await ApiClient.Subscribe(route, parameters);
                if (apiConsumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<TrakHoundApiResponse, IEnumerable<TrakHoundQueue>>(apiConsumer, allowNull: true);
                    resultConsumer.OnReceivedAsync = async (response) =>
                    {
                        try
                        {
                            byte[] requestBody = null;
                            if (response.Content != null)
                            {
                                using (var readStream = new MemoryStream())
                                {
                                    await response.Content.CopyToAsync(readStream);
                                    requestBody = readStream.ToArray();
                                }
                            }
                            var json = Encoding.UTF8.GetString(requestBody);
                            return Json.Convert<IEnumerable<TrakHoundQueue>>(json);
                        }
                        catch { }

                        return null;
                    };
                    return resultConsumer;
                }
            }

            return null;
        }


        public async Task<bool> PublishQueue(string queuePath, string memberPath, int index = 0, string routerId = null)
        {
            if (!string.IsNullOrEmpty(queuePath) && !string.IsNullOrEmpty(memberPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _queuesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["queuePath"] = queuePath;
                parameters["memberPath"] = memberPath;
                parameters["index"] = index.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishQueues(IEnumerable<TrakHoundQueueEntry> entries, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_queuesRoute}/batch");

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, entries, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }


        public async Task<bool> DeleteQueue(string queuePath, string memberPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(queuePath) && !string.IsNullOrEmpty(memberPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _queuesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["queuePath"] = queuePath;
                parameters["memberPath"] = memberPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Delete(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }
    }
}
