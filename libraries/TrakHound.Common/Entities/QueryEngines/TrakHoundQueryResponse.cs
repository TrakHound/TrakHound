// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities.Collections;

namespace TrakHound.Entities.QueryEngines
{
    public struct TrakHoundQueryResponse
    {
        public bool Success { get; set; }

        public TrakHoundQueryResult[] Results { get; set; }


        public TrakHoundQueryResponse() { }

        public TrakHoundQueryResponse(TrakHoundQueryResult[] results, bool success)
        {
            Results = results;
            Success = success;
        }


        public TrakHoundEntityCollection GetEntityCollection()
        {
            if (!Results.IsNullOrEmpty())
            {
                var collection = new TrakHoundEntityCollection();
                foreach (var result in Results)
                {
                    collection.Add(TrakHoundQueryResult.GetEntities(result));
                }
                return collection;
            }

            return null;
        }
    }
}
