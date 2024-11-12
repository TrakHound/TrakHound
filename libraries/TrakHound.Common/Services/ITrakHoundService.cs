// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Logging;
using TrakHound.Requests;
using TrakHound.Volumes;

namespace TrakHound.Services
{
    public interface ITrakHoundService : IDisposable
    {
        string Id { get; }

        string InstanceId { get; }

        TrakHoundSourceChain SourceChain { get; }

        TrakHoundSourceEntry Source { get; }

        ITrakHoundServiceConfiguration Configuration { get; }

        ITrakHoundClient Client { get; }

        ITrakHoundVolume Volume { get; }

        TrakHoundServiceStatus Status { get; }


        event EventHandler<TrakHoundServiceStatusType> StatusChanged;


        event EventHandler<TrakHoundLogItem> LogReceived;


        Task Start();

        Task Stop();
    }
}
