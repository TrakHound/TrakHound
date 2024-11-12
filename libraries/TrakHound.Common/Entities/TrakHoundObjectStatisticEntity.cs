// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Statistic Entities are used to record a Value at a given point in time for a given Time Range.
    /// </summary>
    public struct TrakHoundObjectStatisticEntity : ITrakHoundObjectStatisticEntity
    {
		private string _uuid = null;
        /// <summary>
        /// The Unique Identifier that identifies this Entity
        /// </summary>
        public string Uuid
		{
			get
			{
				if (_uuid == null) _uuid = GenerateUuid(this);
				return _uuid;
			}
		}

        /// <summary>
        /// The UUID of the Object that this Entity is the content for
        /// </summary>
        public string ObjectUuid { get; set; }

        public long TimeRangeStart { get; set; }

        public long TimeRangeEnd { get; set; }

        public int DataType { get; set; }

        public string Value { get; set; }

        public long Timestamp { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Statistic;

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
                return !string.IsNullOrEmpty(ObjectUuid) && 
                       TimeRangeStart > 0 && 
                       TimeRangeEnd > 0 && 
                       !string.IsNullOrEmpty(Value);
            }
        }


        public TrakHoundObjectStatisticEntity()
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            TimeRangeStart = 0;
            TimeRangeEnd = 0;
            DataType = (int)TrakHoundStatisticDataType.Float;
            Value = null;
            SourceUuid = null;
            Timestamp = now;
            Created = now;
        }

        public TrakHoundObjectStatisticEntity(ITrakHoundObjectStatisticEntity entity)
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            TimeRangeStart = 0;
            TimeRangeEnd = 0;
            DataType = (int)TrakHoundStatisticDataType.Float;
            Value = null;
            SourceUuid = null;
            Timestamp = now;
            Created = now;

            if (entity != null)
            {
                ObjectUuid = entity.ObjectUuid;
                TimeRangeStart = entity.TimeRangeStart;
                TimeRangeEnd = entity.TimeRangeEnd;
                DataType = entity.DataType;
                Value = entity.Value;
                SourceUuid = entity.SourceUuid;
                Timestamp = entity.Timestamp;
                Created = entity.Created;
            }
        }

        public TrakHoundObjectStatisticEntity(
            string objectUuid, 
            string timeRangeExpression,
            object value,
            TrakHoundStatisticDataType? dataType = null,
            string sourceUuid = null,
            long timestamp = 0,
            long created = 0
            )
        {
            var now = UnixDateTime.Now;
            var timeRange = TimeRange.Parse(timeRangeExpression);

            ObjectUuid = objectUuid;
            TimeRangeStart = timeRange.From.ToUnixTime();
            TimeRangeEnd = timeRange.To.ToUnixTime();
            DataType = dataType != null ? (int)dataType.Value : (int)GetDataType(value);
            Value = value?.ToString();
            SourceUuid = sourceUuid;
            Timestamp = timestamp > 0 ? timestamp : now;
            Created = created > 0 ? created : now;
        }

        public TrakHoundObjectStatisticEntity(
            string objectUuid,
            long timeRangeStart,
            long timeRangeEnd,
            object value,
            TrakHoundStatisticDataType? dataType = null,
            string sourceUuid = null,
            long timestamp = 0,
            long created = 0
            )
        {
            var now = UnixDateTime.Now;

            ObjectUuid = objectUuid;
            TimeRangeStart = timeRangeStart;
            TimeRangeEnd = timeRangeEnd;
            DataType = dataType != null ? (int)dataType.Value : (int)GetDataType(value);
            Value = value?.ToString();
            SourceUuid = sourceUuid;
            Timestamp = timestamp > 0 ? timestamp : now;
            Created = created > 0 ? created : now;
        }

        public TrakHoundObjectStatisticEntity(
            string objectUuid,
            DateTime timeRangeStart,
            DateTime timeRangeEnd,
            object value,
            TrakHoundStatisticDataType? dataType = null,
            string sourceUuid = null,
            long timestamp = 0,
            long created = 0
            )
        {
            var now = UnixDateTime.Now;

            ObjectUuid = objectUuid;
            TimeRangeStart = timeRangeStart.ToUnixTime();
            TimeRangeEnd = timeRangeEnd.ToUnixTime();
            DataType = dataType != null ? (int)dataType.Value : (int)GetDataType(value);
            Value = value?.ToString();
            SourceUuid = sourceUuid;
            Timestamp = timestamp > 0 ? timestamp : now;
            Created = created > 0 ? created : now;
        }


        public static string GenerateUuid(string objectUuid, long timeRangeStart, long timeRangeEnd)
        {
            return $"{objectUuid}:{timeRangeStart}:{timeRangeEnd}".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectStatisticEntity statistic)
        {
            if (statistic != null)
            {
                return GenerateUuid(statistic.ObjectUuid, statistic.TimeRangeStart, statistic.TimeRangeEnd);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectStatisticEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.TimeRangeStart}:{entity.TimeRangeEnd}:{entity.DataType}:{entity.Value}:{entity.SourceUuid}:{entity.Timestamp}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectStatisticEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static object[] ToArray(ITrakHoundObjectStatisticEntity entity) => new object[] {
            entity.ObjectUuid,
            entity.TimeRangeStart,
            entity.TimeRangeEnd,
            entity.DataType,
            entity.Value,
            entity.SourceUuid,
            entity.Timestamp,
            entity.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectStatisticEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 8)
            {
                return new TrakHoundObjectStatisticEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    TimeRangeStart = obj[1].ToLong(),
                    TimeRangeEnd = obj[2].ToLong(),
                    DataType = obj[3].ToInt(),
                    Value = obj[4]?.ToString(),
                    SourceUuid = obj[5]?.ToString(),
                    Timestamp = obj[6].ToLong(),
                    Created = obj[7].ToLong()
                };
            }

            return new TrakHoundObjectStatisticEntity { };
        }

        public static IEnumerable<ITrakHoundObjectStatisticEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectStatisticEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectStatisticEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }


        public static TrakHoundStatisticDataType GetDataType(object value)
        {
            if (value != null)
            {
                var type = value.GetType();

                if (type == typeof(byte)) return TrakHoundStatisticDataType.Byte;
                if (type == typeof(short)) return TrakHoundStatisticDataType.Int16;
                if (type == typeof(ushort)) return TrakHoundStatisticDataType.Int16;
                if (type == typeof(int)) return TrakHoundStatisticDataType.Int32;
                if (type == typeof(uint)) return TrakHoundStatisticDataType.Int32;
                if (type == typeof(long)) return TrakHoundStatisticDataType.Int64;
                if (type == typeof(ulong)) return TrakHoundStatisticDataType.Int64;
                if (type == typeof(double)) return TrakHoundStatisticDataType.Float;
                if (type == typeof(float)) return TrakHoundStatisticDataType.Float;
                if (type == typeof(decimal)) return TrakHoundStatisticDataType.Decimal;
                if (type == typeof(TimeSpan)) return TrakHoundStatisticDataType.Duration;
            }

            return TrakHoundStatisticDataType.Float;
        }
    }
}
