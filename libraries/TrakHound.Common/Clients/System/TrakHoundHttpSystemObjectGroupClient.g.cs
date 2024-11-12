// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Configurations;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal partial class TrakHoundHttpSystemObjectGroupClient : TrakHoundHttpEntityClientBase<ITrakHoundObjectGroupEntity>, ITrakHoundSystemObjectGroupClient
    {




        public TrakHoundHttpSystemObjectGroupClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByGroup(
            string groupPath,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "group");
            route = Url.AddQueryParameter(route, "groupPath", (string)groupPath);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByGroup(
            IEnumerable<string> groupPaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "group");
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, groupPaths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByGroupUuid(
            string groupUuid,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "group/{groupUuid}");
            route = Url.AddRouteParameter(route, "group/{groupUuid}", "groupUuid", groupUuid);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByGroupUuid(
            IEnumerable<string> groupUuids,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "group");
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, groupUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByMember(
            string memberPath,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member");
            route = Url.AddQueryParameter(route, "memberPath", (string)memberPath);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByMember(
            IEnumerable<string> memberPaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member");
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, memberPaths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByMemberUuid(
            string memberUuid,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member/{memberUuid}");
            route = Url.AddRouteParameter(route, "member/{memberUuid}", "memberUuid", memberUuid);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByMemberUuid(
            IEnumerable<string> memberUuids,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member");
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, memberUuids);

            return ProcessResponse(result);
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>> SubscribeByGroup(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "group/path/subscribe");
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectGroupEntity>(BaseClient.ClientConfiguration, url, paths);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>> SubscribeByGroupUuid(
            IEnumerable<string> groupUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "group/subscribe");
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectGroupEntity>(BaseClient.ClientConfiguration, url, groupUuids);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>> SubscribeByMember(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member/path/subscribe");
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectGroupEntity>(BaseClient.ClientConfiguration, url, paths);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>> SubscribeByMemberUuid(
            IEnumerable<string> memberUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member/subscribe");
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectGroupEntity>(BaseClient.ClientConfiguration, url, memberUuids);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<bool> DeleteByGroupUuid(
            string groupUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "group/{groupUuid}");
            route = Url.AddRouteParameter(route, "group/{groupUuid}", "groupUuid", groupUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);
            return await RestRequest.Delete(url);
        }

        public async Task<bool> DeleteByGroupUuid(
            IEnumerable<string> groupUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "delete/group");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);
            return (await RestRequest.PostResponse(url, groupUuids)).StatusCode == 200;
        }

        public async Task<bool> DeleteByMemberUuid(
            string memberUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member/{memberUuid}");
            route = Url.AddRouteParameter(route, "member/{memberUuid}", "memberUuid", memberUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);
            return await RestRequest.Delete(url);
        }

        public async Task<bool> DeleteByMemberUuid(
            IEnumerable<string> memberUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "delete/member");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectGroupEntity>());
            url = Url.Combine(url, route);
            return (await RestRequest.PostResponse(url, memberUuids)).StatusCode == 200;
        }

    }
}
