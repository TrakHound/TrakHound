// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Http
{
    public class TrakHoundHttpTimeRangeSpanResponse
    {
        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("span")]
        public long Span { get; set; }


        public TrakHoundHttpTimeRangeSpanResponse() { }

        public TrakHoundHttpTimeRangeSpanResponse(string target, long span) 
        {
            Target = target;
            Span = span;
        }

        public TrakHoundHttpTimeRangeSpanResponse(TrakHoundTimeRangeSpan span)
        {
            Target = span.Target;
            Span = span.Span;
        }


        public TrakHoundTimeRangeSpan ToTimeRangeSpan() => new TrakHoundTimeRangeSpan(Target, Span);


        public static IEnumerable<TrakHoundHttpTimeRangeSpanResponse> Create(IEnumerable<string> targets, long span)
        {
            var requests = new List<TrakHoundHttpTimeRangeSpanResponse>();
            if (!targets.IsNullOrEmpty())
            {
                foreach (var target in targets)
                {
                    requests.Add(new TrakHoundHttpTimeRangeSpanResponse(target, span));
                }
            }

            return requests;
        }

        public static IEnumerable<TrakHoundHttpTimeRangeSpanResponse> Create(IEnumerable<TrakHoundTimeRangeSpan> spans)
        {
            var requests = new List<TrakHoundHttpTimeRangeSpanResponse>();
            if (!spans.IsNullOrEmpty())
            {
                foreach (var span in spans)
                {
                    requests.Add(new TrakHoundHttpTimeRangeSpanResponse(span.Target, span.Span));
                }
            }

            return requests;
        }
    }

    public static class TrakHoundHttpTimeRangeSpanQueryRequestExtensions
    {
        public static IEnumerable<TrakHoundTimeRangeSpan> ToRangeSpanQueries(this IEnumerable<TrakHoundHttpTimeRangeSpanResponse> requests)
        {
            var queries = new List<TrakHoundTimeRangeSpan>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    queries.Add(request.ToTimeRangeSpan());
                }
            }

            return queries;
        }
    }
}
