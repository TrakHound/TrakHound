// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Http
{
    public class TrakHoundHttpRangeQueryRequest
    {
        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("start")]
        public long Start { get; set; }

        [JsonPropertyName("stop")]
        public long Stop { get; set; }


        public TrakHoundHttpRangeQueryRequest() { }

        public TrakHoundHttpRangeQueryRequest(string target, long start, long stop) 
        {
            Target = target;
            Start = start;
            Stop = stop;
        }

        public TrakHoundHttpRangeQueryRequest(TrakHoundRangeQuery query)
        {
            Target = query.Target;
            Start = query.From;
            Stop = query.To;
        }


        public TrakHoundRangeQuery ToRangeQuery() => new TrakHoundRangeQuery(Target, Start, Stop);


        public static IEnumerable<TrakHoundHttpRangeQueryRequest> Create(IEnumerable<string> targets, long start, long stop)
        {
            var requests = new List<TrakHoundHttpRangeQueryRequest>();
            if (!targets.IsNullOrEmpty())
            {
                foreach (var target in targets)
                {
                    requests.Add(new TrakHoundHttpRangeQueryRequest(target, start, stop));
                }
            }

            return requests;
        }

        public static IEnumerable<TrakHoundHttpRangeQueryRequest> Create(IEnumerable<TrakHoundRangeQuery> queries)
        {
            var requests = new List<TrakHoundHttpRangeQueryRequest>();
            if (!queries.IsNullOrEmpty())
            {
                foreach (var query in queries)
                {
                    requests.Add(new TrakHoundHttpRangeQueryRequest(query.Target, query.From, query.To));
                }
            }

            return requests;
        }
    }

    public static class TrakHoundHttpRangeQueryRequestExtensions
    {
        public static IEnumerable<TrakHoundRangeQuery> ToRangeQueries(this IEnumerable<TrakHoundHttpRangeQueryRequest> requests)
        {
            var queries = new List<TrakHoundRangeQuery>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    queries.Add(request.ToRangeQuery());
                }
            }

            return queries;
        }
    }
}
