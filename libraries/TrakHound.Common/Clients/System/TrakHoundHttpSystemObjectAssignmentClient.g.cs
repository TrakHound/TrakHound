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
    internal partial class TrakHoundHttpSystemObjectAssignmentClient : TrakHoundHttpEntityClientBase<ITrakHoundObjectAssignmentEntity>, ITrakHoundSystemObjectAssignmentClient
    {




        public TrakHoundHttpSystemObjectAssignmentClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(
            string assigneePath,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "current/assignee");
            route = Url.AddQueryParameter(route, "assigneePath", (string)assigneePath);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(
            IEnumerable<string> assigneePaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "current/assignee/path");
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, assigneePaths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssigneeUuid(
            string assigneeUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "current/assignee/{assigneeUuid}");
            route = Url.AddRouteParameter(route, "current/assignee/{assigneeUuid}", "assigneeUuid", assigneeUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "current/assignee");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, assigneeUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMember(
            string memberPath,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "current/member");
            route = Url.AddQueryParameter(route, "memberPath", (string)memberPath);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMember(
            IEnumerable<string> memberPaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "current/member/path");
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, memberPaths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMemberUuid(
            string memberUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "current/member/{memberUuid}");
            route = Url.AddRouteParameter(route, "current/member/{memberUuid}", "memberUuid", memberUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMemberUuid(
            IEnumerable<string> memberUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "current/member");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, memberUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssignee(
            string assigneePath,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "assignee");
            route = Url.AddQueryParameter(route, "assigneePath", (string)assigneePath);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssignee(
            IEnumerable<string> assigneePaths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "assignee/path");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, assigneePaths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssigneeUuid(
            string assigneeUuid,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "assignee/{assigneeUuid}");
            route = Url.AddRouteParameter(route, "assignee/{assigneeUuid}", "assigneeUuid", assigneeUuid);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "assignee");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, assigneeUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMember(
            string memberPath,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member");
            route = Url.AddQueryParameter(route, "memberPath", (string)memberPath);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMember(
            IEnumerable<string> memberPaths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member/path");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, memberPaths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMemberUuid(
            string memberUuid,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member/{memberUuid}");
            route = Url.AddRouteParameter(route, "member/{memberUuid}", "memberUuid", memberUuid);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMemberUuid(
            IEnumerable<string> memberUuids,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, memberUuids);

            return ProcessResponse(result);
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByAssignee(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "assignee/path/subscribe");
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectAssignmentEntity>(BaseClient.ClientConfiguration, url, paths);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "assignee/subscribe");
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectAssignmentEntity>(BaseClient.ClientConfiguration, url, assigneeUuids);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByMember(
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

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectAssignmentEntity>(BaseClient.ClientConfiguration, url, paths);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByMemberUuid(
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

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectAssignmentEntity>(BaseClient.ClientConfiguration, url, memberUuids);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<bool> DeleteByAssigneeUuid(
            string assigneeUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "assignee/{assigneeUuid}");
            route = Url.AddRouteParameter(route, "assignee/{assigneeUuid}", "assigneeUuid", assigneeUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);
            return await RestRequest.Delete(url);
        }

        public async Task<bool> DeleteByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "delete/assignee");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);
            return (await RestRequest.PostResponse(url, assigneeUuids)).StatusCode == 200;
        }

        public async Task<bool> DeleteByMemberUuid(
            string memberUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "member/{memberUuid}");
            route = Url.AddRouteParameter(route, "member/{memberUuid}", "memberUuid", memberUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
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

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);
            return (await RestRequest.PostResponse(url, memberUuids)).StatusCode == 200;
        }

    }
}
