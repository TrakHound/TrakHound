// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Entities;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TrakHoundObservationAttribute : TrakHoundEntityEntryAttribute
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string BasePath { get; set; }

        public TrakHoundObservationDataType DataType { get; set; }


        public TrakHoundObservationAttribute()
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Observation.ToString();
            DataType = TrakHoundObservationDataType.String;
        }

        public TrakHoundObservationAttribute(string name, TrakHoundObservationDataType dataType = TrakHoundObservationDataType.String)
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Observation.ToString();
            Name = name;
            DataType = dataType;
        }
    }
}
