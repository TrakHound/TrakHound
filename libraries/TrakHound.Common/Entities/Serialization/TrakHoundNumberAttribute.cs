// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Entities;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TrakHoundNumberAttribute : TrakHoundEntityEntryAttribute
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string BasePath { get; set; }

        public TrakHoundNumberDataType? DataType { get; set; }


        public TrakHoundNumberAttribute()
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Number.ToString();
        }

        public TrakHoundNumberAttribute(string name)
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Number.ToString();
            Name = name;
        }

        public TrakHoundNumberAttribute(TrakHoundNumberDataType dataType)
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Number.ToString();
            DataType = dataType;
        }

        public TrakHoundNumberAttribute(string name, TrakHoundNumberDataType dataType)
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Number.ToString();
            Name = name;
            DataType = dataType;
        }


        public static TrakHoundNumberDataType GetDataType(Type type)
        {
            if (type != null)
            {
                if (type == typeof(byte)) return TrakHoundNumberDataType.Byte;
                if (type == typeof(short)) return TrakHoundNumberDataType.Int16;
                if (type == typeof(ushort)) return TrakHoundNumberDataType.Int16;
                if (type == typeof(int)) return TrakHoundNumberDataType.Int32;
                if (type == typeof(uint)) return TrakHoundNumberDataType.Int32;
                if (type == typeof(long)) return TrakHoundNumberDataType.Int64;
                if (type == typeof(ulong)) return TrakHoundNumberDataType.Int64;
                if (type == typeof(float)) return TrakHoundNumberDataType.Float;
                if (type == typeof(double)) return TrakHoundNumberDataType.Double;
                if (type == typeof(decimal)) return TrakHoundNumberDataType.Decimal;
            }

            return TrakHoundNumberDataType.Float;
        }
    }
}
