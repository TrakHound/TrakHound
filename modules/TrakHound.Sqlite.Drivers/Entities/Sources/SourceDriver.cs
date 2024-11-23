// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Sqlite.Drivers.Models;

namespace TrakHound.Sqlite.Drivers
{
    public class SourceDriver : SqliteEntityDriver<ITrakHoundSourceEntity, DatabaseSource>,
        ISourceQueryDriver
    {
        private readonly Dictionary<string, string> _scripts = new Dictionary<string, string>();
        private readonly object _lock = new object();


        public SourceDriver() { }

        public SourceDriver(ITrakHoundDriverConfiguration configuration) : base(configuration)
        {
            EntityName = "trakhound_sources";
            TableColumnList = new List<string> {
                "[type]",
                "[sender]",
                "[parent_uuid]",
                "[created]"
            };
        }

        class PublishItem
        {
            public string Uuid { get; set; }
            public string Type { get; set; }
            public string Sender { get; set; }
            public string ParentUuid { get; set; }
            public long Created { get; set; }
        }

      
        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundSourceEntity> entities)
        {
            var items = new List<PublishItem>();
            foreach (var entity in entities)
            {
                var item = new PublishItem();
                item.Uuid = entity.Uuid;
                item.Type = entity.Type;
                item.Sender = entity.Sender;
                item.ParentUuid = entity.ParentUuid;
                item.Created = entity.Created;
                items.Add(item);
            }

            _client.Insert(GetWriteConnectionString(), items, TableName, new string[] { "uuid" });

            return true;
        }


        public async Task<TrakHoundResponse<ITrakHoundSourceQueryResult>> QueryUuidChain(IEnumerable<string> uuids)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundSourceQueryResult>>();

            if (_client != null)
            {
                var responses = GetSourceChain(uuids);
                if (!responses.IsNullOrEmpty())
                {
                    var dResponses = responses.ToListDictionary(o => o.RequestedId);
                    foreach (var uuid in uuids)
                    {
                        var uuidResponses = dResponses.Get(uuid);
                        if (!uuidResponses.IsNullOrEmpty())
                        {
                            foreach (var response in uuidResponses)
                            {
                                var queryResult = new TrakHoundSourceQueryResult(TrakHoundSourceQueryRequestType.Uuid, response.RequestedId, response.Uuid);

                                results.Add(new TrakHoundResult<ITrakHoundSourceQueryResult>(Id, uuid, TrakHoundResultType.Ok, queryResult, queryResult.Uuid));
                            }
                        }
                        else
                        {
                            results.Add(new TrakHoundResult<ITrakHoundSourceQueryResult>(Id, uuid, TrakHoundResultType.NotFound));
                        }
                    }
                }
                else
                {
                    foreach (var uuid in uuids)
                    {
                        results.Add(new TrakHoundResult<ITrakHoundSourceQueryResult>(Id, uuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(TrakHoundResult<ITrakHoundSourceQueryResult>.InternalError(Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundSourceQueryResult>(results, stpw.ElapsedTicks);
        }


        private IEnumerable<DatabaseStructureHierarchy> GetSourceChain(IEnumerable<string> uuids)
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

                sqlQuery += GetScript("GetSourceChain");

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
                        script = File.ReadAllText(path);

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
