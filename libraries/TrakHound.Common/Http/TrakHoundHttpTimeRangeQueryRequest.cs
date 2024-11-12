// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Http
{
    public class TrakHoundHttpTimeRangeQueryRequest
    {
        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("start")]
        public long Start { get; set; }

        [JsonPropertyName("stop")]
        public long Stop { get; set; }

        [JsonPropertyName("span")]
        public long Span { get; set; }


        public TrakHoundHttpTimeRangeQueryRequest() { }

        public TrakHoundHttpTimeRangeQueryRequest(string target, long start, long stop, long span) 
        {
            Target = target;
            Start = start;
            Stop = stop;
            Span = span;
        }

        public TrakHoundHttpTimeRangeQueryRequest(TrakHoundTimeRangeQuery query)
        {
            Target = query.Target;
            Start = query.From;
            Stop = query.To;
            Span = query.Span;
        }


        public TrakHoundTimeRangeQuery ToTimeRangeQuery() => new TrakHoundTimeRangeQuery(Target, Start, Stop, Span);


        public static IEnumerable<TrakHoundHttpTimeRangeQueryRequest> Create(IEnumerable<string> targets, long start, long stop, long span)
        {
            var requests = new List<TrakHoundHttpTimeRangeQueryRequest>();
            if (!targets.IsNullOrEmpty())
            {
                foreach (var target in targets)
                {
                    requests.Add(new TrakHoundHttpTimeRangeQueryRequest(target, start, stop, span));
                }
            }

            return requests;
        }

        public static IEnumerable<TrakHoundHttpTimeRangeQueryRequest> Create(IEnumerable<TrakHoundTimeRangeQuery> queries)
        {
            var requests = new List<TrakHoundHttpTimeRangeQueryRequest>();
            if (!queries.IsNullOrEmpty())
            {
                foreach (var query in queries)
                {
                    requests.Add(new TrakHoundHttpTimeRangeQueryRequest(query.Target, query.From, query.To, query.Span));
                }
            }

            return requests;
        }
    }

    public static class TrakHoundHttpTimeRangeQueryRequestExtensions
    {
        public static IEnumerable<TrakHoundTimeRangeQuery> ToRangeQueries(this IEnumerable<TrakHoundHttpTimeRangeQueryRequest> requests)
        {
            var queries = new List<TrakHoundTimeRangeQuery>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    queries.Add(request.ToTimeRangeQuery());
                }
            }

            return queries;
        }
    }
}
