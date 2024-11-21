// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Http
{
    public class TrakHoundHttpAggregateResponse
    {
        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("aggregate")]
        public double Value { get; set; }


        public TrakHoundHttpAggregateResponse() { }

        public TrakHoundHttpAggregateResponse(string target, double count) 
        {
            Target = target;
            Value = count;
        }

        public TrakHoundHttpAggregateResponse(TrakHoundAggregate aggregate)
        {
            Target = aggregate.Target;
            Value = aggregate.Value;
        }


        public TrakHoundAggregate ToAggregate() => new TrakHoundAggregate(Target, Value);


        public static IEnumerable<TrakHoundHttpAggregateResponse> Create(IEnumerable<TrakHoundAggregate> counts)
        {
            var requests = new List<TrakHoundHttpAggregateResponse>();
            if (!counts.IsNullOrEmpty())
            {
                foreach (var count in counts)
                {
                    requests.Add(new TrakHoundHttpAggregateResponse(count));
                }
            }

            return requests;
        }
    }

    public static class TrakHoundHttpEntityAggregateResponseExtensions
    {
        public static IEnumerable<TrakHoundAggregate> ToAggregates(this IEnumerable<TrakHoundHttpAggregateResponse> requests)
        {
            var queries = new List<TrakHoundAggregate>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    queries.Add(request.ToAggregate());
                }
            }

            return queries;
        }
    }
}
