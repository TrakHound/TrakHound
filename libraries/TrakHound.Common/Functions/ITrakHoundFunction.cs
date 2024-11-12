// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Logging;
using TrakHound.Requests;
using TrakHound.Volumes;

namespace TrakHound.Functions
{
    public interface ITrakHoundFunction : IDisposable
    {
        string Id { get; }

        string InstanceId { get; }

        TrakHoundSourceChain SourceChain { get; }

        TrakHoundSourceEntry Source { get; }

        ITrakHoundFunctionConfiguration Configuration { get; }

        ITrakHoundClient Client { get; }

        ITrakHoundVolume Volume { get; }


        event EventHandler<TrakHoundLogItem> LogReceived;


        Task<TrakHoundFunctionResponse> Run(IReadOnlyDictionary<string, string> parameters);
    }
}
