// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Http
{
    public class TrakHoundHttpEntityIndexRequest
    {
        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("queryType")]
        public int QueryType { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public TrakHoundHttpEntityIndexRequest() { }

        public TrakHoundHttpEntityIndexRequest(EntityIndexRequest request)
        {
            Target = request.Target;
            QueryType = (int)request.QueryType;
            Value = request.Value;
        }


        public EntityIndexRequest ToRequest()
        {
            var request = new EntityIndexRequest();
            request.Target = Target;
            request.QueryType = (TrakHoundIndexQueryType)QueryType;
            request.Value = Value;
            return request;
        }


        public static IEnumerable<TrakHoundHttpEntityIndexRequest> Create(EntityIndexRequest request)
        {
            if (!string.IsNullOrEmpty(request.Target))
            {
                var results = new List<TrakHoundHttpEntityIndexRequest>();
                results.Add(new TrakHoundHttpEntityIndexRequest(request));
                return results;
            }

            return null;
        }

        public static IEnumerable<TrakHoundHttpEntityIndexRequest> Create(IEnumerable<EntityIndexRequest> requests)
        {
            if (!requests.IsNullOrEmpty())
            {
                var x = new List<TrakHoundHttpEntityIndexRequest>();
                foreach (var request in requests)
                {
                    x.Add(new TrakHoundHttpEntityIndexRequest(request));
                }
                return x;
            }

            return null;
        }
    }

    public static class TrakHoundHttpEntityIndexRequestExtensions
    {
        public static IEnumerable<EntityIndexRequest> ToRequests(this IEnumerable<TrakHoundHttpEntityIndexRequest> requests)
        {
            var queries = new List<EntityIndexRequest>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    queries.Add(request.ToRequest());
                }
            }

            return queries;
        }
    }
}
