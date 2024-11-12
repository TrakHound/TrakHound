// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Configurations
{
    struct ConfigurationFileRemovedArgs
    {
        public string Category { get; set; }

        public string Id { get; set; }


        public ConfigurationFileRemovedArgs(string category, string id)
        {
            Category = category;
            Id = id;
        }
    }
}
