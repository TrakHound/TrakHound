// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public class TrakHoundEntityTableColumn
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public TrakHoundEntityTableDataType DataType { get; set; }

        public int? Length { get; set; }

        public bool AllowNull { get; set; }


        public TrakHoundEntityTableColumn() { }

        public TrakHoundEntityTableColumn(
            string name,
            TrakHoundEntityTableDataType dataType, 
            int? dataLength = null,
            bool allowNull = true
            )
        {
            Name = name;
            DataType = dataType;
            Length = dataLength;
            AllowNull = allowNull;
        }
    }
}
