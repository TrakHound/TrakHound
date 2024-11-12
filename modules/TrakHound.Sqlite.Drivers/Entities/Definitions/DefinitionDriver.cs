// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Sqlite.Drivers.Models;

namespace TrakHound.Sqlite.Drivers
{
    public class DefinitionDriver : SqliteEntityDriver<ITrakHoundDefinitionEntity, DatabaseDefinition>,
        IDefinitionQueryDriver
    {
        private readonly Dictionary<string, string> _scripts = new Dictionary<string, string>();
        private readonly object _lock = new object();


        public DefinitionDriver() { }

        public DefinitionDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            EntityName = "trakhound_definitions";
            TableColumnList = new List<string> {
                "[uuid]",
                "[id]",
                "[type]",
                "[parent_uuid]",
                "[source_uuid]",
                "[created]"
            };
        }

        class PublishItem
        {
            public string Uuid { get; set; }
            public string Id { get; set; }
            public string Type { get; set; }
            public string ParentUuid { get; set; }
            public string SourceUuid { get; set; }
            public long Created { get; set; }      
        }


        public async Task<TrakHoundResponse<string>> Query(string pattern, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<string>>();

            if (_client != null && !string.IsNullOrEmpty(pattern))
            {
                var like = pattern.ToLower().Replace('*', '%');

                // Run SQL Query
                var sqlQuery = $"select [uuid] from [trakhound_definitions] where lower([id]) like '{like}'";

                var responses = _client.ReadList(sqlQuery);
                if (!responses.IsNullOrEmpty())
                {
                    foreach (var response in responses)
                    {
                        results.Add(new TrakHoundResult<string>(Id, pattern, TrakHoundResultType.Ok, response, response));
                    }
                }
                else
                {
                    results.Add(new TrakHoundResult<string>(Id, pattern, TrakHoundResultType.Empty));
                }
            }
            else
            {
                results.Add(TrakHoundResult<string>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<string>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundDefinitionQueryResult>>();

            if (_client != null && !types.IsNullOrEmpty())
            {
                var sqlQuery = "";

                // Add Queries
                sqlQuery += "drop table if exists _types;";
                sqlQuery += "create temp table _types(id text);";
                foreach (var type in types)
                {
                    sqlQuery += $"insert into _types (id) values ('{type}');";
                }

                // Run SQL Query
                sqlQuery += $"select [types].[id] as [query], [uuid] from [trakhound_definitions] as [definitions]";
                sqlQuery += $" inner join (select [id] from _types) as [types] on lower([definitions].[type]) = lower([types].[id]);";

                var responses = _client.ReadList<DatabaseQueryResult>(sqlQuery);
                if (!responses.IsNullOrEmpty())
                {
                    foreach (var type in types)
                    {
                        var typeResponses = responses.Where(o => o.Query?.ToLower() == type?.ToLower());
                        if (!typeResponses.IsNullOrEmpty())
                        {
                            foreach (var response in typeResponses)
                            {
                                var queryResult = new TrakHoundDefinitionQueryResult(type, response.Uuid);

                                results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, type, TrakHoundResultType.Ok, queryResult, response.Uuid));
                            }
                        }
                        else
                        {
                            results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, type, TrakHoundResultType.Empty));
                        }
                    }
                }
                else
                {
                    foreach (var type in types)
                    {
                        results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, type, TrakHoundResultType.Empty));
                    }
                }
            }
            else
            {
                results.Add(TrakHoundResult<ITrakHoundDefinitionQueryResult>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundDefinitionQueryResult>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundDefinitionQueryResult>>();

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

                    sql += "select [b].[id] as [query], [a].[uuid] from [trakhound_definitions] as [a]";
                    sql += " inner join _uuids as [b] on [a].[parent_uuid] = [b].[id] or ([b].[id] is null and [a].[parent_uuid] is null);";

                    var responses = _client.ReadList<DatabaseIdQuery>(sql);

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
                                    var queryResult = new TrakHoundDefinitionQueryResult(parentUuid, response.Uuid);

                                    results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, parentUuid, TrakHoundResultType.Ok, queryResult, response.Uuid));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, parentUuid, TrakHoundResultType.Empty));
                            }
                        }
                    }
                    else
                    {
                        foreach (var parentUuid in parentUuids)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, parentUuid, TrakHoundResultType.Empty));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<ITrakHoundDefinitionQueryResult>.BadRequest(Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<ITrakHoundDefinitionQueryResult>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundDefinitionQueryResult>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundDefinitionQueryResult>>();

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

                    sql += "select [b].[id] as [query], [a].[parent_uuid] from [trakhound_definitions] as [a]";
                    sql += " inner join _uuids as [b] on [a].[uuid] = [b].[id];";

                    var responses = _client.ReadList<DatabaseIdQuery>(sql);

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
                                    var queryResult = new TrakHoundDefinitionQueryResult(childUuid, response.Uuid);

                                    results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, childUuid, TrakHoundResultType.Ok, queryResult, response.Uuid));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, childUuid, TrakHoundResultType.Empty));
                            }
                        }
                    }
                    else
                    {
                        foreach (var parentUuid in childUuids)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, parentUuid, TrakHoundResultType.Empty));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<ITrakHoundDefinitionQueryResult>.BadRequest(Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<ITrakHoundDefinitionQueryResult>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundDefinitionQueryResult>(results, stpw.ElapsedTicks);
        }



        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundDefinitionEntity> entities)
        {
            var items = new List<PublishItem>();
            foreach (var entity in entities)
            {
                var item = new PublishItem();
                item.Uuid = entity.Uuid;
                item.Id = entity.Id;
                item.Type = entity.Type;
                item.ParentUuid = entity.ParentUuid;
                item.SourceUuid = entity.SourceUuid;
                item.Created = entity.Created;
                items.Add(item);
            }

            _client.Insert(items, TableName, new string[] { "uuid" });

            return true;
        }

        
        private IEnumerable<DatabaseStructureHierarchy> GetDefinitionTreeIds(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var responses = new List<DatabaseStructureHierarchy>();

                foreach (var uuid in uuids)
                {
                    var targetResponse = new DatabaseStructureHierarchy();
                    targetResponse.RequestedId = uuid;
                    targetResponse.Uuid = uuid;
                    targetResponse.TypeId = 0;
                    responses.Add(targetResponse);
                }

                var roots = GetDefinitionRootIds(uuids);
                if (!roots.IsNullOrEmpty()) responses.AddRange(roots.Where(o => o.TypeId != 0));

                var children = GetDefinitionChildIds(uuids);
                if (!children.IsNullOrEmpty()) responses.AddRange(children.Where(o => o.TypeId != 0));

                return responses;
            }

            return null;
        }

        private IEnumerable<DatabaseStructureHierarchy> GetDefinitionRootIds(IEnumerable<string> uuids)
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

                sqlQuery += GetScript("GetDefinitionRoots");

                return _client.ReadList<DatabaseStructureHierarchy>(sqlQuery);
            }

            return null;
        }

        private IEnumerable<DatabaseStructureHierarchy> GetDefinitionChildIds(IEnumerable<string> uuids)
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

                sqlQuery += GetScript("GetDefinitionChildren");

                return _client.ReadList<DatabaseStructureHierarchy>(sqlQuery);
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
    }
}
