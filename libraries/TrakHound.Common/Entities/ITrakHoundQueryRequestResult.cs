// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities
{
    public interface ITrakHoundQueryRequestResult
    {
        string Uuid { get; }

        string Query { get; }

        IEnumerable<string> TargetIds { get; }

        IEnumerable<string> RootIds { get; }
    }
}
