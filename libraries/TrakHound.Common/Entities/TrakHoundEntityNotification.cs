// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Entities
{
    public class TrakHoundEntityNotification
	{
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public TrakHoundEntityNotificationType Type { get; set; }

        [JsonPropertyName("category")]
        public byte Category { get; set; }

        [JsonPropertyName("class")]
        public byte Class { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonIgnore]
        public ITrakHoundEntity Entity { get; set; }


		public TrakHoundEntityNotification() { }

		public TrakHoundEntityNotification(TrakHoundEntityNotificationType type, string uuid)
		{
			Type = type;
			Uuid = uuid;
		}

        public TrakHoundEntityNotification(TrakHoundEntityNotificationType type, ITrakHoundEntity entity)
        {
            Type = type;
			if (entity != null)
			{
				Category = entity.Category;
				Class = entity.Class;
				Uuid = entity.Uuid;
                Entity = entity;
			}
        }
    }
}
