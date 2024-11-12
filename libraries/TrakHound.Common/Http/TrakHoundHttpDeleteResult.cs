// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Http
{
    public class TrakHoundHttpDeleteResult
    {
        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("count")]
        public long Count { get; set; }


        public TrakHoundHttpDeleteResult() { }

        public TrakHoundHttpDeleteResult(EntityDeleteResult result)
        {
            Target = result.Target;
            Count = result.Count;
        }


        public EntityDeleteResult ToResult()
        {
            var result = new EntityDeleteResult();
            result.Target = Target;
            result.Count = Count;
            return result;
        }


        public static IEnumerable<TrakHoundHttpDeleteResult> Create(EntityDeleteResult result)
        {
            if (!string.IsNullOrEmpty(result.Target) && result.Count > 0)
            {
                var results = new List<TrakHoundHttpDeleteResult>();
                results.Add(new TrakHoundHttpDeleteResult(result));
                return results;
            }

            return null;
        }

        public static IEnumerable<TrakHoundHttpDeleteResult> Create(IEnumerable<EntityDeleteResult> results)
        {
            if (!results.IsNullOrEmpty())
            {
                var x = new List<TrakHoundHttpDeleteResult>();
                foreach (var result in results)
                {
                    x.Add(new TrakHoundHttpDeleteResult(result));
                }
                return x;
            }

            return null;
        }
    }

    public static class TrakHoundHttpDeleteResultExtensions
    {
        public static IEnumerable<EntityDeleteResult> ToResults(this IEnumerable<TrakHoundHttpDeleteResult> results)
        {
            var queries = new List<EntityDeleteResult>();

            if (!results.IsNullOrEmpty())
            {
                foreach (var result in results)
                {
                    queries.Add(result.ToResult());
                }
            }

            return queries;
        }
    }
}
