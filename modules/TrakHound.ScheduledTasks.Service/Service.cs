// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Services;
using TrakHound.Volumes;

namespace TrakHound.ScheduledTasks
{
    public class Service : TrakHoundService
    {
        private readonly Dictionary<string, Engine> _engines = new Dictionary<string, Engine>(); // Configuration.Id => Engine
        private readonly object _lock = new object();
        private ITrakHoundVolumeListener _configurationListener;
        private bool _started;


        public Service(ITrakHoundServiceConfiguration configuration, ITrakHoundClient client, ITrakHoundVolume volume) : base(configuration, client, volume) { }


        protected async override Task OnStartAsync()
        {
            await LoadConfigurations();
            StartEngines();

            _configurationListener = Volume.CreateListener("/");
            _configurationListener.Changed += ConfigurationChanged;
            _configurationListener.Start();

            _started = true;
        }

        protected override void OnStop()
        {
            _started = false;

            if (_configurationListener != null) _configurationListener.Dispose();

            StopEngines();
        }


        #region "Configuration"

        private async void ConfigurationChanged(string path, TrakHoundVolumeOnChangeType changeType)
        {
            Log(TrakHoundLogLevel.Information, $"Configuration File Changed : {path} => {changeType}");
            await LoadConfiguration(path, _started);
        }


        private async Task LoadConfigurations()
        {
            var configurationFilePaths = await Volume.ListFiles();
            if (!configurationFilePaths.IsNullOrEmpty())
            {
                foreach (var configurationFilePath in configurationFilePaths)
                {
                    await LoadConfiguration(configurationFilePath, false);
                }
            }
        }

        private async Task LoadConfiguration(string configurationPath, bool startEngine)
        {
            Log(TrakHoundLogLevel.Information, $"Load Configuration :  {configurationPath}");

            var configuration = await Volume.ReadJson<ScheduledTaskConfiguration>(configurationPath);

            if (configuration != null && configuration.Enabled && !string.IsNullOrEmpty(configuration.Id))
            {
                var engine = new Engine(this, configuration);
                engine.LogReceived += EngineLogReceived;

                // Stop Engine (if already Exists)
                var existingEngine = _engines.GetValueOrDefault(configuration.Id);
                if (existingEngine != null)
                {
                    existingEngine.Stop();
                }

                lock (_lock)
                {
                    _engines.Remove(configuration.Id);
                    _engines.Add(configuration.Id, engine);
                }

                if (startEngine) engine.Start();
            }
        }

        #endregion

        #region "Engines"

        private void StartEngines()
        {
            if (!_engines.IsNullOrEmpty())
            {
                // Start Engines
                foreach (var engine in _engines.Values)
                {
                    engine.Start();
                }
            }
        }

        private void StopEngines()
        {
            if (!_engines.IsNullOrEmpty())
            {
                // Stop Engines
                foreach (var engine in _engines.Values)
                {
                    engine.Stop();
                }
            }
        }

        #endregion

        #region "Logging"

        private void EngineLogReceived(Engine engine, TrakHoundLogLevel level, string message, string code = null)
        {
            Log(level, message, code);
        }

        #endregion

    }
}
