// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Http
{
    public class TrakHoundHttpCountResponse
    {
        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("count")]
        public long Count { get; set; }


        public TrakHoundHttpCountResponse() { }

        public TrakHoundHttpCountResponse(string target, long count) 
        {
            Target = target;
            Count = count;
        }

        public TrakHoundHttpCountResponse(TrakHoundCount count)
        {
            Target = count.Target;
            Count = count.Count;
        }


        public TrakHoundCount ToCount() => new TrakHoundCount(Target, Count);


        public static IEnumerable<TrakHoundHttpCountResponse> Create(IEnumerable<TrakHoundCount> counts)
        {
            var requests = new List<TrakHoundHttpCountResponse>();
            if (!counts.IsNullOrEmpty())
            {
                foreach (var count in counts)
                {
                    requests.Add(new TrakHoundHttpCountResponse(count));
                }
            }

            return requests;
        }
    }

    public static class TrakHoundHttpEntityCountResponseExtensions
    {
        public static IEnumerable<TrakHoundCount> ToCounts(this IEnumerable<TrakHoundHttpCountResponse> requests)
        {
            var queries = new List<TrakHoundCount>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    queries.Add(request.ToCount());
                }
            }

            return queries;
        }
    }
}
