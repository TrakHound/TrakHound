// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Entities;

namespace TrakHound.Blazor.Services
{
    public class EntityClipboardService
    {
        //private readonly ExplorerService _explorerService;
        private readonly ITrakHoundClient _client;
        private readonly Dictionary<string, ClipboardItem> _items = new Dictionary<string, ClipboardItem>();
        private int _itemCount;
        private bool _isShown;


        public struct ClipboardItem
        {
            public string ServerId { get; set; }
            public string RouterId { get; set; }
            public ITrakHoundEntity Entity { get; set; }
            public bool KeepOriginal { get; set; }


            public ClipboardItem(string serverId, string routerId, ITrakHoundEntity entity, bool keepOriginal)
            {
                ServerId = serverId;
                RouterId = routerId;
                Entity = entity;
                KeepOriginal = keepOriginal;
            }
        }


        public int ItemCount => _itemCount;

        public bool IsShown
        {
            get => _isShown;
            set
            {
                _isShown = value;

                if (IsShownChanged != null) IsShownChanged.Invoke(this, _isShown);
            }
        }

        public EventHandler<ITrakHoundEntity> EntityAdded { get; set; }

        public EventHandler<string> EntityRemoved { get; set; }

        public EventHandler Cleared { get; set; }

        public EventHandler<bool> IsShownChanged { get; set; }


        public EntityClipboardService(ITrakHoundClient client)
        {
            _client = client;
        }


        private static string GenerateKey(string serverId, string routerId, string uuid)
        {
            return $"{serverId}:{routerId}:{uuid}";
        }


        public bool Add(string serverId, string routerId, ITrakHoundEntity entity, bool keepOriginal = true) 
        {
            var key = GenerateKey(serverId, routerId, entity.Uuid);
            if (!_items.ContainsKey(key))
            {
                _items.Add(key, new ClipboardItem(serverId, routerId, entity, keepOriginal));
                _itemCount++;

                if (EntityAdded != null) EntityAdded.Invoke(this, entity);
            }
            
            return true; 
        }

        public bool Add(string serverId, string routerId, IEnumerable<ITrakHoundEntity> entities, bool keepOriginal = true) 
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    var key = GenerateKey(serverId, routerId, entity.Uuid);
                    if (!_items.ContainsKey(key))
                    {
                        _items.Add(key, new ClipboardItem(serverId, routerId, entity, keepOriginal));
                        _itemCount++;

                        if (EntityAdded != null) EntityAdded.Invoke(this, entity);
                    }
                }
            }

            return true; 
        }

        public async Task<bool> Add<TEntity>(string serverId, string routerId, string entityUuid, bool keepOriginal = true) where TEntity : ITrakHoundEntity
        {
            if (_client != null)
            {
                var entityClient = _client.System.Entities.GetEntityClient<TEntity>();
                if (entityClient != null)
                {
                    var entity = await entityClient.ReadByUuid(entityUuid);
                    if (entity != null)
                    {
                        Add(serverId, routerId, entity, keepOriginal);
                    }
                }
            }

            return true;    
        }

        public async Task<bool> Add<TEntity>(string serverId, string routerId, IEnumerable<string> entityUuids, bool keepOriginal = true) where TEntity : ITrakHoundEntity
        {
            if (_client != null)
            {
                var entityClient = _client.System.Entities.GetEntityClient<TEntity>();
                if (entityClient != null)
                {
                    var entities = await entityClient.ReadByUuid(entityUuids);
                    if (entities != null)
                    {
                        var x = new List<ITrakHoundEntity>();
                        foreach (var entity in entities) x.Add(entity);

                        Add(serverId, routerId, x, keepOriginal);
                    }
                }
            }

            return true;
        }


        public IEnumerable<ClipboardItem> Get()
        {
            return _items.Values.ToList();
        }


        public void Remove(string serverId, string routerId, string uuid)
        {
            if (_items.Remove(GenerateKey(serverId, routerId, uuid)))
            {
                _itemCount--;

                if (EntityRemoved != null) EntityRemoved.Invoke(this, uuid);
            }
        }


        public void Clear()
        {
            _items.Clear();
            _itemCount = 0;

            if (Cleared != null) Cleared.Invoke(this, new EventArgs());
        }

    }
}
