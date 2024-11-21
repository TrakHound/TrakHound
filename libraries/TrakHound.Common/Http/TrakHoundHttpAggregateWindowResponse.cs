// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Http
{
    public class TrakHoundHttpAggregateWindowResponse
    {
        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("start")]
        public long Start { get; set; }

        [JsonPropertyName("end")]
        public long End { get; set; }

        [JsonPropertyName("aggregate")]
        public double Value { get; set; }


        public TrakHoundHttpAggregateWindowResponse() { }

        public TrakHoundHttpAggregateWindowResponse(string target, long start, long end, double count) 
        {
            Target = target;
            Start = start;
            End = end;
            Value = count;
        }

        public TrakHoundHttpAggregateWindowResponse(TrakHoundAggregateWindow aggregate)
        {
            Target = aggregate.Target;
            Start = aggregate.Start;
            End = aggregate.End;
            Value = aggregate.Value;
        }


        public TrakHoundAggregateWindow ToAggregateWindow() => new TrakHoundAggregateWindow(Target, Start, End, Value);


        public static IEnumerable<TrakHoundHttpAggregateWindowResponse> Create(IEnumerable<TrakHoundAggregateWindow> counts)
        {
            var requests = new List<TrakHoundHttpAggregateWindowResponse>();
            if (!counts.IsNullOrEmpty())
            {
                foreach (var count in counts)
                {
                    requests.Add(new TrakHoundHttpAggregateWindowResponse(count));
                }
            }

            return requests;
        }
    }

    public static class TrakHoundHttpEntityAggregateWindowResponseExtensions
    {
        public static IEnumerable<TrakHoundAggregateWindow> ToAggregateWindows(this IEnumerable<TrakHoundHttpAggregateWindowResponse> requests)
        {
            var queries = new List<TrakHoundAggregateWindow>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    queries.Add(request.ToAggregateWindow());
                }
            }

            return queries;
        }
    }
}
