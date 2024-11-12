// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TrakHound.Utilities
{
    public partial class SqliteClient
    {
        public static SqliteParameter CreateParameter(string name, object value)
        {
            return new SqliteParameter(name, value ?? DBNull.Value);
        }

        //public static SqliteParameter CreateIdListParameter(string parameterName, string id)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("id", typeof(string)));
        //    if (!string.IsNullOrEmpty(id)) dt.Rows.Add(id);

        //    var p = new SqliteParameter(parameterName, dt);
        //    p.TypeName = "dbo.TrakHoundIdList";
        //    return p;
        //}

        //public static SqliteParameter CreateIdListParameter(string parameterName, IEnumerable<string> ids)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("id", typeof(string)));
        //    if (!ids.IsNullOrEmpty())
        //    {
        //        foreach (var id in ids.ToList())
        //        {
        //            if (!string.IsNullOrEmpty(id)) dt.Rows.Add(id);
        //        }
        //    }

        //    var p = new SqliteParameter(parameterName, dt);
        //    p. = "dbo.TrakHoundIdList";
        //    return p;
        //}

        //public static SqliteParameter CreateIdListParameter(string parameterName, int id)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("id", typeof(string)));
        //    dt.Rows.Add(id);

        //    var p = new SqlParameter(parameterName, dt);
        //    p.TypeName = "dbo.TrakHoundIdList";
        //    return p;
        //}

        //public static SqlParameter CreateIdListParameter(string parameterName, IEnumerable<int> ids)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("id", typeof(string)));
        //    if (!ids.IsNullOrEmpty())
        //    {
        //        foreach (var id in ids.ToList())
        //        {
        //            dt.Rows.Add(id);
        //        }
        //    }

        //    var p = new SqlParameter(parameterName, dt);
        //    p.TypeName = "dbo.TrakHoundIdList";
        //    return p;
        //}


        //public static SqlParameter CreateRowListParameter(string parameterName, long row)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("row", typeof(long)));
        //    dt.Rows.Add(row);

        //    var p = new SqlParameter(parameterName, dt);
        //    p.TypeName = "dbo.RowList";
        //    return p;
        //}

        //public static SqlParameter CreateRowListParameter(string parameterName, IEnumerable<long> rows)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("row", typeof(long)));
        //    if (!rows.IsNullOrEmpty())
        //    {
        //        foreach (var row in rows.ToList())
        //        {
        //            dt.Rows.Add(row);
        //        }
        //    }

        //    var p = new SqlParameter(parameterName, dt);
        //    p.TypeName = "dbo.RowList";
        //    return p;
        //}



        //public static SqlParameter CreateRangeQueryParameter(string parameterName, TrakHoundRangeQuery query)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("target", typeof(string)));
        //    dt.Columns.Add(new DataColumn("from", typeof(long)));
        //    dt.Columns.Add(new DataColumn("to", typeof(long)));
        //    dt.Rows.Add(query.Target, query.From, query.To);

        //    var p = new SqlParameter(parameterName, dt);
        //    p.TypeName = "dbo.TrakHoundRangeQueryList";
        //    return p;
        //}

        //public static SqlParameter CreateRangeQueryParameter(string parameterName, IEnumerable<TrakHoundRangeQuery> queries)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("target", typeof(string)));
        //    dt.Columns.Add(new DataColumn("from", typeof(long)));
        //    dt.Columns.Add(new DataColumn("to", typeof(long)));
        //    if (!queries.IsNullOrEmpty())
        //    {
        //        foreach (var query in queries.ToList())
        //        {
        //            dt.Rows.Add(query.Target, query.From, query.To);
        //        }
        //    }

        //    var p = new SqlParameter(parameterName, dt);
        //    p.TypeName = "dbo.TrakHoundRangeQueryList";
        //    return p;
        //}

        //public static SqlParameter CreateTimeRangeQueryParameter(string parameterName, IEnumerable<TrakHoundTimeRangeQuery> queries)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("target", typeof(string)));
        //    dt.Columns.Add(new DataColumn("time_range_id", typeof(string)));
        //    if (!queries.IsNullOrEmpty())
        //    {
        //        foreach (var query in queries.ToList())
        //        {
        //            if (!query.TimeRangeIds.IsNullOrEmpty())
        //            {
        //                foreach (var timeRangeId in query.TimeRangeIds)
        //                {
        //                    dt.Rows.Add(query.Target, timeRangeId);
        //                }
        //            }
        //        }
        //    }

        //    var p = new SqlParameter(parameterName, dt);
        //    p.TypeName = "dbo.TrakHoundTimeRangeQueryList";
        //    return p;
        //}



        //public static SqlParameter CreateDeleteRequestListParameter(string parameterName, string id, long timestamp)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("entityId", typeof(string)));
        //    dt.Columns.Add(new DataColumn("timestamp", typeof(long)));
        //    if (!string.IsNullOrEmpty(id)) dt.Rows.Add(id, timestamp);

        //    var p = new SqlParameter(parameterName, dt);
        //    p.TypeName = "dbo.TrakHoundEntityDeleteRequestList";
        //    return p;
        //}

        //public static SqlParameter CreateDeleteRequestListParameter(string parameterName, EntityDeleteRequest request)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("entityId", typeof(string)));
        //    dt.Columns.Add(new DataColumn("timestamp", typeof(long)));
        //    dt.Rows.Add(request.EntityId, request.Timestamp);

        //    var p = new SqlParameter(parameterName, dt);
        //    p.TypeName = "dbo.TrakHoundEntityDeleteRequestList";
        //    return p;
        //}

        //public static SqlParameter CreateDeleteRequestListParameter(string parameterName, IEnumerable<EntityDeleteRequest> requests)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("entityId", typeof(string)));
        //    dt.Columns.Add(new DataColumn("timestamp", typeof(long)));
        //    if (!requests.IsNullOrEmpty())
        //    {
        //        foreach (var request in requests.ToList())
        //        {
        //            dt.Rows.Add(request.EntityId, request.Timestamp);
        //        }
        //    }

        //    var p = new SqlParameter(parameterName, dt);
        //    p.TypeName = "dbo.TrakHoundEntityDeleteRequestList";
        //    return p;
        //}

        //public static SqlParameter CreateSearchListParameter(string parameterName, IEnumerable<SearchParameter> searchParameters)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("id", typeof(string)));
        //    dt.Columns.Add(new DataColumn("from", typeof(long)));
        //    dt.Columns.Add(new DataColumn("to", typeof(long)));
        //    dt.Columns.Add(new DataColumn("tag", typeof(string)));
        //    if (!searchParameters.IsNullOrEmpty())
        //        foreach (var searchParameter in searchParameters)
        //            dt.Rows.Add(searchParameter.Id, searchParameter.From, searchParameter.To, searchParameter.Tag);

        //    var p = new SqlParameter(parameterName, dt);
        //    p.TypeName = "dbo.SearchList";
        //    return p;
        //}
    }
}
