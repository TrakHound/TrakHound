// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities
{
    public partial struct TrakHoundQueryResult
    {
        public string Schema { get; set; }

        public TrakHoundQueryColumnDefinition[] Columns { get; set; }

        public object[,] Rows { get; set; }

        public int RowCount => Rows != null && Columns != null && Columns.Length > 0 ? Rows.Length / Columns.Length : 0;

        public int ColumnCount => Columns != null ? Columns.Length : 0;


        public TrakHoundQueryResult(TrakHoundQueryColumnDefinition[] columns, object[,] rows, string schema = null)
        {
            Columns = columns;
            Rows = rows;
            Schema = schema;
        }


        private static IEnumerable<TEntity> GetEntities<TEntity>(TrakHoundQueryResult queryResult) where TEntity : ITrakHoundEntity
        {
            if (queryResult.Columns != null && queryResult.RowCount > 0)
            {
                var entities = new TEntity[queryResult.RowCount];
                for (var i = 0; i < queryResult.RowCount; i++)
                {
                    var row = new object[queryResult.ColumnCount];
                    for (var j = 0; j < queryResult.ColumnCount; j++)
                    {
                        row[j] = queryResult.Rows[i, j];
                    }

                    entities[i] = TrakHoundEntity.FromArray<TEntity>(row);
                }
                return entities;
            }

            return null;
        }

        public static TrakHoundQueryDataType GetValueDataType(string contentType)
        {
            switch (contentType)
            {
                case TrakHoundObjectContentTypes.Assignment: return TrakHoundQueryDataType.Object;
                case TrakHoundObjectContentTypes.Boolean: return TrakHoundQueryDataType.Boolean;
                case TrakHoundObjectContentTypes.Duration: return TrakHoundQueryDataType.Duration;
                case TrakHoundObjectContentTypes.Event: return TrakHoundQueryDataType.Object;
                //case TrakHoundObjectContentTypes.Feed: return TrakHoundQueryDataType.String;
                case TrakHoundObjectContentTypes.Group: return TrakHoundQueryDataType.Object;
                case TrakHoundObjectContentTypes.Observation: return TrakHoundQueryDataType.String;
                case TrakHoundObjectContentTypes.Reference: return TrakHoundQueryDataType.Object;
                case TrakHoundObjectContentTypes.State: return TrakHoundQueryDataType.Definition;
                case TrakHoundObjectContentTypes.String: return TrakHoundQueryDataType.String;
                case TrakHoundObjectContentTypes.Timestamp: return TrakHoundQueryDataType.Timestamp;
                case TrakHoundObjectContentTypes.Vocabulary: return TrakHoundQueryDataType.Definition;
            }

            return TrakHoundQueryDataType.String;
        }
    }
}
