// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Logging;
using TrakHound.Volumes;

namespace TrakHound.Drivers
{
    public abstract class TrakHoundDriver : ITrakHoundDriver
    {
        private readonly ITrakHoundDriverConfiguration _configuration;
        private ITrakHoundVolume _volume;
        private ITrakHoundLogger _logger;

        public ITrakHoundDriverConfiguration Configuration => _configuration;

        private string _id;
        public string Id
        {
            get
            {
                if (_id == null) _id = $"{Configuration?.Id}:{GetType().AssemblyQualifiedName}".ToMD5Hash();
                return _id;
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                if (_name == null) _name = GetType().Name;
                return _name;
            }
        }

        public virtual bool IsAvailable { get; set; }

        public virtual string AvailabilityMessage { get; set; }

        public ITrakHoundVolume Volume => _volume;

        public ITrakHoundLogger Logger => _logger;


        public TrakHoundDriver(ITrakHoundDriverConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
            OnDisposed();
        }

        protected virtual void OnDisposed() { }


        public void SetVolume(ITrakHoundVolume volume)
        {
            _volume = volume;
            AfterVolumeSet();
        }

        public void SetLogger(ITrakHoundLogger logger)
        {
            _logger = logger;
            AfterLoggerSet();
        }

        protected virtual void AfterVolumeSet() { }

        protected virtual void AfterLoggerSet() { }


        protected void Log(TrakHoundLogLevel level, string message, string code = null)
        {
            if (Logger != null) Logger.Log(new TrakHoundLogItem("Main", level, message, code, UnixDateTime.Now));
        }

        protected void Log(string logName, TrakHoundLogLevel level, string message, string code = null)
        {
            if (Logger != null) Logger.Log(new TrakHoundLogItem(logName, level, message, code, UnixDateTime.Now));
        }


        protected TrakHoundResponse<TResult> NotFound<TResult>(string request = null)
        {
            return TrakHoundResponse<TResult>.NotFound(Id, request);
        }

        protected TrakHoundResponse<TResult> RouteNotConfigured<TResult>(string request = null)
        {
            return TrakHoundResponse<TResult>.RouteNotConfigured(Id, request);
        }

        protected TrakHoundResponse<TResult> NotAvailable<TResult>(string request = null)
        {
            return TrakHoundResponse<TResult>.NotAvailable(Id, request);
        }
    }
}
