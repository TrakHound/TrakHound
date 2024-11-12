// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Requests;

namespace TrakHound.Api
{
    public interface ITrakHoundApiController 
    {
        string Id { get; set; }

        string InstanceId { get; }

        string BaseUrl { get; }

        string BasePath { get; }

        string BaseLocation { get; }

        TrakHoundSourceChain SourceChain { get; }

        TrakHoundSourceEntry Source { get; }

        ITrakHoundApiConfiguration Configuration { get; }
    }
}
