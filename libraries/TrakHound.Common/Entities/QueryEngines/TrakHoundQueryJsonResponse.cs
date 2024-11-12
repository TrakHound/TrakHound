// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Entities.QueryEngines
{
    public class TrakHoundQueryJsonResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("results")]
        public TrakHoundQueryJsonResult[] Results { get; set; }


        public TrakHoundQueryJsonResponse() { }

        public TrakHoundQueryJsonResponse(TrakHoundQueryResponse response)
        {
            Success = response.Success;

            if (!response.Results.IsNullOrEmpty())
            {
                var jsonResults = new TrakHoundQueryJsonResult[response.Results.Length];
                for (var i = 0; i < response.Results.Length; i++)
                {
                    jsonResults[i] = new TrakHoundQueryJsonResult(response.Results[i]);
                }
                Results = jsonResults;
            }
        }

        public TrakHoundQueryResponse ToResponse()
        {
            var response = new TrakHoundQueryResponse();
            response.Success = Success;
            
            if (!Results.IsNullOrEmpty())
            {
                var results = new TrakHoundQueryResult[Results.Length];
                for (var i = 0; i < Results.Length; i++)
                {
                    var result = new TrakHoundQueryResult();
                    result.Schema = Results[i].Schema;

                    // Add Columns
                    if (!Results[i].Columns.IsNullOrEmpty())
                    {
                        var columns = new TrakHoundQueryColumnDefinition[Results[i].Columns.Length];
                        for (var j = 0; j < Results[i].Columns.Length; j++)
                        {
                            var column = new TrakHoundQueryColumnDefinition();
                            column.Name = Results[i].Columns[j].Name;
                            column.DataType = Results[i].Columns[j].DataType;
                            columns[j] = column;
                        }
                        result.Columns = columns;
                    }

                    if (!Results[i].Rows.IsNullOrEmpty())
                    {
                        var rowCount = Results[i].Rows.Length;
                        var columnCount = Results[i].Columns.Length;

                        var rows = new object[rowCount, columnCount];
                        for (var j = 0; j < Results[i].Rows.Length; j++)
                        {
                            for (var k = 0; k < columnCount; k++)
                            {
                                rows[j, k] = Results[i].Rows[j][k];
                            }
                        }
                        result.Rows = rows;
                    }

                    results[i] = result;
                }
                response.Results = results;
            }

            return response;
        }
    }
}
