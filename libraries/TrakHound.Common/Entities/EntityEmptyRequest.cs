// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TrakHound.Entities
{
    public struct EntityEmptyRequest
    {
        public string EntityUuid { get; set; }

        public long Timestamp { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(EntityUuid);
            }
        }


        public EntityEmptyRequest(string entityUuid, long timestamp = 0)
        {
            EntityUuid = entityUuid;
            Timestamp = timestamp > 0 ? timestamp : UnixDateTime.Now;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static EntityEmptyRequest FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static object[] ToArray(EntityEmptyRequest obj) => new object[] {
            obj.EntityUuid,
            obj.Timestamp
        };

        public object[] ToArray() => ToArray(this);


        public static EntityEmptyRequest FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 2)
            {
                return new EntityEmptyRequest
                {
                    EntityUuid = obj[0]?.ToString(),
                    Timestamp = obj[1].ToLong()
                };
            }

            return new EntityEmptyRequest { };
        }

        public static IEnumerable<EntityEmptyRequest> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<EntityEmptyRequest>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<EntityEmptyRequest>();
        }

        public static int WriteJson(Stream stream, EntityEmptyRequest request)
        {
            try
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartArray();
                    writer.WriteStringValue(request.EntityUuid);
                    writer.WriteNumberValue(request.Timestamp);
                    writer.WriteEndArray();
                    writer.Flush();
                    return (int)writer.BytesCommitted + writer.BytesPending;
                }
            }
            catch { }

            return 0;
        }

        public static EntityEmptyRequest FromJson(ReadOnlySpan<byte> jsonBytes)
        {
            var entity = new EntityEmptyRequest();

            if (jsonBytes != null)
            {
                try
                {
                    var reader = new Utf8JsonReader(jsonBytes); reader.Read();
                    if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        reader.Read();
                        entity.EntityUuid = reader.GetString(); reader.Read();
                        entity.Timestamp = reader.GetInt64();
                    }
                }
                catch { }
            }

            return entity;
        }
    }
}
