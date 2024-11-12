// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Data
{
    public class TrakHoundDataColumn
    {
        public int Index { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }


        public TrakHoundDataColumn() { }

        public TrakHoundDataColumn(string id, string name = null)
        {
            Id = id;
            Name = name;
        }

        public TrakHoundDataColumn(int index, string id, string name = null)
        {
            Index = index;
            Id = id;
            Name = name;
        }
    }
}
