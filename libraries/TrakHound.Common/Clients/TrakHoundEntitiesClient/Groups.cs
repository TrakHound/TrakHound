// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _groupsRoute = "group";


        public async Task<IEnumerable<TrakHoundGroup>> GetGroups(string groupPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(groupPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _groupsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["groupPath"] = groupPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundGroup>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundGroup>> GetGroups(IEnumerable<string> groupPaths, string routerId = null)
        {
            if (!groupPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _groupsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundGroup>>(url, groupPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundGroup>>> SubscribeGroups(string groupPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(groupPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _groupsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["groupPath"] = groupPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundGroup>>(route, parameters);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundGroup>>> SubscribeGroupsByMember(string memberPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(memberPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, $"{_groupsRoute}/member");

                var parameters = new Dictionary<string, string>();
                parameters["groupPath"] = memberPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundGroup>>(route, parameters);
            }

            return null;
        }


        public async Task<bool> PublishGroup(string groupPath, string memberPath, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(groupPath) && !string.IsNullOrEmpty(memberPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _groupsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["groupPath"] = groupPath;
                parameters["memberPath"] = memberPath;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishGroups(IEnumerable<TrakHoundGroupEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_groupsRoute}/batch");

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, entries, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }


        public async Task<bool> DeleteGroup(string groupPath, string memberPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(groupPath) && !string.IsNullOrEmpty(memberPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _groupsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["groupPath"] = groupPath;
                parameters["memberPath"] = memberPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Delete(url, parameters);
                return response.Success;
            }

            return false;
        }
    }
}
