// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _sourcesRoute = "sources";


        public async Task<TrakHoundSource> GetSource(string uuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var url = Url.Combine(ApiRoute, _sourcesRoute, uuid);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<TrakHoundSource>(url, queryParameters: parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundSource>> GetSources(IEnumerable<string> uuids, string routerId = null)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _sourcesRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundSource>>(url, uuids, queryParameters: parameters);
            }

            return null;
        }


        public async Task<IEnumerable<TrakHoundSource>> GetSourceChain(string uuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var url = Url.Combine(ApiRoute, _sourcesRoute, uuid, "chain");

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundSource>>(url, queryParameters: parameters);
            }

            return null;
        }


        public async Task<bool> PublishSource(TrakHoundSourceEntry entry, string routerId = null)
        {
            if (entry != null)
            {
                var entries = new List<TrakHoundSourceEntry>();
                entries.Add(entry);
                return await PublishSources(entries, routerId);
            }

            return false;
        }

        public async Task<bool> PublishSources(IEnumerable<TrakHoundSourceEntry> entries, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, $"{_sourcesRoute}/batch");

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, entries, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }
    }
}
