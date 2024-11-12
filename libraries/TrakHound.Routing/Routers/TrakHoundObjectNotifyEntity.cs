// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Entities;

namespace TrakHound.Routing.Routers
{
    internal class TrakHoundObjectNotifyEntity
    {
        public byte Category { get; set; }

        public byte Class { get; set; }

        public string Uuid { get; set; }


        public TrakHoundObjectNotifyEntity(byte entityCategory, byte entityClass, string uuid)
        {
            Category = entityCategory;
            Class = entityClass;
            Uuid = uuid;
        }

        public TrakHoundObjectNotifyEntity(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                Category = entity.Category;
                Class = entity.Class;
                Uuid = entity.Uuid;
            }
        }
    }
}
