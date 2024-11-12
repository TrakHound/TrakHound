// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace TrakHound.Entities
{
    /// <summary>
    /// Observations are used to record a Value at a given point in time. This Value may be static or may be constantly changing. 
    /// </summary>
    public struct TrakHoundObjectObservationEntity : ITrakHoundObjectObservationEntity
    {
        private static JsonTypeInfo<object[]> _jsonTypeInfo = JsonTypeInfo.CreateJsonTypeInfo<object[]>(Json.DefaultOptions);


		private string _uuid = null;
        public string Uuid
		{
			get
			{
				if (_uuid == null) _uuid = GenerateUuid(this);
				return _uuid;
			}
		}

        public string ObjectUuid { get; set; }

        public ulong BatchId { get; set; }

        public ulong Sequence { get; set; }

        public int DataType { get; set; }

        public string Value { get; set; }

        public long Timestamp { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Observation;

		private byte[] _hash = null;
		public byte[] Hash
		{
			get
			{
				if (_hash == null) _hash = GenerateHash(this);
				return _hash;
			}
		}

		public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(ObjectUuid) && Value != null;
            }
        }


        public TrakHoundObjectObservationEntity()
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            DataType = (int)TrakHoundObservationDataType.String;
            Value = null;
            BatchId = 0;
            Sequence = 0;
            Timestamp = now;
            SourceUuid = null;
            Created = now;
        }

        public TrakHoundObjectObservationEntity(
            string objectUuid,
            object value,
            long timestamp = 0,
            ulong batchId = 0,
            ulong sequence = 0,
            string sourceUuid = null,
            int dataType = (int)TrakHoundObservationDataType.String,
            long created = 0
            )
        {
            var now = UnixDateTime.Now;

            ObjectUuid = objectUuid;
            DataType = dataType;
            Value = value != null ? value.ToString() : null;
            BatchId = batchId;
            Sequence = sequence;
            Timestamp = timestamp > 0 ? timestamp : now;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : now;
        }

        public TrakHoundObjectObservationEntity(ITrakHoundObjectObservationEntity entity)
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            DataType = (int)TrakHoundObservationDataType.String;
            Value = null;
            BatchId = 0;
            Sequence = 0;
            Timestamp = now;
            SourceUuid = null;
            Created = now;

            if (entity != null)
            {
                ObjectUuid = entity.ObjectUuid;
                DataType = entity.DataType;
                Value = entity.Value;
                BatchId = entity.BatchId;
                Sequence = entity.Sequence;
                Timestamp = entity.Timestamp;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created;
            }
        }


        public static string GenerateUuid(string objectUuid, long timestamp)
        {
            return $"{objectUuid}:{timestamp}".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectObservationEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid, entity.Timestamp);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectObservationEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.DataType}:{entity.Value}:{entity.BatchId}:{entity.Sequence}:{entity.Timestamp}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => Json.ToJsonArray(ToArray());

        public static ITrakHoundObjectObservationEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static ITrakHoundObjectObservationEntity FromJson(ReadOnlySpan<byte> json) => FromArray(Json.Convert<object[]>(json, _jsonTypeInfo));


        public static object[] ToArray(ITrakHoundObjectObservationEntity entity) => new object[] {
            entity.ObjectUuid,
            entity.DataType,
            entity.Value,
            entity.BatchId,
            entity.Sequence,
            entity.Timestamp,
            entity.SourceUuid,
            entity.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectObservationEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 8)
            {
                return new TrakHoundObjectObservationEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    DataType = obj[1].ToInt(),
                    Value = obj[2]?.ToString(),
                    BatchId = obj[3].ToULong(),
                    Sequence = obj[4].ToULong(),
                    Timestamp = obj[5].ToLong(),
                    SourceUuid = obj[6]?.ToString(),
                    Created = obj[7].ToLong()
                };
            }

            return new TrakHoundObjectObservationEntity { };
        }

        public static IEnumerable<ITrakHoundObjectObservationEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectObservationEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectObservationEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (overwrite || string.IsNullOrEmpty(SourceUuid)) SourceUuid = sourceUuid;
        }
    }
}

