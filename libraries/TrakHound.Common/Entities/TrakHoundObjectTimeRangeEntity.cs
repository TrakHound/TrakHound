// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to associate a Range of Time to a TrakHound Object Entity
    /// </summary>
    public struct TrakHoundObjectTimeRangeEntity : ITrakHoundObjectTimeRangeEntity
    {
		private string _uuid = null;
		public string Uuid
		{
			get
			{
				if (_uuid == null) _uuid = GenerateUuid(this);
				return _uuid;
			}
		}

		/// <summary>
		/// The ID of the Object that the TimeRange is assigned to
		/// </summary>
		public string ObjectUuid { get; set; }

        /// <summary>
        /// The timestamp in Unix Ticks when this TimeRange begins
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        /// The timestamp in Unix Ticks when this TimeRange ends
        /// </summary>
        public long End { get; set; }

        /// <summary>
        /// The duration of the TimeRange
        /// </summary>
        public TimeSpan Duration => TimeSpan.FromTicks(End - Start);

        /// <summary>
        /// The ID of the Source of the TimeRange
        /// </summary>
        public string SourceUuid { get; set; }

        /// <summary>
        /// The timestamp that the TimeRange was created
        /// </summary>
        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.TimeRange;


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
                    Start > 0 &&
                    End > 0 &&
                    !string.IsNullOrEmpty(SourceUuid) &&
                    Created > 0;
            }
        }


        public TrakHoundObjectTimeRangeEntity()
        {
            ObjectUuid = null;
            Start = 0;
            End = 0;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectTimeRangeEntity(
            string objectUuid,
            long start,
            long end,
            string sourceUuid = null,
            long created = 0
            )
        {
            ObjectUuid = objectUuid;
            Start = start;
            End = end;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectTimeRangeEntity(
            string objectUuid,
            DateTime start,
            DateTime end,
            string sourceUuid = null,
            long created = 0
            )
        {
            ObjectUuid = objectUuid;
            Start = start.ToUnixTime();
            End = end.ToUnixTime();
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectTimeRangeEntity(ITrakHoundObjectTimeRangeEntity schedule)
        {
            ObjectUuid = null;
            Start = 0;
            End = 0;
            SourceUuid = null;
            Created = 0;

            if (schedule != null)
            {
                ObjectUuid = schedule.ObjectUuid;
                Start = schedule.Start;
                End = schedule.End;
                SourceUuid = schedule.SourceUuid;
                Created = schedule.Created;
            }
        }


        public static string GenerateUuid(string objectUuid)
        {
            return $"{objectUuid}:time-range".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectTimeRangeEntity schedule)
        {
            if (schedule != null)
            {
                return GenerateUuid(schedule.ObjectUuid);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectTimeRangeEntity schedule)
        {
            if (schedule != null)
            {
                return $"{schedule.ObjectUuid}:{schedule.Start}:{schedule.End}:{schedule.SourceUuid}:{schedule.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectTimeRangeEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static object[] ToArray(ITrakHoundObjectTimeRangeEntity primitive) => new object[] {
            primitive.ObjectUuid,
            primitive.Start,
            primitive.End,
            primitive.SourceUuid,
            primitive.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectTimeRangeEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 5)
            {
                return new TrakHoundObjectTimeRangeEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    Start = obj[1].ToLong(),
                    End = obj[2].ToLong(),
                    SourceUuid = obj[3]?.ToString(),
                    Created = obj[4].ToLong()
                };
            }

            return new TrakHoundObjectTimeRangeEntity { };
        }

        public static IEnumerable<ITrakHoundObjectTimeRangeEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectTimeRangeEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectTimeRangeEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
