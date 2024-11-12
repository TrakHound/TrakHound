// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TrakHound.Entities
{
    public struct EntityDeleteRequest
    {
        public string Target { get; set; }

        public long Timestamp { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Target);
            }
        }


        public EntityDeleteRequest(string target, long timestamp = 0)
        {
            Target = target;
            Timestamp = timestamp > 0 ? timestamp : UnixDateTime.Now;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static EntityDeleteRequest FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static object[] ToArray(EntityDeleteRequest obj) => new object[] {
            obj.Target,
            obj.Timestamp
        };

        public object[] ToArray() => ToArray(this);


        public static EntityDeleteRequest FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 2)
            {
                return new EntityDeleteRequest
                {
                    Target = obj[0]?.ToString(),
                    Timestamp = obj[1].ToLong()
                };
            }

            return new EntityDeleteRequest { };
        }

        public static IEnumerable<EntityDeleteRequest> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<EntityDeleteRequest>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<EntityDeleteRequest>();
        }


        public static IEnumerable<EntityDeleteRequest> Create(string target, long timestamp = 0)
        {
            var requests = new List<EntityDeleteRequest>();

            var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

            if (!string.IsNullOrEmpty(target))
            {
                requests.Add(new EntityDeleteRequest(target, ts));
            }

            return requests;
        }

        public static IEnumerable<EntityDeleteRequest> Create(IEnumerable<string> targets, long timestamp = 0)
        {
            var requests = new List<EntityDeleteRequest>();

            var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

            if (!targets.IsNullOrEmpty())
            {
                foreach (var target in targets)
                {
                    requests.Add(new EntityDeleteRequest(target, ts));
                }
            }

            return requests;
        }


        public static int WriteJson(Stream stream, EntityDeleteRequest request)
        {
            try
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartArray();
                    writer.WriteStringValue(request.Target);
                    writer.WriteNumberValue(request.Timestamp);
                    writer.WriteEndArray();
                    writer.Flush();
                    return (int)writer.BytesCommitted + writer.BytesPending;
                }
            }
            catch { }

            return 0;
        }

        public static EntityDeleteRequest FromJson(ReadOnlySpan<byte> jsonBytes)
        {
            var entity = new EntityDeleteRequest();

            if (jsonBytes != null)
            {
                try
                {
                    var reader = new Utf8JsonReader(jsonBytes); reader.Read();
                    if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        reader.Read();
                        entity.Target = reader.GetString(); reader.Read();
                        entity.Timestamp = reader.GetInt64();
                    }
                }
                catch { }
            }

            return entity;
        }
    }
}
