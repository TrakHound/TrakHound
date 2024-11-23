// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Sqlite.Drivers.Models;
using TrakHound.Utilities;

namespace TrakHound.Sqlite.Drivers
{
    public class ObjectDriver : SqliteEntityDriver<ITrakHoundObjectEntity, DatabaseObject>,
        IObjectQueryDriver,
        IObjectDeleteDriver,
        IEntityIndexQueryDriver<ITrakHoundObjectEntity>,
        IEntityIndexUpdateDriver<ITrakHoundObjectEntity>
    {
        //public static readonly Dictionary<string, ObjectDriver> Instances = new Dictionary<string, ObjectDriver>();
        private static readonly object _instanceLock = new object();

        private readonly Dictionary<string, ITrakHoundObjectEntity> _uuidIndex = new Dictionary<string, ITrakHoundObjectEntity>();
        private readonly Dictionary<string, IEnumerable<string>> _queryIndex = new Dictionary<string, IEnumerable<string>>();
        private readonly Dictionary<string, IEnumerable<string>> _reverseQueryIndex = new Dictionary<string, IEnumerable<string>>();
        private readonly Dictionary<string, string> _scripts = new Dictionary<string, string>();
        private readonly object _lock = new object();


        public ObjectDriver() { }

        public ObjectDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            //AddInstance(configuration.Id, this);

            EntityName = "trakhound_objects";
            TableColumnList = new List<string> {
                "[uuid]",
                "[namespace]",
                "[path]",
                "[name]",
                "[content_type]",
                "[priority]",
                "[definition_uuid]",
                "[source_uuid]",
                "[created]"
            };
        }

        //private static void AddInstance(string configurationId, ObjectDriver instance)
        //{
        //    if (configurationId != null && instance != null)
        //    {
        //        lock (_instanceLock)
        //        {
        //            Instances.Remove(configurationId);
        //            Instances.Add(configurationId, instance);
        //        }
        //    }
        //}

        //public static ObjectDriver GetInstance(string configurationId)
        //{
        //    if (configurationId != null)
        //    {
        //        lock (_instanceLock)
        //        {
        //            return Instances.GetValueOrDefault(configurationId);
        //        }
        //    }

        //    return null;
        //}

        class PublishItem
        {
            public string Uuid { get; set; }
            public string ParentUuid { get; set; }
            public string Namespace { get; set; }
            public string Path { get; set; }
            public string Name { get; set; }
            public string ContentType { get; set; }
            public int Priority { get; set; }
            public string DefinitionUuid { get; set; }
            public string SourceUuid { get; set; }
            public long Created { get; set; }      
        }

        class IndexPublishItem
        {
            public string Uuid { get; set; }
            public string Target { get; set; }
            public string Value { get; set; }
            public string Subject { get; set; }
            public long Order { get; set; }
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> Query(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (_client != null && !queryRequest.Queries.IsNullOrEmpty())
            {
                var sqlQuery = "";

                // Add Queries
                sqlQuery += "drop table if exists _queries;";
                sqlQuery += "create temp table _queries(id text);";
                foreach (var query in queryRequest.Queries)
                {
                    sqlQuery += $"insert into _queries (id) values ('{query}');";
                }

                // Add ParentUuids
                sqlQuery += "drop table if exists _parentUuids;";
                sqlQuery += "create temp table _parentUuids(id text);";
                if (!queryRequest.ParentUuids.IsNullOrEmpty())
                {
                    foreach (var parentUuid in queryRequest.ParentUuids)
                    {
                        if (!string.IsNullOrEmpty(parentUuid)) sqlQuery += $"insert into _parentUuids (id) values ('{parentUuid}');";
                        else sqlQuery += $"insert into _parentUuids (id) values (null);";
                    }
                }


                // Run SQL Query
                if (!queryRequest.ParentUuids.IsNullOrEmpty()) sqlQuery += $"select [queries].[id] as [query], [uuid], [parent_uuids].[id] as [parent_uuid], [parent_uuids].[requested_id] as [requested_parent_uuid] from [trakhound_objects] as [objects]";
                else sqlQuery += $"select [queries].[id] as [query], [uuid] from [trakhound_objects] as [objects]";

                switch (queryRequest.Type)
                {
                    case TrakHoundObjectQueryRequestType.Uuid:
                        sqlQuery += $" inner join (select [id] from _queries) as [queries] on [queries].[id] = [objects].[uuid]";
                        break;

                    case TrakHoundObjectQueryRequestType.ContentType:
                        sqlQuery += $" inner join (select [id] from _queries) as [queries] on lower([queries].[id]) = lower([objects].[content_type])";
                        break;

                    case TrakHoundObjectQueryRequestType.Name:
                        sqlQuery += $" inner join (select [id] from _queries) as [queries] on (instr('~', [queries].[id]) = 1 and [objects].[name] like replace(substring([queries].[id], 2, 1000), '*', '%')) or lower([queries].[id]) = lower([objects].[name])";
                        break;

                    case TrakHoundObjectQueryRequestType.DefinitionUuid:
                        sqlQuery += $" inner join (select [id] from _queries) as [queries] on [queries].[id] = [objects].[definition_uuid]";
                        break;
                }

                if (!queryRequest.ParentUuids.IsNullOrEmpty())
                {
                    if (queryRequest.ParentLevel > 1)
                    {
                        sqlQuery += $" inner join (select [requested_id], [uuid] as [id] from ({GetScript("GetObjectQueryChildren")} where [level] = {queryRequest.ParentLevel})) as [parent_uuids] on [parent_uuids].[id] = [objects].[parent_uuid] or ([parent_uuids].[id] is null and [objects].[parent_uuid] is null)";
                    }
                    else if (queryRequest.ParentLevel > 0)
                    {
                        sqlQuery += $" inner join (select [id] as [requested_id], [id] from _parentUuids) as [parent_uuids] on [parent_uuids].[id] = [objects].[parent_uuid] or ([parent_uuids].[id] is null and [objects].[parent_uuid] is null)";
                    }
                    else
                    {
                        sqlQuery += $" inner join (select [requested_id], [uuid] as [id] from ({GetScript("GetObjectQueryChildren")}) union select [id] as [requested_id], [id] from _parentUuids) as [parent_uuids] on [parent_uuids].[id] = [objects].[parent_uuid] or ([parent_uuids].[id] is null and [objects].[parent_uuid] is null)";
                    }
                }

                if (!string.IsNullOrEmpty(queryRequest.Namespace))
                {
                    sqlQuery += $" where lower([objects].[namespace]) = '{queryRequest.Namespace.ToLower()}'";
                }

                var responses = _client.ReadList<DatabaseQueryResult>(GetReadConnectionString(), sqlQuery);

                // Set Results
                if (!responses.IsNullOrEmpty())
                {
                    if (!queryRequest.ParentUuids.IsNullOrEmpty())
                    {
                        foreach (var parentUuid in queryRequest.ParentUuids)
                        {
                            var isRoot = parentUuid == null;

                            foreach (var query in queryRequest.Queries)
                            {
                                var parentResponses = responses.Where(o => o.Query?.ToLower() == query?.ToLower() && o.RequestedParentUuid == parentUuid);
                                if (!parentResponses.IsNullOrEmpty())
                                {
                                    foreach (var response in parentResponses)
                                    {
                                        var queryResult = new TrakHoundObjectQueryResult(queryRequest.Type, queryRequest.Namespace, query, response.Uuid, queryRequest.ParentLevel, response.ParentUuid, response.RequestedParentUuid, isRoot);

                                        results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.Ok, queryResult, response.Uuid));
                                    }
                                }
                                else
                                {
                                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.Empty));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var query in queryRequest.Queries)
                        {
                            var parentResponses = responses.Where(o => o.Query?.ToLower() == query?.ToLower());
                            if (!parentResponses.IsNullOrEmpty())
                            {
                                foreach (var response in parentResponses)
                                {
                                    var queryResult = new TrakHoundObjectQueryResult(queryRequest.Type, queryRequest.Namespace, query, response.Uuid);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.Ok, queryResult, response.Uuid));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.Empty));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var query in queryRequest.Queries)
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.Empty));
                    }
                }
            }
            else
            {
                results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRoot(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (_client != null)
            {
                var sql = "select [uuid] as [query], [uuid] from [trakhound_objects] where [parent_uuid] is null;";

                var responses = _client.ReadList<DatabaseIdQuery>(GetReadConnectionString(), sql);

                // Set Results
                if (!responses.IsNullOrEmpty())
                {
                    foreach (var response in responses)
                    {
                        var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, "All", response.Uuid);

                        results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, "All", TrakHoundResultType.Ok, queryResult, response.Uuid));
                    }
                }
                else
                {
                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, "All", TrakHoundResultType.Empty));
                }
            }
            else
            {
                results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRoot(IEnumerable<string> namespaces, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (_client != null)
            {
                if (!namespaces.IsNullOrEmpty())
                {
                    var sql = "";
                    sql += "drop table if exists _namespaces;";
                    sql += "create temp table _namespaces(id text);";

                    foreach (var ns in namespaces)
                    {
                        if (!string.IsNullOrEmpty(ns)) sql += $"insert into _namespaces (id) values ('{ns}');";
                        else sql += $"insert into _namespaces (id) values (null);";
                    }

                    sql += "select [b].[id] as [query], [a].[uuid] from [trakhound_objects] as [a]";
                    sql += " inner join _namespaces as [b] on lower([a].[namespace]) = lower([b].[id])";
                    sql += " where [a].[parent_uuid] is null;";

                    var responses = _client.ReadList<DatabaseIdQuery>(GetReadConnectionString(), sql);

                    // Set Results
                    if (!responses.IsNullOrEmpty())
                    {
                        foreach (var ns in namespaces)
                        {
                            var parentResponses = responses.Where(o => o.Query == ns);
                            if (!parentResponses.IsNullOrEmpty())
                            {
                                foreach (var response in parentResponses)
                                {
                                    var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, ns, ns, response.Uuid);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, ns, TrakHoundResultType.Ok, queryResult, response.Uuid));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, ns, TrakHoundResultType.Empty));
                            }
                        }
                    }
                    else
                    {
                        foreach (var ns in namespaces)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, ns, TrakHoundResultType.Empty));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.BadRequest(Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (_client != null)
            {
                if (!parentUuids.IsNullOrEmpty())
                {
                    var sql = "";
                    sql += "drop table if exists _uuids;";
                    sql += "create temp table _uuids(id text);";

                    foreach (var parentUuid in parentUuids)
                    {
                        if (!string.IsNullOrEmpty(parentUuid)) sql += $"insert into _uuids (id) values ('{parentUuid}');";
                        else sql += $"insert into _uuids (id) values (null);";
                    }

                    sql += "select [b].[id] as [query], [a].[uuid] as [uuid] from [trakhound_objects] as [a]";
                    sql += " inner join _uuids as [b] on [a].[parent_uuid] = [b].[id] or ([b].[id] is null and [a].[parent_uuid] is null);";

                    var responses = _client.ReadList<DatabaseIdQuery>(GetReadConnectionString(), sql);

                    // Set Results
                    if (!responses.IsNullOrEmpty())
                    {
                        foreach (var parentUuid in parentUuids)
                        {
                            var parentResponses = responses.Where(o => o.Query == parentUuid);
                            if (!parentResponses.IsNullOrEmpty())
                            {
                                foreach (var response in parentResponses)
                                {
                                    var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, parentUuid, response.Uuid);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, parentUuid, TrakHoundResultType.Ok, queryResult, response.Uuid));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, parentUuid, TrakHoundResultType.Empty));
                            }
                        }
                    }
                    else
                    {
                        foreach (var parentUuid in parentUuids)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, parentUuid, TrakHoundResultType.Empty));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.BadRequest(Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (_client != null)
            {
                if (!childUuids.IsNullOrEmpty())
                {
                    var sql = "";
                    sql += "drop table if exists _uuids;";
                    sql += "create temp table _uuids(id text);";

                    foreach (var childUuid in childUuids)
                    {
                        if (!string.IsNullOrEmpty(childUuid)) sql += $"insert into _uuids (id) values ('{childUuid}');";
                        else sql += $"insert into _uuids (id) values (null);";
                    }

                    sql += "select [b].[id] as [query], [a].[parent_uuid] from [trakhound_objects] as [a]";
                    sql += " inner join _uuids as [b] on [a].[uuid] = [b].[id];";

                    var responses = _client.ReadList<DatabaseIdQuery>(GetReadConnectionString(), sql);

                    // Set Results
                    if (!responses.IsNullOrEmpty())
                    {
                        foreach (var childUuid in childUuids)
                        {
                            var parentResponses = responses.Where(o => o.Query == childUuid);
                            if (!parentResponses.IsNullOrEmpty())
                            {
                                foreach (var response in parentResponses)
                                {
                                    var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, childUuid, response.Uuid);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.Ok, queryResult, response.Uuid));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.Empty));
                            }
                        }
                    }
                    else
                    {
                        foreach (var parentUuid in childUuids)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, parentUuid, TrakHoundResultType.Empty));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.BadRequest(Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (_client != null)
            {
                if (!rootUuids.IsNullOrEmpty())
                {
                    var responses = GetObjectChildIds(rootUuids);
                    if (!responses.IsNullOrEmpty())
                    {
                        var dResponses = responses.ToListDictionary(o => o.RequestedId);

                        foreach (var rootUuid in rootUuids)
                        {
                            //var parentResponses = responses.Where(o => o.RequestedId == rootUuid);
                            var parentResponses = dResponses.Get(rootUuid);
                            if (!parentResponses.IsNullOrEmpty())
                            {
                                foreach (var response in parentResponses)
                                {
                                    var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, rootUuid, response.Uuid);
                                    queryResult.Skip = skip;
                                    queryResult.Take = take;
                                    queryResult.SortOrder = sortOrder;

                                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, rootUuid, TrakHoundResultType.Ok, queryResult, response.Uuid));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, rootUuid, TrakHoundResultType.Empty));
                            }
                        }
                    }
                    else
                    {
                        foreach (var rootUuid in rootUuids)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, rootUuid, TrakHoundResultType.Empty));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.BadRequest(Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (_client != null)
            {
                if (!childUuids.IsNullOrEmpty())
                {
                    var responses = GetObjectRootIds(childUuids);
                    if (!responses.IsNullOrEmpty())
                    {
                        foreach (var rootUuid in childUuids)
                        {
                            var parentResponses = responses.Where(o => o.RequestedId == rootUuid);
                            if (!parentResponses.IsNullOrEmpty())
                            {
                                foreach (var response in parentResponses)
                                {
                                    var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, rootUuid, response.Uuid);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, rootUuid, TrakHoundResultType.Ok, queryResult, response.Uuid));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, rootUuid, TrakHoundResultType.Empty));
                            }
                        }
                    }
                    else
                    {
                        foreach (var rootUuid in childUuids)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, rootUuid, TrakHoundResultType.Empty));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.BadRequest(Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<ITrakHoundObjectQueryResult>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results, stpw.ElapsedTicks);
        }


        public async Task<TrakHoundResponse<string>> ListNamespaces(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<string>>();

            if (_client != null)
            {
                var sql = "select distinct [namespace] from [trakhound_objects];";

                var responses = _client.ReadStringList(GetReadConnectionString(), sql);
                if (!responses.IsNullOrEmpty())
                {
                    foreach (var response in responses)
                    {
                        results.Add(new TrakHoundResult<string>(Id, "All", TrakHoundResultType.Ok, response));
                    }
                }
                else
                {
                    results.Add(new TrakHoundResult<string>(Id, "All", TrakHoundResultType.Empty));
                }
            }
            else
            {
                results.Add(TrakHoundResult<string>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<string>(results, stpw.ElapsedTicks);
        }


        public async Task<TrakHoundResponse<TrakHoundCount>> QueryChildCount(IEnumerable<string> uuids)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<TrakHoundCount>>();

            if (_client != null)
            {
                if (!uuids.IsNullOrEmpty())
                {
                    var sql = "";
                    sql += "drop table if exists _uuids;";
                    sql += "create temp table _uuids(id text);";

                    foreach (var uuid in uuids)
                    {
                        if (!string.IsNullOrEmpty(uuid)) sql += $"insert into _uuids (id) values ('{uuid}');";
                        else sql += $"insert into _uuids (id) values (null);";
                    }

                    sql += "select [b].[id] as [requested_id], count(*) as [count] from [trakhound_objects] as [a]";
                    sql += " inner join _uuids as [b] on [a].[parent_uuid] = [b].[id]";
                    sql += " group by [a].[parent_uuid];";

                    var responses = _client.ReadList<DatabaseEntityCount>(GetReadConnectionString(), sql);
                    if (!responses.IsNullOrEmpty())
                    {
                        foreach (var rootUuid in uuids)
                        {
                            var parentResponses = responses.Where(o => o.RequestedId == rootUuid);
                            if (!parentResponses.IsNullOrEmpty())
                            {
                                foreach (var response in parentResponses)
                                {
                                    var count = new TrakHoundCount(response.RequestedId, response.Count);

                                    results.Add(new TrakHoundResult<TrakHoundCount>(Id, rootUuid, TrakHoundResultType.Ok, count));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<TrakHoundCount>(Id, rootUuid, TrakHoundResultType.Empty));
                            }
                        }
                    }
                    else
                    {
                        foreach (var rootUuid in uuids)
                        {
                            results.Add(new TrakHoundResult<TrakHoundCount>(Id, rootUuid, TrakHoundResultType.Empty));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<TrakHoundCount>.BadRequest(Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<TrakHoundCount>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundCount>(results, stpw.ElapsedTicks);
        }


        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectEntity> entities)
        {
            var builder = new StringBuilder();
            foreach (var entity in entities)
            {
                builder.Append($"insert into {TableName}");
                builder.Append(' ');
                builder.Append("([uuid], [namespace], [path], [name], [parent_uuid], [content_type], [definition_uuid], [priority], [source_uuid], [created])");
                builder.Append(' ');
                builder.Append("values");
                builder.Append(' ');
                builder.Append($"({FormatValue(entity.Uuid)}, {FormatValue(entity.Namespace)}, {FormatValue(entity.Path)}, {FormatValue(entity.Name)}, {FormatValue(entity.ParentUuid)}, {FormatValue(entity.ContentType)}, {FormatValue(entity.DefinitionUuid)}, {FormatValue(entity.Priority)}, {FormatValue(entity.SourceUuid)}, {FormatValue(entity.Created)})");
                builder.Append(' ');
                builder.Append("on conflict ([uuid]) do update set");
                builder.Append(' ');
                builder.Append($"[content_type] = {FormatValue(entity.ContentType)}, [definition_uuid] = {FormatValue(entity.DefinitionUuid)}, [priority] = {FormatValue(entity.Priority)}, [source_uuid] = {FormatValue(entity.SourceUuid)}, [created] = {FormatValue(entity.Created)}");
                builder.Append(' ');
                builder.Append($"where [priority] <= {FormatValue(entity.Priority)}");
                builder.Append(';');
            }
            
            var query = builder.ToString();
            query = SqliteClient.CreateTableQuery<PublishItem>(TableName, new string[] { "uuid" }) + query;

            return _client.ExecuteNonQuery(GetWriteConnectionString(), query);
        }


        class IndexQueryResponse
        {
            public string Subject { get; set; }
        }

        public async Task<TrakHoundResponse<string>> QueryIndex(IEnumerable<EntityIndexRequest> requests, long skip, long take, SortOrder sortOrder)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<string>>();

            if (_client != null)
            {
                if (!requests.IsNullOrEmpty())
                {
                    foreach (var request in requests)
                    {
                        var sql = $"select [subject] from [trakhound_entity_index] where [target] = '{request.Target}' ";

                        switch (request.QueryType)
                        {
                            case TrakHoundIndexQueryType.Equal:
                                sql += $"and [value] = '{request.Value}'";
                                break;

                            case TrakHoundIndexQueryType.NotEqual:
                                sql += $"and [value] <> '{request.Value}'";
                                break;

                            case TrakHoundIndexQueryType.LessThan:
                                sql += $"and [value] < '{request.Value}'";
                                break;

                            case TrakHoundIndexQueryType.LessThanOrEqual:
                                sql += $"and [value] <= '{request.Value}'";
                                break;

                            case TrakHoundIndexQueryType.GreaterThan:
                                sql += $"and [value] > '{request.Value}'";
                                break;

                            case TrakHoundIndexQueryType.GreaterThanOrEqual:
                                sql += $"and [value] >= '{request.Value}'";
                                break;

                            case TrakHoundIndexQueryType.Like:
                                sql += $"and [value] like '{request.Value}'";
                                break;
                        }

                        var responses = _client.ReadList<IndexQueryResponse>(GetReadConnectionString(), sql);

                        // Set Results
                        if (!responses.IsNullOrEmpty())
                        {
                            foreach (var response in responses)
                            {
                                results.Add(new TrakHoundResult<string>(Id, "All", TrakHoundResultType.Ok, response.Subject));
                            }
                        }
                        else
                        {
                            results.Add(new TrakHoundResult<string>(Id, "All", TrakHoundResultType.NotFound));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<string>.BadRequest(Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<string>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<string>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<bool>> UpdateIndex(IEnumerable<EntityIndexPublishRequest> requests)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<bool>>();

            var items = new List<IndexPublishItem>();
            foreach (var request in requests)
            {
                var item = new IndexPublishItem();
                item.Uuid = request.Uuid;
                item.Target = request.Target;
                item.Value = request.Value;
                item.Subject = request.Subject;
                item.Order = request.Order;
                items.Add(item);
            }

            _client.Insert(GetWriteConnectionString(), items, "[trakhound_entity_index]", new string[] { "uuid" });

            foreach (var request in requests)
            {
                results.Add(new TrakHoundResult<bool>(Id, request.Uuid, TrakHoundResultType.Ok, true));
            }

            stpw.Stop();
            return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
        }


        public async Task<TrakHoundResponse<bool>> DeleteByRootUuid(IEnumerable<string> rootUuids)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<bool>>();

            if (_client != null)
            {
                if (!rootUuids.IsNullOrEmpty())
                {
                    var sql = "";
                    sql += "drop table if exists _uuids;";
                    sql += "create temp table _uuids(id text);";

                    foreach (var uuid in rootUuids)
                    {
                        if (!string.IsNullOrEmpty(uuid)) sql += $"insert into _uuids (id) values ('{uuid}');";
                        else sql += $"insert into _uuids (id) values (null);";
                    }

                    sql += $"delete from [trakhound_objects] where [uuid] in";
                    sql += $" (select [a].[uuid] from [trakhound_objects] as [a] inner join (select [path] from [trakhound_objects] as [a] inner join _uuids as [b] on [b].[id] = [a].[uuid]) as [b] on [a].[path] like trim([b].[path]) || '/%');";

                    if (_client.ExecuteNonQuery(GetWriteConnectionString(), sql))
                    {
                        foreach (var rootUuid in rootUuids)
                        {
                            results.Add(new TrakHoundResult<bool>(Id, rootUuid, TrakHoundResultType.Ok, true));
                        }
                    }
                    else
                    {
                        foreach (var rootUuid in rootUuids)
                        {
                            results.Add(new TrakHoundResult<bool>(Id, rootUuid, TrakHoundResultType.InternalError));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<bool>.BadRequest(Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<bool>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
        }


        private IEnumerable<DatabaseStructureHierarchy> GetObjectRootIds(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var sqlQuery = "";
                sqlQuery += "drop table if exists _uuids;";
                sqlQuery += "create temp table _uuids(id TEXT);";

                foreach (var uuid in uuids)
                {
                    sqlQuery += $"insert into _uuids(id) values('{uuid}');";
                }

                sqlQuery += GetScript("GetObjectRoots");

                return _client.ReadList<DatabaseStructureHierarchy>(GetReadConnectionString(), sqlQuery);
            }

            return null;
        }

        private IEnumerable<DatabaseStructureHierarchy> GetObjectChildIds(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var sqlQuery = "";
                sqlQuery += "drop table if exists _uuids;";
                sqlQuery += "create temp table _uuids(id TEXT);";

                foreach (var uuid in uuids)
                {
                    sqlQuery += $"insert into _uuids(id) values('{uuid}');";
                }

                sqlQuery += GetScript("GetObjectChildren");

                return _client.ReadList<DatabaseStructureHierarchy>(GetReadConnectionString(), sqlQuery);
            }

            return null;
        }


        private string GetScript(string scriptName)
        {
            if (!string.IsNullOrEmpty(scriptName))
            {
                string script;
                lock (_lock) script = _scripts.GetValueOrDefault(scriptName);
                if (script == null)
                {
                    var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    dir = Path.Combine(dir, "scripts");

                    var path = Path.Combine(dir, scriptName + ".sql");
                    if (File.Exists(path))
                    {
                        script =  File.ReadAllText(path);

                        lock (_lock)
                        {
                            _scripts.Remove(scriptName);
                            _scripts.Add(scriptName, script);
                        }
                    }
                }

                return script;
            }

            return null;
        }


        private static object FormatValue(object value)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(string))
                {
                    return $"'{value}'";
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return "null";
            }
        }
    }
}
