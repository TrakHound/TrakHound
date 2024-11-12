// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Entities.QueryEngines
{
    public class TrakHoundQueryJsonColumnDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("dataType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public TrakHoundQueryDataType DataType { get; set; }


        public TrakHoundQueryJsonColumnDefinition() { }

        public TrakHoundQueryJsonColumnDefinition(TrakHoundQueryColumnDefinition column)
        {
            Name = column.Name;
            DataType = column.DataType;
        }
    }

    public class TrakHoundQueryJsonResult
    {
        [JsonPropertyName("schema")]
        public string Schema { get; set; }

        [JsonPropertyName("columns")]
        public TrakHoundQueryJsonColumnDefinition[] Columns { get; set; }

        [JsonPropertyName("rows")]
        public object[][] Rows { get; set; }


        public TrakHoundQueryJsonResult() { }

        public TrakHoundQueryJsonResult(TrakHoundQueryResult result)
        {
            if (!result.Columns.IsNullOrEmpty())
            {
                var columns = new TrakHoundQueryJsonColumnDefinition[result.Columns.Length];
                for (var i = 0; i < columns.Length; i++)
                {
                    columns[i] = new TrakHoundQueryJsonColumnDefinition(result.Columns[i]);
                }
                Columns = columns;
            }

            if (result.Rows != null && result.Rows.Length > 0)
            {
                var rowCount = result.Rows.GetLength(0);
                var columnCount = result.Rows.GetLength(1);

                var rows = new object[rowCount][];
                for (var i = 0; i < rowCount; i++)
                {
                    var row = new object[columnCount];
                    for (var j = 0; j < columnCount; j++)
                    {
                        row[j] = result.Rows[i, j];
                    }
                    rows[i] = row;
                }

                Rows = rows;
            }

            Schema = result.Schema;
        }
    }
}
