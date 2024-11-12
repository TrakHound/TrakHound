// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Blazor.Diagrams
{
    public class TrakHoundDiagramNodeConfiguration
    {
        public string Id { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public IEnumerable<TrakHoundDiagramLinkConfiguration> Links { get; set; }
    }
}
