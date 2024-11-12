// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Logging;
using TrakHound.Packages;
using TrakHound.Requests;
using TrakHound.Volumes;

namespace TrakHound.Services
{
    public abstract class TrakHoundService : ITrakHoundService
    {
        private readonly ITrakHoundServiceConfiguration _configuration;
        private readonly ITrakHoundClient _client;
        private readonly ITrakHoundVolume _volume;
        private TrakHoundServiceStatus _status;


        public string Id { get; set; }

        public string InstanceId { get; set; }

        public TrakHoundPackage Package { get; set; }

        public TrakHoundSourceChain SourceChain { get; set; }

        public TrakHoundSourceEntry Source { get; set; }

        public ITrakHoundServiceConfiguration Configuration => _configuration;

        public ITrakHoundClient Client => _client;

        public ITrakHoundVolume Volume => _volume;

        public TrakHoundServiceStatus Status => _status;


        public event EventHandler<TrakHoundServiceStatusType> StatusChanged;

        public event EventHandler<TrakHoundLogItem> LogReceived;


        public TrakHoundService(
            ITrakHoundServiceConfiguration configuration,
            ITrakHoundClient client,
            ITrakHoundVolume volume
            )
        {
            _configuration = configuration;
            _client = client;
            _volume = volume;
        }


        public async Task Start()
        {
            OnStart();
            await OnStartAsync();
        }

        public async Task Stop()
        {
            OnStop();
            await OnStopAsync();
        }

        public void Dispose()
        {
            Stop().Wait();
        }


        protected virtual void OnStart() { }

        protected virtual Task OnStartAsync() => Task.CompletedTask;

        protected virtual void OnStop() { }

        protected virtual Task OnStopAsync() => Task.CompletedTask;


        protected void Log(TrakHoundLogLevel level, string message, string code = null)
        {
            if (LogReceived != null) LogReceived.Invoke(this, new TrakHoundLogItem("Main", level, message, code, UnixDateTime.Now));
        }

        protected void Log(string logName, TrakHoundLogLevel level, string message, string code = null)
        {
            if (LogReceived != null) LogReceived.Invoke(this, new TrakHoundLogItem(logName, level, message, code, UnixDateTime.Now));
        }

        internal void LogInternal(TrakHoundLogLevel level, string message, string code = null)
        {
            if (LogReceived != null) LogReceived.Invoke(this, new TrakHoundLogItem("Main", level, message, code, UnixDateTime.Now));
        }

        internal void LogInternal(string logName, TrakHoundLogLevel level, string message, string code = null)
        {
            if (LogReceived != null) LogReceived.Invoke(this, new TrakHoundLogItem(logName, level, message, code, UnixDateTime.Now));
        }
    }
}
