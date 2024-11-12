// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Services
{
    public class TrakHoundServiceProcessManager<TServiceProcess, TConfiguration> : IDisposable
        where TServiceProcess : TrakHoundServiceProcess<TConfiguration>
        where TConfiguration : ITrakHoundServiceProcessConfiguration
    {
        private readonly Dictionary<string, TServiceProcess> _processes = new Dictionary<string, TServiceProcess>();
        private readonly ITrakHoundService _service;
        private readonly string _basePath;
        private readonly object _lock = new object();
        private ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>> _configurationNotifyConsumer;
        private ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>> _enabledConsumer;

        public virtual string ServiceName { get; }


        public TrakHoundServiceProcessManager(ITrakHoundService service, string basePath)
        {
			_service = service;
            _basePath = basePath;
        }

        public void Dispose()
        {
            if (_configurationNotifyConsumer != null) _configurationNotifyConsumer.Dispose();
            if (_enabledConsumer != null) _enabledConsumer.Dispose();
        }


        public async Task Start()
        {
            OnStart();
            await OnStartAsync();

            _ = Task.Run(StartProcesses);
        }

        protected virtual void OnStart() { }
        protected virtual Task OnStartAsync() { return Task.CompletedTask; }

        public async Task Stop()
        {
            OnStop();
            await OnStopAsync();

            await StopProcesses();
        }

        protected virtual void OnStop() { }
        protected virtual Task OnStopAsync() { return Task.CompletedTask; }


        private async Task StartProcesses()
        {
            var query = TrakHoundPath.Combine("/", _basePath, "*");


            // Start Processes
            var configurationObjects = await _service.Client.System.Entities.Objects.Query(query);
            if (!configurationObjects.IsNullOrEmpty())
            {
                foreach (var configurationObject in configurationObjects)
                {
                    await StartProcess(configurationObject.Uuid);
                }
            }

            //_enabledConsumer = await _service.Client.System.Entities.Objects.Boolean.SubscribeByObject($"{query}/Enabled");
            //if (_enabledConsumer != null)
            //{
            //    _enabledConsumer.Received += EnabledReceived;
            //}

            // Listen for Changes to MTConnect Device Configuration Objects
            _configurationNotifyConsumer = await _service.Client.System.Entities.Objects.Notify(query, TrakHoundEntityNotificationType.All);
            if (_configurationNotifyConsumer != null)
            {
                _configurationNotifyConsumer.Received += ConfigurationNotificationsReceived;
            }
        }

        private async Task StopProcesses()
        {
            IEnumerable<string> processKeys;
            lock (_lock) processKeys = _processes.Keys?.ToList();

            if (!processKeys.IsNullOrEmpty())
            {
                foreach (var key in processKeys)
                {
                    TServiceProcess process;
                    lock (_lock) process = _processes[key];
                    if (process != null)
                    {
                        try
                        {
                            await process.Stop();
                        }
                        catch { 
                            // Need to log this
                        }
                    }
                }
            }

            lock (_lock) _processes.Clear();
        }


        private async void EnabledReceived(object sender, IEnumerable<ITrakHoundObjectBooleanEntity> entities)
        {
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    var objectEntity = await _service.Client.System.Entities.Objects.ReadByUuid(entity.ObjectUuid);
                    if (objectEntity != null)
                    {
                        var configurationObjectUuid = objectEntity.ParentUuid;
                        if (configurationObjectUuid != null)
                        {
                            if (entity.Value)
                            {
                                await StartProcess(configurationObjectUuid);
                            }
                            else
                            {
                                await StopProcess(configurationObjectUuid);
                            }
                        }
                    }
                }
            }
        }

        private async void ConfigurationNotificationsReceived(object sender, IEnumerable<TrakHoundEntityNotification> notifications)
        {
            if (!notifications.IsNullOrEmpty())
            {
                foreach (var notification in notifications)
                {
                    if (notification != null)
                    {
                        switch (notification.Type)
                        {
                            case TrakHoundEntityNotificationType.Created: await StartProcess(notification.Uuid); break;
                            case TrakHoundEntityNotificationType.Changed: await StartProcess(notification.Uuid); break;
                            case TrakHoundEntityNotificationType.Deleted: await StopProcess(notification.Uuid); break;
                        }
                    }
                }
            }
        }


        private async Task StartProcess(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                TServiceProcess process;
                lock (_lock) process = _processes.GetValueOrDefault(objectUuid);
                if (process == null)
                {
                    // Get Process Configuration
                    var processConfiguration = await _service.Client.Entities.GetSingle<TConfiguration>($"uuid:{objectUuid}");
                    if (processConfiguration != null && !string.IsNullOrEmpty(processConfiguration.Id) && processConfiguration.Enabled)
                    {
                        // Create Process
                        process = OnCreateProcess(_service, _basePath, processConfiguration);
                        if (process != null)
                        {
                            // Subscribe to Process Log
                            process.LogReceived += EngineLogReceived;

                            lock (_lock)
                            {
                                _processes.Remove(objectUuid);
                                _processes.Add(objectUuid, process);
                            }

                            // Start Process
                            await process.Start();
                        }
                    }
                }
            }
        }

        private async Task StopProcess(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                TServiceProcess process;
                lock (_lock) process = _processes.GetValueOrDefault(objectUuid);
                if (process != null)
                {
                    // Stop Process
                    await process.Stop();

                    // Remove from list
                    lock (_lock) _processes.Remove(objectUuid);
                }
            }
        }

        protected virtual TServiceProcess OnCreateProcess(ITrakHoundService service, string basePath, TConfiguration processConfiguration)
        {
            try
            {
                var constructor = typeof(TServiceProcess).GetConstructor(new Type[] { typeof(ITrakHoundService), typeof(string), typeof(TConfiguration) });
                if (constructor != null)
                {
                    return (TServiceProcess)constructor.Invoke(new object[] { service, basePath, processConfiguration });
                }
            }
            catch (Exception ex)
            {
                //if (ApiLoadError != null) ApiLoadError.Invoke(this, ex);
            }

            return null;
        }

        private void EngineLogReceived(object sender, Logging.TrakHoundLogItem logItem)
        {
            if (_service != null)
            {
                ((TrakHoundService)_service).LogInternal(logItem.LogLevel, logItem.Message, logItem.Code);
            }
        }
    }
}
