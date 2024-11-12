// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Sqlite.Drivers.Models;

namespace TrakHound.Sqlite.Drivers
{
    public abstract class SqliteEntityDriver<TEntity, TDatabaseEntity> :
        SqliteDriver,
        IEntityReadDriver<TEntity>,
        IEntityPublishDriver<TEntity>,
        IEntityDeleteDriver<TEntity>,
        IEntityEmptyDriver<TEntity>
        where TEntity : ITrakHoundEntity
        where TDatabaseEntity : IDatabaseEntity<TEntity>
    {
        protected string EntityName { get; set; }

        public string TableName => $"[{EntityName}]";

        protected IEnumerable<string> TableColumnList { get; set; }

        protected string TableColumns => string.Join(", ", TableColumnList);


        public enum QueryType
        {
            Id,
            Assignee,
            Object
        }


        public SqliteEntityDriver() { }

        public SqliteEntityDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        #region "Internal"

        public static async Task<TrakHoundResponse<TEntity>> ProcessResponse(
            SqliteDriver driver,
            string requestedId,
            Func<IEnumerable<string>, Task<IEnumerable<TDatabaseEntity>>> readFunction,
            QueryType queryType
            )
        {
            var requestedIds = new List<string>();
            if (requestedId != null) requestedIds.Add(requestedId);

            return await ProcessResponse(driver, requestedIds, readFunction, queryType);
        }

        public static async Task<TrakHoundResponse<TEntity>> ProcessResponse(
            SqliteDriver driver,
            IEnumerable<string> requestedIds,
            Func<IEnumerable<string>, Task<IEnumerable<TDatabaseEntity>>> readFunction,
            QueryType queryType
            )
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<TEntity>>();

            if (driver != null)
            {
                if (!requestedIds.IsNullOrEmpty())
                {
                    if (driver.IsAvailable)
                    {
                        var notFound = new List<string>();

                        // Get Responses
                        var responses = await readFunction(requestedIds);

                        // Set Results
                        if (!responses.IsNullOrEmpty())
                        {
                            foreach (var requestedId in requestedIds)
                            {
                                var idResponses = responses.Where(o => o.RequestedId == requestedId);
                                if (!idResponses.IsNullOrEmpty())
                                {
                                    foreach (var response in idResponses)
                                    {
                                        results.Add(new TrakHoundResult<TEntity>(driver.Id, requestedId, TrakHoundResultType.Ok, response.ToEntity()));
                                    }
                                }
                                else
                                {
                                    notFound.Add(requestedId);
                                }
                            }
                        }
                        else
                        {
                            notFound.AddRange(requestedIds);
                        }

                        if (!notFound.IsNullOrEmpty())
                        {
                            if (queryType == QueryType.Object)
                            {
                                foreach (var requestedId in notFound)
                                {
                                    results.Add(new TrakHoundResult<TEntity>(driver.Id, requestedId, TrakHoundResultType.Empty));
                                }



                                //// Check for Empty Entities
                                //IEnumerable<DatabaseEntityComponentCount> empties = null;
                                //switch (queryType)
                                //{
                                //    case QueryType.Object: empties = await ReadEntities(driver, notFound); break;
                                //}

                                //foreach (var requestedId in notFound)
                                //{
                                //    var empty = empties?.FirstOrDefault(o => o.AssemblyId == requestedId);
                                //    if (empty != null && empty.Count < 1)
                                //    {
                                //        results.Add(new TrakHoundResult<TEntity>(driver.Id, requestedId, TrakHoundResultType.Empty));
                                //    }
                                //    else
                                //    {
                                //        results.Add(new TrakHoundResult<TEntity>(driver.Id, requestedId, TrakHoundResultType.NotFound));
                                //    }
                                //}
                            }
                            else
                            {
                                foreach (var requestedId in notFound)
                                {
                                    results.Add(new TrakHoundResult<TEntity>(driver.Id, requestedId, TrakHoundResultType.NotFound));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var requestedId in requestedIds)
                        {
                            results.Add(new TrakHoundResult<TEntity>(driver.Id, requestedId, TrakHoundResultType.NotAvailable));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<TEntity>.BadRequest(driver.Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<TEntity>.InternalError(driver.Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<TEntity>(results, stpw.ElapsedTicks);
        }


        public static async Task<TrakHoundResponse<TrakHoundCount>> ProcessResponse(SqliteDriver driver, string requestedId, Func<IEnumerable<string>, Task<IEnumerable<DatabaseEntityCount>>> readFunction)
        {
            var requestedIds = new List<string>();
            if (requestedId != null) requestedIds.Add(requestedId);

            return await ProcessResponse(driver, requestedIds, readFunction);
        }

        public static async Task<TrakHoundResponse<TrakHoundCount>> ProcessResponse(SqliteDriver driver, IEnumerable<string> requestedIds, Func<IEnumerable<string>, Task<IEnumerable<DatabaseEntityCount>>> readFunction)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<TrakHoundCount>>();

            if (driver != null)
            {
                if (!requestedIds.IsNullOrEmpty())
                {
                    if (driver.IsAvailable)
                    {
                        // Get Responses
                        var responses = await readFunction(requestedIds);

                        // Set Results
                        if (!responses.IsNullOrEmpty())
                        {
                            foreach (var requestedId in requestedIds)
                            {
                                var idResponses = responses.Where(o => o.RequestedId == requestedId);
                                if (!idResponses.IsNullOrEmpty())
                                {
                                    foreach (var response in idResponses)
                                    {
                                        results.Add(new TrakHoundResult<TrakHoundCount>(driver.Id, requestedId, TrakHoundResultType.Ok, new TrakHoundCount(response.RequestedId, response.Count)));
                                    }
                                }
                                else
                                {
                                    results.Add(new TrakHoundResult<TrakHoundCount>(driver.Id, requestedId, TrakHoundResultType.NotFound));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var requestedId in requestedIds)
                        {
                            results.Add(new TrakHoundResult<TrakHoundCount>(driver.Id, requestedId, TrakHoundResultType.NotAvailable));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<TrakHoundCount>.BadRequest(driver.Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<TrakHoundCount>.InternalError(driver.Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundCount>(results, stpw.ElapsedTicks);
        }


        public static async Task<TrakHoundResponse<TrakHoundTimeRangeSpan>> ProcessResponse(SqliteDriver driver, string requestedId, Func<IEnumerable<string>, Task<IEnumerable<DatabaseTimeRangeSpan>>> readFunction)
        {
            var requestedIds = new List<string>();
            if (requestedId != null) requestedIds.Add(requestedId);

            return await ProcessResponse(driver, requestedIds, readFunction);
        }

        public static async Task<TrakHoundResponse<TrakHoundTimeRangeSpan>> ProcessResponse(SqliteDriver driver, IEnumerable<string> requestedIds, Func<IEnumerable<string>, Task<IEnumerable<DatabaseTimeRangeSpan>>> readFunction)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<TrakHoundTimeRangeSpan>>();

            if (driver != null)
            {
                if (!requestedIds.IsNullOrEmpty())
                {
                    if (driver.IsAvailable)
                    {
                        // Get Responses
                        var responses = await readFunction(requestedIds);

                        // Set Results
                        if (!responses.IsNullOrEmpty())
                        {
                            foreach (var requestedId in requestedIds)
                            {
                                var idResponses = responses.Where(o => o.RequestedId == requestedId);
                                if (!idResponses.IsNullOrEmpty())
                                {
                                    foreach (var response in idResponses)
                                    {
                                        results.Add(new TrakHoundResult<TrakHoundTimeRangeSpan>(driver.Id, requestedId, TrakHoundResultType.Ok, new TrakHoundTimeRangeSpan(response.RequestedId, response.Span)));
                                    }
                                }
                                else
                                {
                                    results.Add(new TrakHoundResult<TrakHoundTimeRangeSpan>(driver.Id, requestedId, TrakHoundResultType.NotFound));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var requestedId in requestedIds)
                        {
                            results.Add(new TrakHoundResult<TrakHoundTimeRangeSpan>(driver.Id, requestedId, TrakHoundResultType.NotAvailable));
                        }
                    }
                }
                else
                {
                    results.Add(TrakHoundResult<TrakHoundTimeRangeSpan>.BadRequest(driver.Id));
                }
            }
            else
            {
                results.Add(TrakHoundResult<TrakHoundTimeRangeSpan>.InternalError(driver.Id));
            }

            stpw.Stop();
            return new TrakHoundResponse<TrakHoundTimeRangeSpan>(results, stpw.ElapsedTicks);
        }


        //public static async Task<bool> InitializeEntityRecords(SqlServerClient client, IEnumerable<ITrakHoundEntityComponent> entities)
        //{
        //    if (client != null && !entities.IsNullOrEmpty())
        //    {
        //        var assemblyIds = entities.Select(o => ITrakHoundEntityComponent.GetAssemblyId(o));

        //        var table = PrepareObjects(assemblyIds);
        //        var i = await client.WriteBulk(table, "trakhound_entities_create_temp", "trakhound_entities_merge_temp", "trakhound_drop_temp_table");
        //        return i >= 0;
        //    }

        //    return false;
        //}

        #endregion


        #region "Read"

        protected async virtual Task<IEnumerable<TDatabaseEntity>> OnRead(IEnumerable<string> uuids)
        {
            var sql = "";
            sql += "drop table if exists _uuids;";
            sql += "create temp table _uuids(target text);";

            foreach (var uuid in uuids)
            {
                if (!string.IsNullOrEmpty(uuid)) sql += $"insert into _uuids (target) values ('{uuid}');";
            }

            sql += $"select [b].[target] as [requested_id], {TableColumns} from {TableName} as [a]";
            sql += " inner join _uuids as [b] on [a].[uuid] = [b].[target];";

            return _client.ReadList<TDatabaseEntity>(sql);
        }



        public async Task<TrakHoundResponse<TEntity>> Read(long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending)
        {
            return TrakHoundResponse<TEntity>.InternalError(null, Id);
        }

        public async Task<TrakHoundResponse<TEntity>> Read(IEnumerable<string> ids)
        {
            Func<IEnumerable<string>, Task<IEnumerable<TDatabaseEntity>>> readFunction = async (ids) =>
            {
                return await OnRead(ids);
            };

            return await ProcessResponse(this, ids, readFunction, QueryType.Id);
        }

        #endregion

        #region "Publish"

        protected virtual async Task<bool> OnPublish(IEnumerable<TEntity> entities)
        {
            _client.Insert(entities, TableName);

            return true;
        }

        protected virtual async Task<bool> OnPublishAfter(IEnumerable<TEntity> entities)
        {
            return true;
        }

        public virtual async Task<TrakHoundResponse<TrakHoundPublishResult<TEntity>>> Publish(IEnumerable<TEntity> entities)
        {
            return await PublishItems(entities);
        }


        private async Task<TrakHoundResponse<TrakHoundPublishResult<TEntity>>> PublishItems(IEnumerable<TEntity> entities)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<TrakHoundPublishResult<TEntity>>>();

            if (_client != null && !entities.IsNullOrEmpty())
            {
                await OnPublish(entities);

                await OnPublishAfter(entities);

                foreach (var entity in entities)
                {
                    var publishResult = new TrakHoundPublishResult<TEntity>(TrakHoundPublishResultType.Created, entity);
                    results.Add(new TrakHoundResult<TrakHoundPublishResult<TEntity>>(Id, entity.Uuid, TrakHoundResultType.Ok, publishResult));
                }
            }

            stpw.Stop();

            return new TrakHoundResponse<TrakHoundPublishResult<TEntity>>(results, stpw.ElapsedTicks);
        }

        #endregion

        #region "Delete"

        protected virtual async Task<bool> OnDelete(IEnumerable<EntityDeleteRequest> requests)
        {
            var conditions = new List<string>();
            foreach (var request in requests)
            {
                conditions.Add($"[uuid] = '{request.Target}'");
            }
            var condition = string.Join(" or ", conditions);

            var query = $"delete from {TableName} where {condition};";
            return _client.ExecuteNonQuery(query);
        }

        protected virtual async Task<bool> OnDeleteAfter(IEnumerable<EntityDeleteRequest> requests)
        {
            return true;
        }

        public virtual async Task<TrakHoundResponse<bool>> Delete(IEnumerable<EntityDeleteRequest> requests)
        {
            return await DeleteEntities(requests);
        }


        private async Task<TrakHoundResponse<bool>> DeleteEntities(IEnumerable<EntityDeleteRequest> requests)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<bool>>();

            if (_client != null && !requests.IsNullOrEmpty())
            {
                if (await OnDelete(requests))
                {
                    await OnDeleteAfter(requests);

                    foreach (var request in requests)
                    {
                        results.Add(new TrakHoundResult<bool>(Id, request.Target, TrakHoundResultType.Ok));
                    }
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
        }

        #endregion

        #region "Empty"

        protected async virtual Task<bool> OnEmpty(IEnumerable<EntityEmptyRequest> requests)
        {
            return true;
        }


        public async Task<TrakHoundResponse<bool>> Empty(EntityEmptyRequest request)
        {
            if (request.IsValid)
            {
                return await Empty(new List<EntityEmptyRequest> { request });
            }

            return TrakHoundResponse<bool>.InternalError(Id);
        }

        public async Task<TrakHoundResponse<bool>> Empty(IEnumerable<EntityEmptyRequest> requests)
        {
            return await EmptyEntities(requests);
        }


        private async Task<TrakHoundResponse<bool>> EmptyEntities(IEnumerable<EntityEmptyRequest> requests)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<bool>>();

            if (_client != null && !requests.IsNullOrEmpty())
            {
                if (await OnEmpty(requests))
                {
                    foreach (var request in requests)
                    {
                        results.Add(new TrakHoundResult<bool>(Id, request.EntityUuid, TrakHoundResultType.Ok));
                    }
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
        }

        #endregion

    }
}
