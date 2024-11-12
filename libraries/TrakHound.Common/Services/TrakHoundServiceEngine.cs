// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Logging;

namespace TrakHound.Services
{
    public class TrakHoundServiceEngine
    {
        private const int _restartInterval = 5000;

        private readonly string _engineId;
        private readonly string _packageId;
        private readonly string _packageVersion;
        private readonly ITrakHoundService _service;
        private readonly ITrakHoundClient _client;
        private CancellationTokenSource _stop;
        private TrakHoundServiceStatusType _status;


        public string EngineId => _engineId;

        public string PackageId => _packageId;

        public string PackageVersion => _packageVersion;

        public ITrakHoundService Service => _service;

        public ITrakHoundClient Client => _client;

        public TrakHoundServiceStatusType Status => _status;


        public event EventHandler<TrakHoundServiceStatusType> StatusChanged;

        public event EventHandler<TrakHoundLogItem> LogReceived;


        public TrakHoundServiceEngine(ITrakHoundService service, string packageId, string packageVersion)
        {
            _service = service;

            if (service != null)
            {
                _engineId = service.Id;
                _packageId = packageId;
                _packageVersion = packageVersion;
                _client = service.Client;

                service.LogReceived += ServiceLogReceived;
            }
        }


        public void Start()
        {
            if (_status != TrakHoundServiceStatusType.Started && _status != TrakHoundServiceStatusType.Starting)
            {
                _status = TrakHoundServiceStatusType.Starting;
                if (StatusChanged != null) StatusChanged.Invoke(_service, _status);
                if (LogReceived != null) LogReceived.Invoke(_service, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Information, "Service Starting.."));

                _stop = new CancellationTokenSource();

                _ = Task.Run(Worker);
            }
        }

        public async Task Stop()
        {
            if (_status != TrakHoundServiceStatusType.Stopped && _status != TrakHoundServiceStatusType.Stopping)
            {
                _status = TrakHoundServiceStatusType.Stopping;
                if (StatusChanged != null) StatusChanged.Invoke(_service, _status);
                if (LogReceived != null) LogReceived.Invoke(_service, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Information, "Service Stopping.."));

                try
                {
                    if (_service != null) await _service.Stop();
                    if (_service != null) _service.Dispose();
                    if (_stop != null) _stop.Cancel();
                }
                catch (Exception ex)
                {
                    if (LogReceived != null) LogReceived.Invoke(_service, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Error, $"Service Stop ERROR : {ex.Message}"));

                    _status = TrakHoundServiceStatusType.Stopped;
                    if (StatusChanged != null) StatusChanged.Invoke(_service, _status);
                    if (LogReceived != null) LogReceived.Invoke(_service, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Information, "Service Stopped"));
                }
            }
        }

        private async Task Worker()
        {
            if (_client != null && _service != null)
            {
                var first = true;

                await Task.Delay(250);

                while (!_stop.IsCancellationRequested)
                {
                    try
                    {
                        if (!first)
                        {
                            _status = TrakHoundServiceStatusType.Starting;
                            if (StatusChanged != null) StatusChanged.Invoke(_service, _status);
                            if (LogReceived != null) LogReceived.Invoke(_service, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Information, "Service Starting.."));

                            await Task.Delay(250);
                        }

                        // Start the Service
                        await _service.Start();

                        _status = TrakHoundServiceStatusType.Started;
                        if (StatusChanged != null) StatusChanged.Invoke(_service, _status);
                        if (LogReceived != null) LogReceived.Invoke(_service, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Information, "Service Started"));

                        while (!_stop.IsCancellationRequested) await Task.Delay(250);
                    }
                    catch (TaskCanceledException) { }
                    catch (Exception ex)
                    {
                        _status = TrakHoundServiceStatusType.Error;
                        if (StatusChanged != null) StatusChanged.Invoke(_service, _status);
                        if (LogReceived != null) LogReceived.Invoke(_service, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Critical, ex.Message, ex.HResult.ToString()));

                        await Task.Delay(_restartInterval, _stop.Token);
                    }

                    if (!_stop.IsCancellationRequested)
                    {
                        if (LogReceived != null) LogReceived.Invoke(_service, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Information, "Service Restarting"));
                    }

                    first = false;
                }

                await Task.Delay(250);

                _status = TrakHoundServiceStatusType.Stopped;
                if (StatusChanged != null) StatusChanged.Invoke(_service, _status);
                if (LogReceived != null) LogReceived.Invoke(_service, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Information, "Service Stopped"));
            }
        }

        private void ServiceLogReceived(object sender, TrakHoundLogItem item)
        {
            if (LogReceived != null) LogReceived.Invoke(_service, item);
        }
    }
}
