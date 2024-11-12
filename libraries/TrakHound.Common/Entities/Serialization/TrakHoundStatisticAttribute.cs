// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Entities;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TrakHoundStatisticAttribute : TrakHoundEntityEntryAttribute
    {
        public string Name { get; set; }

        public string BasePath { get; set; }

        public string TimeRange { get; set; }


        public TrakHoundStatisticAttribute(string timeRange)
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Statistic.ToString();
            TimeRange = timeRange;
        }
    }
}
