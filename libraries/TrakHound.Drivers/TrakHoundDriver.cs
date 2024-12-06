// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Volumes;

namespace TrakHound.Drivers
{
    public abstract class TrakHoundDriver : ITrakHoundDriver
    {
        private readonly ITrakHoundDriverConfiguration _configuration;

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

        public ITrakHoundVolume Volume { get; set; }


        public TrakHoundDriver() { }

        public TrakHoundDriver(ITrakHoundDriverConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void Dispose()
        {
            OnDisposed();
        }

        protected virtual void OnDisposed() { }  


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
