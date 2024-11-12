// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using TrakHound.Buffers;
using TrakHound.Clients;
using TrakHound.Drivers;
using TrakHound.Routing.Routers;

namespace TrakHound.Routing
{
    public partial class TrakHoundEntitiesRouter
    {
        private readonly TrakHoundRouter _router;
        private readonly ITrakHoundDriverProvider _driverProvider;
        private readonly ITrakHoundBufferProvider _bufferProvider;

        private readonly ITrakHoundClient _client;

        private SourcesRouter _sources;
        private DefinitionsRouter _definitions;
        private ObjectsRouter _objects;


        public TrakHoundRouter Router => _router;

        public ITrakHoundBufferProvider Buffers => _bufferProvider;


        /// <summary>
        /// Router used to access TrakHound Source Entities
        /// </summary>
        public SourcesRouter Sources => _sources;

        /// <summary>
        /// Router used to access TrakHound Definition Entities
        /// </summary>
        public DefinitionsRouter Definitions => _definitions;

        /// <summary>
        /// Router used to access TrakHound Object Entities
        /// </summary>
        public ObjectsRouter Objects => _objects;


        public TrakHoundEntitiesRouter(TrakHoundRouter router, ITrakHoundDriverProvider driverProvider, ITrakHoundBufferProvider bufferProvider)
        {
            _router = router;
            _bufferProvider = bufferProvider;
            _client = router.Client;
        }


        public void Initialize(IEnumerable<TrakHoundRouter> routers, IEnumerable<ITrakHoundDriver> drivers)
        {
            _sources = new SourcesRouter(_router);
            _definitions = new DefinitionsRouter(_router);
            _objects = new ObjectsRouter(_router);
        }
    }
}
