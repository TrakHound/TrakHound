// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Logging;

namespace TrakHound.Services
{
    public class TrakHoundServiceProcess<TConfiguration> where TConfiguration : ITrakHoundServiceProcessConfiguration
    {
		private const string _instancesPath = ".Instances";
		private const string _servicesPath = "Services";
		private const string _processesPath = "Processes";
		private const string _logsPath = "Logs";
		private const string _defaultLogName = "Main";

		private static readonly ITrakHoundLogger _logger = new TrakHoundLogger<TrakHoundServiceProcess<TConfiguration>>();
        private readonly ITrakHoundService _service;
        private readonly string _basePath;
        private readonly TConfiguration _processConfiguration;
        private CancellationTokenSource _stop;

        public virtual string ServiceName { get; }

        public string ProcessId => _processConfiguration?.Id;

        public ITrakHoundService Service => _service;

        public ITrakHoundClient Client => _service.Client;

        public event EventHandler<TrakHoundLogItem> LogReceived;


        public TrakHoundServiceProcess(ITrakHoundService service, string basePath, TConfiguration processConfiguration)
        {
			_service = service;
            _basePath = basePath;
			_processConfiguration = processConfiguration;
        }


        public async Task Start()
        {
            if (_processConfiguration != null)
            {
				Log(TrakHoundLogLevel.Information, $"({_processConfiguration.Id}) {ServiceName} Process Starting..");

                try
                {
                    OnStart();
                    await OnStartAsync();
                }
                catch (Exception ex)
                {
                    Log(TrakHoundLogLevel.Error, $"({_processConfiguration.Id}) {ServiceName} Process Error : {ex.Message}");
                }

                _stop = new CancellationTokenSource();

                _ = Task.Run(Worker);

				Log(TrakHoundLogLevel.Information, $"({_processConfiguration.Id}) {ServiceName} Process Started");
			}
        }

        protected virtual void OnStart() { }

        protected virtual Task OnStartAsync() { return Task.CompletedTask; }


        public async Task Stop()
        {
            if (_processConfiguration != null)
            {
                Log(TrakHoundLogLevel.Information, $"({_processConfiguration.Id}) {ServiceName} Process Stopping..");

                if (_stop != null) _stop.Cancel();

                try
                {
                    OnStop();
                    await OnStopAsync();
                }
                catch (Exception ex)
                {
                    Log(TrakHoundLogLevel.Error, $"({_processConfiguration.Id}) {ServiceName} Process Error : {ex.Message}");
                }

				Log(TrakHoundLogLevel.Information, $"({_processConfiguration.Id}) {ServiceName} Process Stopped");
			}
        }

        protected virtual void OnStop() { }

        protected virtual Task OnStopAsync() { return Task.CompletedTask; }


        private async Task Worker()
        {
            var configurationPath = TrakHoundPath.Combine(_instancesPath, _service.InstanceId, _servicesPath, _service.Configuration.PackageId, "Engines", _service.Id, _processesPath, _processConfiguration.Id);
            await Client.Entities.PublishObject(configurationPath);

            OnWorker();
            await OnWorkerAsync();
        }

        protected virtual void OnWorker() { }

        protected virtual Task OnWorkerAsync() => Task.CompletedTask;


		public void Log(TrakHoundLogLevel level, string message, string code = null)
		{
            if (LogReceived != null) LogReceived.Invoke(this, new TrakHoundLogItem("Main", level, message, code, UnixDateTime.Now));
		}

        public void Log(string logName, TrakHoundLogLevel level, string message, string code = null)
		{
            if (LogReceived != null) LogReceived.Invoke(this, new TrakHoundLogItem(logName, level, message, code, UnixDateTime.Now));
        }
	}
}
