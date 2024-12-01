// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using TrakHound.Buffers;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Routing
{
    /// <summary>
    /// Used to initialize and provide access to IApiDriver classes. Loads drivers from plugin modules on initialization.
    /// </summary>
    public partial class TrakHoundRouterProvider
    {
        private readonly ITrakHoundConfigurationProfile _configurationProfile;
        private readonly ITrakHoundDriverProvider _driverProvider;
        private readonly ITrakHoundBufferProvider _bufferProvider;

        private readonly List<TrakHoundRouter> _routers = new List<TrakHoundRouter>();
        private readonly DelayEvent _loadDelay = new DelayEvent(1000);
        private readonly object _lock = new object();

        /// <summary>
        /// List of Routers used to access a combination of drivers
        /// </summary>
        public IEnumerable<TrakHoundRouter> Routers => _routers;

        public event EventHandler<TrakHoundRouter> RouterAdded;

        public event EventHandler<string> RouterRemoved;

        public ITrakHoundClientProvider ClientProvider { get; set; }


        public TrakHoundRouterProvider(
            ITrakHoundConfigurationProfile configurationProfile,
            ITrakHoundDriverProvider driverProvider,
            ITrakHoundBufferProvider bufferProvider
            )
        {
            _configurationProfile = configurationProfile;
            _configurationProfile.ConfigurationAdded += ConfigurationUpdated;
            _configurationProfile.ConfigurationRemoved += ConfigurationUpdated;

            _driverProvider = driverProvider;
            _driverProvider.DriverAdded += (s, o) => { _loadDelay.Set(); };
            _driverProvider.DriverRemoved += (s, o) => { _loadDelay.Set(); };

            _bufferProvider = bufferProvider;

            _loadDelay.Elapsed += LoadDelayElapsed;
        }


        public void Dispose()
        {
            _loadDelay.Elapsed -= LoadDelayElapsed;
            _loadDelay.Dispose();

            lock (_lock) _routers.Clear();
        }


        private void ConfigurationUpdated(object sender, ITrakHoundConfiguration configuration)
        {
            if (configuration != null && configuration.Category == TrakHoundRouterConfiguration.ConfigurationCategory)
            {
                _loadDelay.Set();
            }
        }

        private void LoadDelayElapsed(object sender, System.EventArgs args)
        {
            Load();
        }

        public void Load()
        {
            CreateRouters();
            MapRouters();
            InitializeRouters();
            InitializeBuffers();
        }
        

        #region "Information"

        public IEnumerable<TrakHoundRouterInformation> GetInformation()
        {
            var informations = new List<TrakHoundRouterInformation>();

            if (!_routers.IsNullOrEmpty())
            {
                foreach (var router in _routers)
                {
                    var information = TrakHoundRouterInformation.Create(router);
                    if (information != null) informations.Add(information);
                }
            }

            return informations;
        }

        public TrakHoundRouterInformation GetInformation(string routerId)
        {
            if (!string.IsNullOrEmpty(routerId))
            {
                var router = _routers.FirstOrDefault(o => o.Id == routerId);
                return TrakHoundRouterInformation.Create(router);
            }

            return null;
        }

        #endregion

        #region "Routers"

        public TrakHoundRouter AddRouter(TrakHoundRouterConfiguration configuration)
        {
            if (_configurationProfile != null)
            {
                _configurationProfile.Add(configuration);
            }

            bool add;
            lock (_lock) add = !_routers.Any(o => o.Id == configuration.Id);
            if (add)
            {
                var router = new TrakHoundRouter(configuration, _driverProvider, _bufferProvider);

                var client = ClientProvider?.GetClient();
                client.RouterId = configuration.Id;
                router.Client = client;

                router.MapTargets(Routers, _driverProvider.Drivers);
                router.Initialize(Routers, _driverProvider.Drivers);

                lock (_lock) _routers.Add(router);

                if (RouterAdded != null) RouterAdded.Invoke(this, router);

                return router;
            }

            return null;
        }

        public void RemoveRouter(string routerId)
        {
            if (_configurationProfile != null)
            {
                _configurationProfile.Remove(TrakHoundRouterConfiguration.ConfigurationCategory, routerId);

                if (RouterRemoved != null) RouterRemoved.Invoke(this, routerId);

                Load();
            }
        }


        public void MapRouters()
        {
            if (!Routers.IsNullOrEmpty())
            {
                var drivers = _driverProvider.Drivers;

                foreach (var router in Routers)
                {
                    router.MapTargets(Routers, drivers);
                }
            }
        }

        private void InitializeRouters()
        {
            if (!Routers.IsNullOrEmpty())
            {
                var drivers = _driverProvider.Drivers;

                foreach (var router in Routers)
                {
                    router.Initialize(Routers, drivers);
                }
            }
        }

        private void CreateRouters()
        {
            lock (_lock) _routers.Clear();

            if (_configurationProfile != null)
            {
                var configurations = _configurationProfile.Get<TrakHoundRouterConfiguration>(TrakHoundRouterConfiguration.ConfigurationCategory);
                if (!configurations.IsNullOrEmpty())
                {
                    foreach (var configuration in configurations)
                    {
                        var router = new TrakHoundRouter(configuration, _driverProvider, _bufferProvider);

                        lock (_lock)
                        {
                            if (!_routers.Any(o => o.Id == configuration.Id))
                            {
                                _routers.Add(router);
                            }
                        }

                        var client = ClientProvider?.GetClient();
                        client.RouterId = configuration.Id;
                        router.Client = client;

                        if (RouterAdded != null) RouterAdded.Invoke(this, router);
                    }
                }
            }
        }

        public TrakHoundRouter GetRouter()
        {
            lock (_lock)
            {
                if (!_routers.IsNullOrEmpty())
                {
                    var defaultRouter = _routers.FirstOrDefault(o => o.Configuration.Name == TrakHoundRouter.Default);
                    if (defaultRouter != null)
                    {
                        if (defaultRouter.Initialized) return defaultRouter;
                    }
                    else
                    {
                        var router = _routers.FirstOrDefault();
                        if (router.Initialized) return router;
                    }
                }
            }


            return null;
        }

        public TrakHoundRouter GetRouter(string routerKey)
        {
            lock (_lock)
            {
                if (!_routers.IsNullOrEmpty())
                {
                    if (!string.IsNullOrEmpty(routerKey))
                    {
                        var router = _routers.FirstOrDefault(o => MatchRouterKey(o, routerKey));
                        if (router != null && router.Initialized) return router;
                    }
                    else
                    {
                        return GetRouter();
                    }
                }
            }

            return null;
        }

        private static bool MatchRouterKey(TrakHoundRouter router, string routerKey)
        {
            if (router != null && !string.IsNullOrEmpty(router.Id) && !string.IsNullOrEmpty(routerKey))
            {
                if (router.Id == routerKey)
                {
                    return true;
                }
                else if (router.Configuration != null && !string.IsNullOrEmpty(router.Configuration.Name))
                {
                    return router.Configuration.Name.ToLower() == routerKey.ToLower();
                }
            }

            return false;
        }

        #endregion

        #region "Buffers"

        //public void InitializeBuffers()
        //{
        //    if (_bufferProvider != null && _driverProvider != null && !_driverProvider.Drivers.IsNullOrEmpty())
        //    {
        //        var driverConfigurationIds = _driverProvider.Drivers.Select(o => o.Configuration.Id).Distinct().ToList();
        //        foreach (var driverConfigurationId in driverConfigurationIds)
        //        {
        //            AddEntityBuffers<ITrakHoundDefinitionEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundDefinitionMetadataEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundDefinitionDescriptionEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundDefinitionWikiEntity>(driverConfigurationId);

        //            AddEntityBuffers<ITrakHoundObjectEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectAssignmentEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectBlobEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectBooleanEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectDurationEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectEventEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectFeedEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectGroupEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectHashEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectLedgerEntity>(driverConfigurationId);
        //            //AddEntityBuffers<ITrakHoundObjectLinkEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectLogEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectMessageEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectMessageQueueEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectMetadataEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectNumberEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectObservationEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectQueueEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectReferenceEntity>(driverConfigurationId);
        //            //AddEntityBuffers<ITrakHoundObjectSequenceEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectSetEntity>(driverConfigurationId);
        //            //AddEntityBuffers<ITrakHoundObjectSnapshotEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectStateEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectStatisticEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectStringEntity>(driverConfigurationId);
        //            //AddEntityBuffers<ITrakHoundObjectTimeGroupEntity>(driverConfigurationId);
        //            //AddEntityBuffers<ITrakHoundObjectTimeHashEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectTimeRangeEntity>(driverConfigurationId);
        //            //AddEntityBuffers<ITrakHoundObjectTimeSequenceEntity>(driverConfigurationId);
        //            //AddEntityBuffers<ITrakHoundObjectTimeSetEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectTimestampEntity>(driverConfigurationId);
        //            //AddEntityBuffers<ITrakHoundObjectTimeWikiEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectVocabularyEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundObjectVocabularySetEntity>(driverConfigurationId);
        //            //AddEntityBuffers<ITrakHoundObjectWikiEntity>(driverConfigurationId);

        //            AddEntityBuffers<ITrakHoundSourceEntity>(driverConfigurationId);
        //            AddEntityBuffers<ITrakHoundSourceMetadataEntity>(driverConfigurationId);

        //            //AddEntityBuffers<ITrakHoundTransactionEntity>(driverConfigurationId);
        //        }
        //    }
        //}

        private void AddEntityBuffers<TEntity>(string configurationId) where TEntity : ITrakHoundEntity
        {
            AddPublishBuffer<TEntity>(configurationId);
            AddEmptyBuffer<TEntity>(configurationId);
            AddDeleteBuffer<TEntity>(configurationId);
            AddIndexBuffer<TEntity>(configurationId);
        }

        private void AddPublishBuffer<TEntity>(string configurationId) where TEntity : ITrakHoundEntity
        {
            if (_bufferProvider != null && _driverProvider != null)
            {
                var driver = _driverProvider.GetDriver<IEntityPublishDriver<TEntity>>(configurationId);
                if (driver != null)
                {
                    _bufferProvider.AddPublishBuffer<TEntity>(driver);
                }
            }
        }

        private void AddEmptyBuffer<TEntity>(string configurationId) where TEntity : ITrakHoundEntity
        {
            if (_bufferProvider != null && _driverProvider != null)
            {
                var driver = _driverProvider.GetDriver<IEntityEmptyDriver<TEntity>>(configurationId);
                if (driver != null)
                {
                    _bufferProvider.AddEmptyBuffer<TEntity>(driver);
                }
            }
        }

        private void AddDeleteBuffer<TEntity>(string configurationId) where TEntity : ITrakHoundEntity
        {
            if (_bufferProvider != null && _driverProvider != null)
            {
                var driver = _driverProvider.GetDriver<IEntityDeleteDriver<TEntity>>(configurationId);
                if (driver != null)
                {
                    _bufferProvider.AddDeleteBuffer<TEntity>(driver);
                }
            }
        }

        private void AddIndexBuffer<TEntity>(string configurationId) where TEntity : ITrakHoundEntity
        {
            if (_bufferProvider != null && _driverProvider != null)
            {
                var driver = _driverProvider.GetDriver<IEntityIndexUpdateDriver<TEntity>>(configurationId);
                if (driver != null)
                {
                    _bufferProvider.AddIndexBuffer<TEntity>(driver);
                }
            }
        }

        #endregion

    }
}
