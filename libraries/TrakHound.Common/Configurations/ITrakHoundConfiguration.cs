// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Configurations
{
    public interface ITrakHoundConfiguration 
    {
        string Id { get; }

        string Category { get; }

        string Path { get; set; }

        string Hash { get; }
    }
}
