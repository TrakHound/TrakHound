// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _objectsRoute = "objects";


        public async Task<TrakHoundObject> GetObject(string path, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["path"] = path;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return (await ApiClient.QueryJson<IEnumerable<TrakHoundObject>>(route, parameters))?.FirstOrDefault();
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundObject>> GetObjects(string path, int skip = 0, int take = 1000, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["path"] = path;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundObject>>(route, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundObject>> GetObjects(IEnumerable<string> paths, int skip = 0, int take = 1000, string routerId = null)
        {
            if (!paths.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundObject>>(route, paths, queryParameters: parameters);
            }

            return null;
        }

        public async Task<Dictionary<string, string>> GetObjectUuids(string path, int skip = 0, int take = 1000, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, "ids");
                route = Url.AddQueryParameter(route, "path", path);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, string>>(route, queryParameters: parameters);
            }

            return null;
        }

        public async Task<Dictionary<string, string>> GetObjectUuids(IEnumerable<string> paths, int skip = 0, int take = 1000, string routerId = null)
        {
            if (!paths.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, "ids");

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, string>>(route, paths, queryParameters: parameters);
            }

            return null;
        }

        public async Task<Dictionary<string, string>> GetObjectContentTypes(string path, int skip = 0, int take = 1000, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, "content-type");
                route = Url.AddQueryParameter(route, "path", path);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, string>>(route, queryParameters: parameters);
            }

            return null;
        }

        public async Task<Dictionary<string, string>> GetObjectContentTypes(IEnumerable<string> paths, int skip = 0, int take = 1000, string routerId = null)
        {
            if (!paths.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, "content-type");

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, string>>(route, paths, queryParameters: parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundObject>> GetObjectsByParentUuid(string parentUuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(parentUuid))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, $"{parentUuid}/children");

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundObject>>(route, queryParameters: parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundObject>> GetObjectsByParentUuid(IEnumerable<string> parentUuids, string routerId = null)
        {
            if (!parentUuids.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, "children");

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundObject>>(route, parentUuids, queryParameters: parameters);
            }

            return null;
        }


        public async Task<TrakHoundObjectResponse> PublishObject(
            string path,
            TrakHoundObjectContentType contentType = TrakHoundObjectContentType.Directory,
            string definitionId = null,
            Dictionary<string, string> metadata = null,
            bool async = false,
            string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, "batch");

                var ns = TrakHoundPath.GetNamespace(path);
                var partialPath = TrakHoundPath.GetPartialPath(path);

                var addPath = TrakHoundPath.ToRoot(partialPath);

                var requests = new List<TrakHoundObjectEntry>();

                var request = new TrakHoundObjectEntry();
                request.Path = addPath;
                request.Namespace = ns;
                request.ContentType = contentType.ToString();
                request.DefinitionId = definitionId;
                request.Metadata = metadata;
                requests.Add(request);

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = (await ApiClient.Publish<IEnumerable<TrakHoundObjectResponse>>(route, requests, queryParameters: parameters))?.FirstOrDefault(o => o.Path == addPath);
                await PublishAfter();
                return response;
            }

            return null;
        }

        public async Task<TrakHoundObjectResponse> PublishObject(TrakHoundObjectEntry entry, bool async = false, string routerId = null)
        {
            if (entry != null)
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, "batch");

                var entries = new List<TrakHoundObjectEntry>();
                entries.Add(entry);

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = (await ApiClient.Publish<IEnumerable<TrakHoundObjectResponse>>(route, entries, queryParameters: parameters))?.FirstOrDefault();
                await PublishAfter();
                return response;
            }

            return null;
        }

        public async Task<TrakHoundObjectResponse> PublishObject(IEnumerable<TrakHoundObjectEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, "batch");

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = (await ApiClient.Publish<IEnumerable<TrakHoundObjectResponse>>(route, entries, queryParameters: parameters))?.FirstOrDefault();
                await PublishAfter();
                return response;
            }

            return null;
        }


        public async Task<bool> DeleteObject(string path, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["path"] = path;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Delete(route, parameters);
                return response.Success;
            }

            return false;
        }

        public async Task<bool> DeleteObjects(IEnumerable<string> paths, string routerId = null)
        {
            if (!paths.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, "batch");

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Delete(route, paths, queryParameters: parameters);
                return response.Success;
            }

            return false;
        }
    }
}
