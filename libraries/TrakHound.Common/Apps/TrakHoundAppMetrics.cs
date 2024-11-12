// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Apps
{
    public class TrakHoundAppMetrics
    {
        [JsonPropertyName("appId")]
        public string AppId { get; set; }
    }
}
