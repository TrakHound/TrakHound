// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Radzen;
using System;
using System.Collections.Concurrent;
using TrakHound.Clients;
using TrakHound.Entities;
using TrakHound.Requests;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public class ObjectExplorerService : IDisposable
    {
        private const int _recentLimit = 2000 * 1000000;


        private readonly string _baseUrl;
        private readonly ITrakHoundClient _client;
        private readonly TrakHound.Blazor.Services.NotificationService _notificationService;
        private readonly ObjectExplorerAddService _addService;
        private readonly ObjectExplorerDeleteService _deleteService;
        private readonly string _consumerId;

        private readonly Dictionary<string, ITrakHoundObjectEntity> _objects = new Dictionary<string, ITrakHoundObjectEntity>();
        private readonly ListDictionary<string, string> _objectParents = new ListDictionary<string, string>();
        private readonly ListDictionary<string, ITrakHoundObjectMetadataEntity> _objectMetadata = new ListDictionary<string, ITrakHoundObjectMetadataEntity>();
        private readonly Dictionary<string, ITrakHoundDefinitionEntity> _definitions = new Dictionary<string, ITrakHoundDefinitionEntity>();
        private readonly ListDictionary<string, string> _definitionParents = new ListDictionary<string, string>();
        private readonly Dictionary<string, ITrakHoundDefinitionDescriptionEntity> _definitionDescriptions = new Dictionary<string, ITrakHoundDefinitionDescriptionEntity>();
        private readonly ListDictionary<string, ITrakHoundSourceEntity> _sourceChains = new ListDictionary<string, ITrakHoundSourceEntity>();

        private readonly List<string> _targetUuids = new List<string>();
        private readonly HashSet<string> _expandedNamespaces = new HashSet<string>();
        private readonly HashSet<string> _expandedObjects = new HashSet<string>();
        private readonly Dictionary<string, ITrakHoundEntity> _content = new Dictionary<string, ITrakHoundEntity>();
        private readonly Dictionary<string, long> _objectChildCount = new Dictionary<string, long>();
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();
        private readonly Dictionary<string, long> _recentValues = new Dictionary<string, long>();
        private readonly Dictionary<TrakHoundObjectContentType, ITrakHoundConsumer> _consumers = new Dictionary<TrakHoundObjectContentType, ITrakHoundConsumer>();
        private readonly List<IContentConsumer> _contentConsumers = new List<IContentConsumer>();
        private readonly ConcurrentQueue<ContentUpdate> _contentUpdated = new ConcurrentQueue<ContentUpdate>();
        private readonly ThrottleEvent _contentUpdateThrottle;
        private readonly DelayEvent _selectedObjectLoadDelay;
        private readonly System.Timers.Timer _recentTimer;
        private readonly object _lock = new object();

        private string _query;
        private string _previousQuery;
        private string _uuid;
        private string _previousUuid;
        private IEnumerable<string> _filteredTargetUuids;
        private ITrakHoundObjectEntity _selectedObject;
        private string _clipboardId;
        private bool _hiddenShown;


        class ContentUpdate
        {
            public string ObjectUuid { get; set; }
            public ITrakHoundEntity Entity { get; set; }
            public string Value { get; set; }
            public long Timestamp { get; set; }
        }


        public string InstanceId { get; set; }

        public string RouterId { get; set; }

        public string BaseUrl => _baseUrl;

        public ITrakHoundClient Client => _client;

        public TrakHound.Blazor.Services.NotificationService NotificationService => _notificationService;

        public ObjectExplorerAddService AddService => _addService;

        public ObjectExplorerDeleteService DeleteService => _deleteService;

        public string Query => _query;

        public string Uuid => _uuid;

        public IEnumerable<string> TargetUuids => _targetUuids;

        public IEnumerable<string> FilteredTargetUuids => _filteredTargetUuids;

        public Dictionary<string, ITrakHoundObjectEntity> Objects => _objects;

        public ITrakHoundObjectEntity SelectedObject => _selectedObject;

        public string SelectedObjectUuid => _selectedObject?.Uuid;

        public string ClipboardId => _clipboardId;

        public bool HiddenShown => _hiddenShown;


        public EventHandler<ITrakHoundObjectEntity> EditClicked { get; set; }

        public EventHandler<ITrakHoundObjectEntity> AddChildClicked { get; set; }

        public EventHandler<ITrakHoundObjectEntity> RefreshClicked { get; set; }

        public EventHandler<string> NamespaceExpanded { get; set; } // Namespace to Expand

        public EventHandler<string> NamespaceCollapsed { get; set; } // Namespace to Expand

        public EventHandler<string> ObjectExpanded { get; set; } // Uuid of Object to Expand

        public EventHandler<string> ObjectCollapsed { get; set; } // Uuid of Object to Expand

        public EventHandler<ITrakHoundObjectEntity> SelectedObjectChanged { get; set; }

        public EventHandler Updated { get; set; }

        public EventHandler TargetObjectsUpdated { get; set; }

        public EventHandler<string> QueryChanged { get; set; }

        public EventHandler<string> ObjectUpdated { get; set; }

        public EventHandler<string> ValueUpdated { get; set; }


        public Func<string, string> ObjectsQueryLinkFunction { get; set; }

        public Func<string, string> DefinitionsQueryLinkFunction { get; set; }

        public Func<string, string> TransactionsQueryLinkFunction { get; set; }

        public Func<string, string> SourcesQueryLinkFunction { get; set; }


        public ObjectExplorerService(string baseUrl, ITrakHoundClient client)
        {
            _baseUrl = baseUrl;
            _client = client;
            _consumerId = Guid.NewGuid().ToString();

            _notificationService = new Services.NotificationService();
            _notificationService.Updated += (s, o) => Updated?.Invoke(this, o);

            _addService = new ObjectExplorerAddService(this);
            _deleteService = new ObjectExplorerDeleteService(this);

            _recentTimer = new System.Timers.Timer();
            _recentTimer.Interval = 500;
            _recentTimer.Elapsed += ProcessRecentValues;
            _recentTimer.Start();

            //_contentUpdateThrottle = new ThrottleEvent(500);
            //_contentUpdateThrottle.Elapsed += ContentUpdateThrottleElapsed;

            _selectedObjectLoadDelay = new DelayEvent(500);
            _selectedObjectLoadDelay.Elapsed += SelectedObjectLoadDelayElapsed;
        }

        public void Dispose()
        {
            if (_notificationService != null) _notificationService.Dispose();
            if (_recentTimer != null) _recentTimer.Dispose();

            lock (_lock)
            {
                _values.Clear();

                if (!_consumers.IsNullOrEmpty())
                {
                    foreach (var consumer in _consumers) consumer.Value.Dispose();
                }

                if (!_contentConsumers.IsNullOrEmpty())
                {
                    foreach (var consumer in _contentConsumers) consumer.Dispose();
                }

                _consumers.Clear();
            }
        }


        public ITrakHoundClient GetClient()
        {
            return _client;
        }

        public IEnumerable<ITrakHoundObjectEntity> GetChildObjects(string parentUuid)
        {
            if (!string.IsNullOrEmpty(parentUuid))
            {
                var childUuids = _objectParents.Get(parentUuid);
                if (!childUuids.IsNullOrEmpty())
                {
                    var results = new List<ITrakHoundObjectEntity>();
                    foreach (var childUuid in childUuids)
                    {
                        var result = _objects.GetValueOrDefault(childUuid);
                        if (result != null) results.Add(result);
                    }
                    return results;
                }
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectEntity> GetChildObjectsByRoot(string parentUuid)
        {
            var results = new List<ITrakHoundObjectEntity>();

            if (!string.IsNullOrEmpty(parentUuid))
            {
                var childObjects = GetChildObjects(parentUuid);
                if (!childObjects.IsNullOrEmpty())
                {
                    foreach (var childObject in childObjects)
                    {
                        results.Add(childObject);
                        results.AddRange(GetChildObjectsByRoot(childObject.Uuid));
                    }
                }
            }

            return results;
        }

        public bool GetNamespaceExpanded(string ns)
        {
            if (!string.IsNullOrEmpty(ns))
            {
                lock (_lock)
                {
                    return _expandedNamespaces.Contains(ns.ToLower());
                }
            }

            return false;
        }

        public bool GetObjectExpanded(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                lock (_lock)
                {
                    return _expandedObjects.Contains(uuid);
                }
            }

            return false;
        }

        public long GetObjectChildCount(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                lock (_lock)
                {
                    return _objectChildCount.GetValueOrDefault(uuid);
                }
            }

            return 0;
        }

        public IEnumerable<ITrakHoundObjectMetadataEntity> GetObjectMetadata(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                return _objectMetadata.Get(objectUuid);
            }

            return null;
        }

        public void AddObjectMetadata(string entityUuid, string name, object value)
        {
            if (!string.IsNullOrEmpty(entityUuid) && !string.IsNullOrEmpty(name) && value != null)
            {
                _objectMetadata.Remove(entityUuid);
                _objectMetadata.Add(entityUuid, new TrakHoundObjectMetadataEntity(entityUuid, name, value));
            }
        }

        public void AddObjectMetadata(ITrakHoundObjectMetadataEntity metadataEntity)
        {
            if (metadataEntity != null && !string.IsNullOrEmpty(metadataEntity.EntityUuid))
            {
                _objectMetadata.Remove(metadataEntity.EntityUuid);
                _objectMetadata.Add(metadataEntity.EntityUuid, metadataEntity);
            }
        }

        public void RemoveObjectMetadata(string entityUuid, string name)
        {
            if (!string.IsNullOrEmpty(entityUuid) && !string.IsNullOrEmpty(name))
            {
                var entityMetadata = _objectMetadata.Get(entityUuid);
                if (!entityMetadata.IsNullOrEmpty())
                {
                    // Clear all Metadata for Object
                    _objectMetadata.Remove(entityUuid);

                    // Remove Metadata matching Name
                    var x = entityMetadata.ToList();
                    x.RemoveAll(o => o.Name == name);

                    // Add back new Metadata list for Object
                    _objectMetadata.Add(entityUuid, x);
                }
            }
        }

        public ITrakHoundDefinitionEntity GetDefinition(string definitionUuid)
        {
            if (!string.IsNullOrEmpty(definitionUuid))
            {
                return _definitions.GetValueOrDefault(definitionUuid);
            }

            return null;
        }

        public ITrakHoundDefinitionDescriptionEntity GetDefinitionDescription(string definitionUuid)
        {
            if (!string.IsNullOrEmpty(definitionUuid))
            {
                return _definitionDescriptions.GetValueOrDefault(definitionUuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundDefinitionEntity> GetInheritedDefinitions(string definitionUuid)
        {
            if (!string.IsNullOrEmpty(definitionUuid))
            {
                var uuids = _definitionParents.Get(definitionUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var results = new List<ITrakHoundDefinitionEntity>();
                    foreach (var uuid in uuids)
                    {
                        var result = _definitions.GetValueOrDefault(uuid);
                        if (result != null) results.Add(result);
                    }
                    return results;
                }
            }

            return null;
        }



        public IEnumerable<ITrakHoundSourceEntity> GetSourceChain(string sourceUuid)
        {
            if (!string.IsNullOrEmpty(sourceUuid))
            {
                return _sourceChains.Get(sourceUuid);
            }

            return null;
        }



        public void AddNotification(NotificationType type, string message, string details = null, int duration = 5000)
        {
            _notificationService.AddNotification(type, message, details, duration);
        }


        public void Update()
        {
            if (Updated != null) Updated.Invoke(this, EventArgs.Empty);
        }

        public void UpdateTargetObjects()
        {
            if (TargetObjectsUpdated != null) TargetObjectsUpdated.Invoke(this, EventArgs.Empty);
        }


        public void SetQuery(string query)
        {
            if (QueryChanged != null) QueryChanged.Invoke(this, query);
        }

        public void SetTargetUuids(IEnumerable<string> uuids)
        {
            _targetUuids.Clear();
            if (!uuids.IsNullOrEmpty())
            {
                _targetUuids.AddRange(uuids);
            }

            UpdateTargetObjects();
        }


        public void ClearTree()
        {
            _uuid = null;
            _previousUuid = null;
            _query = null;
            _previousQuery = null;
            _selectedObject = null;
            _targetUuids.Clear();
            _expandedNamespaces.Clear();
            _expandedObjects.Clear();
            _objects.Clear();
            _objectParents.Clear();
            _objectChildCount.Clear();
            _definitions.Clear();
            _definitionParents.Clear();
            _definitionDescriptions.Clear();
            _sourceChains.Clear();
        }


        public async Task LoadByExpression(string query, bool forceReload = false)
        {
            if (string.IsNullOrEmpty(query)) _query = "/";
            else _query = query;

            if (forceReload || _query != _previousQuery)
            {
                if (_client != null)
                {
                    _uuid = null;
                    _previousUuid = null;
                    _previousQuery = _query;
                    _selectedObject = null;
                    _targetUuids.Clear();
                    _objects.Clear();
                    _objectParents.Clear();

                    var objects = await _client.System.Entities.Objects.Query(_query);
                    if (!objects.IsNullOrEmpty())
                    {
                        foreach (var obj in objects)
                        {
                            var objectUuid = obj.Uuid;
                            if (!string.IsNullOrEmpty(objectUuid))
                            {
                                _objects.Remove(objectUuid);
                                _objects.Add(objectUuid, obj);

                                if (!string.IsNullOrEmpty(obj.ParentUuid))
                                {
                                    _objectParents.Add(obj.ParentUuid, obj.Uuid);
                                }
                            }
                        }

                        LoadContent(objects);

                        // Expand Namespaces
                        var namespaces = objects.Select(o => o.Namespace).Distinct();
                        lock (_lock)
                        {
                            foreach (var ns in namespaces)
                            {
                                _expandedNamespaces.Add($"NAMESPACE:{ns}".ToLower());
                            }
                        }
                    }

                    _targetUuids.Clear();
                    if (!objects.IsNullOrEmpty())
                    {
                        _targetUuids.AddRange(objects.Select(o => o.Uuid));
                    }

                    FilterTargetObjects();
                }
            }
        }

        public async Task LoadByQuery(string query, bool forceReload = false)
        {
            if (string.IsNullOrEmpty(query)) _query = "select >> from [/]";
            else _query = query;

            if (forceReload || _query != _previousQuery)
            {
                if (_client != null)
                {
                    _uuid = null;
                    _previousUuid = null;
                    _previousQuery = _query;
                    _selectedObject = null;
                    _targetUuids.Clear();
                    _expandedNamespaces.Clear();
                    _expandedObjects.Clear();
                    _objects.Clear();
                    _objectParents.Clear();
                    _objectChildCount.Clear();
                    _definitions.Clear();
                    _definitionParents.Clear();
                    _definitionDescriptions.Clear();
                    _sourceChains.Clear();

                    var queryResponse = await _client.System.Entities.Query(_query);
                    if (queryResponse.Success)
                    {
                        if (!queryResponse.Results.IsNullOrEmpty())
                        {
                            var objects = new List<ITrakHoundObjectEntity>();
                            foreach (var queryResult in queryResponse.Results)
                            {
                                if (queryResult.Schema == "trakhound.entities.objects")
                                {
                                    if (queryResult.Rows != null)
                                    {
                                        var rowCount = queryResult.Rows.GetLength(0);
                                        var columnCount = queryResult.Rows.GetLength(1);

                                        for (var i = 0; i < rowCount; i++)
                                        {
                                            var row = new object[columnCount];
                                            for (var j = 0; j < columnCount; j++)
                                            {
                                                row[j] = queryResult.Rows[i, j];
                                            }

                                            var objectEntity = TrakHoundObjectEntity.FromArray(row);
                                            if (objectEntity.IsValid) objects.Add(objectEntity);
                                        }
                                    }
                                }
                            }

                            var targetUuids = objects?.Select(o => o.Uuid);

                            if (!objects.IsNullOrEmpty())
                            {
                                foreach (var obj in objects)
                                {
                                    var objectUuid = obj.Uuid;
                                    if (!string.IsNullOrEmpty(objectUuid))
                                    {
                                        _objects.Remove(objectUuid);
                                        _objects.Add(objectUuid, obj);

                                        if (!string.IsNullOrEmpty(obj.ParentUuid))
                                        {
                                            _objectParents.Add(obj.ParentUuid, obj.Uuid);
                                        }
                                    }
                                }

                                // Get Child Count
                                var directoryUuids = objects?.Where(o => o.ContentType == TrakHoundObjectContentTypes.Directory)?.Select(o => o.Uuid);
                                var childCounts = await _client.System.Entities.Objects.QueryChildCount(directoryUuids);
                                if (!childCounts.IsNullOrEmpty())
                                {
                                    lock (_lock)
                                    {
                                        foreach (var count in childCounts)
                                        {
                                            _objectChildCount.Remove(count.Target);
                                            _objectChildCount.Add(count.Target, count.Count);
                                        }
                                    }
                                }

                                LoadContent(objects);

                                // Expand Namespaces
                                var namespaces = objects.Select(o => o.Namespace).Distinct();
                                lock (_lock)
                                {
                                    foreach (var ns in namespaces)
                                    {
                                        _expandedNamespaces.Add($"NAMESPACE:{ns}".ToLower());
                                    }
                                }
                            }

                            _targetUuids.Clear();
                            if (!objects.IsNullOrEmpty())
                            {
                                _targetUuids.AddRange(targetUuids);
                            }

                            FilterTargetObjects();
                        }
                    }
                }
            }
        }

        public async Task LoadByUuid(string uuid, bool forceReload = false)
        {
            if (forceReload || !string.IsNullOrEmpty(uuid) && uuid != _previousUuid)
            {
                if (_client != null)
                {
                    _uuid = null;
                    _previousUuid = null;
                    _query = null;
                    _previousQuery = null;
                    _selectedObject = null;
                    _targetUuids.Clear();
                    _expandedNamespaces.Clear();
                    _expandedObjects.Clear();
                    _objects.Clear();
                    _objectParents.Clear();
                    _objectChildCount.Clear();
                    _definitions.Clear();
                    _definitionParents.Clear();
                    _definitionDescriptions.Clear();
                    _sourceChains.Clear();

                    var objects = await _client.System.Entities.Objects.ReadByUuid(new string[] { _uuid });
                    if (!objects.IsNullOrEmpty())
                    {
                        foreach (var obj in objects)
                        {
                            var objectUuid = obj.Uuid;
                            if (!string.IsNullOrEmpty(objectUuid))
                            {
                                _objects.Remove(objectUuid);
                                _objects.Add(objectUuid, obj);

                                if (!string.IsNullOrEmpty(obj.ParentUuid))
                                {
                                    _objectParents.Add(obj.ParentUuid, obj.Uuid);
                                }
                            }
                        }

                        // Get Child Count
                        var childCount = await _client.System.Entities.Objects.QueryChildCount(uuid);
                        if (childCount != null)
                        {
                            lock (_lock)
                            {
                                _objectChildCount.Remove(childCount.Target);
                                _objectChildCount.Add(childCount.Target, childCount.Count);
                            }
                        }

                        LoadContent(objects);

                        // Expand Namespaces
                        var namespaces = objects.Select(o => o.Namespace).Distinct();
                        lock (_lock)
                        {
                            foreach (var ns in namespaces)
                            {
                                _expandedNamespaces.Add($"NAMESPACE:{ns}".ToLower());
                            }
                        }
                    }

                    _targetUuids.Clear();
                    if (!objects.IsNullOrEmpty())
                    {
                        _targetUuids.AddRange(objects.Select(o => o.Uuid));
                    }

                    FilterTargetObjects();
                    SelectObject(_uuid);
                }
            }
        }

        public async Task LoadObject(string uuid)
        {
            if (_client != null)
            {
                var childObjects = await _client.System.Entities.Objects.QueryByParentUuid(uuid, 0, 100, SortOrder.Ascending);
                if (!childObjects.IsNullOrEmpty())
                {
                    var obj = _objects.GetValueOrDefault(uuid);
                    if (obj != null)
                    {
                        _objects.Remove(uuid);
                        _objects.Add(uuid, obj);

                        if (!string.IsNullOrEmpty(obj.ParentUuid))
                        {
                            _objectParents.Add(obj.ParentUuid, obj.Uuid);
                        }

                        foreach (var childObject in childObjects)
                        {
                            _objects.Remove(childObject.Uuid);
                            _objects.Add(childObject.Uuid, childObject);

                            if (!string.IsNullOrEmpty(childObject.ParentUuid))
                            {
                                _objectParents.Add(childObject.ParentUuid, childObject.Uuid);
                            }
                        }

                        var targetUuids = childObjects.Select(o => o.Uuid);

                        // Get Child Count
                        var directoryUuids = childObjects?.Where(o => o.ContentType == TrakHoundObjectContentTypes.Directory)?.Select(o => o.Uuid);
                        var childCounts = await _client.System.Entities.Objects.QueryChildCount(directoryUuids);
                        if (!childCounts.IsNullOrEmpty())
                        {
                            lock (_lock)
                            {
                                foreach (var count in childCounts)
                                {
                                    _objectChildCount.Remove(count.Target);
                                    _objectChildCount.Add(count.Target, count.Count);
                                }
                            }
                        }

                        LoadContent(childObjects);

                        // Expand Namespaces
                        var namespaces = childObjects.Select(o => o.Namespace).Distinct();
                        lock (_lock)
                        {
                            foreach (var ns in namespaces)
                            {
                                _expandedNamespaces.Add($"NAMESPACE:{ns}".ToLower());
                            }
                        }
                    }
                }
            }
        }

        public async Task LoadObjects(IEnumerable<string> uuids)
        {
            if (_client != null && !uuids.IsNullOrEmpty())
            {
                var childObjects = await _client.System.Entities.Objects.QueryByParentUuid(uuids, 0, 100);
                if (!childObjects.IsNullOrEmpty())
                {
                    foreach (var childObject in childObjects)
                    {
                        _objects.Remove(childObject.Uuid);
                        _objects.Add(childObject.Uuid, childObject);

                        if (!string.IsNullOrEmpty(childObject.ParentUuid))
                        {
                            _objectParents.Add(childObject.ParentUuid, childObject.Uuid);
                        }
                    }

                    var targetUuids = childObjects.Select(o => o.Uuid);

                    // Get Child Count
                    var directoryUuids = childObjects?.Where(o => o.ContentType == TrakHoundObjectContentTypes.Directory)?.Select(o => o.Uuid);
                    var childCounts = await _client.System.Entities.Objects.QueryChildCount(directoryUuids);
                    if (!childCounts.IsNullOrEmpty())
                    {
                        lock (_lock)
                        {
                            foreach (var count in childCounts)
                            {
                                _objectChildCount.Remove(count.Target);
                                _objectChildCount.Add(count.Target, count.Count);
                            }
                        }
                    }

                    LoadContent(childObjects);

                    // Expand Namespaces
                    var namespaces = childObjects.Select(o => o.Namespace).Distinct();
                    lock (_lock)
                    {
                        foreach (var ns in namespaces)
                        {
                            _expandedNamespaces.Add($"NAMESPACE:{ns}".ToLower());
                        }
                    }
                }
            }
        }

        public async Task LoadObjects(IEnumerable<ITrakHoundObjectEntity> objects)
        {
            if (_client != null)
            {
                if (!objects.IsNullOrEmpty())
                {
                    foreach (var obj in objects)
                    {
                        var objectUuid = obj.Uuid;
                        if (!string.IsNullOrEmpty(objectUuid))
                        {
                            _objects.Remove(objectUuid);
                            _objects.Add(objectUuid, obj);

                            if (!string.IsNullOrEmpty(obj.ParentUuid))
                            {
                                _objectParents.Add(obj.ParentUuid, obj.Uuid);
                            }
                        }
                    }

                    var targetUuids = objects.Select(o => o.Uuid);

                    // Get Child Count
                    var directoryUuids = objects?.Where(o => o.ContentType == TrakHoundObjectContentTypes.Directory)?.Select(o => o.Uuid);
                    var childCounts = await _client.System.Entities.Objects.QueryChildCount(directoryUuids);
                    if (!childCounts.IsNullOrEmpty())
                    {
                        lock (_lock)
                        {
                            foreach (var count in childCounts)
                            {
                                _objectChildCount.Remove(count.Target);
                                _objectChildCount.Add(count.Target, count.Count);
                            }
                        }
                    }

                    LoadContent(objects);

                    // Expand Namespaces
                    var namespaces = objects.Select(o => o.Namespace).Distinct();
                    lock (_lock)
                    {
                        foreach (var ns in namespaces)
                        {
                            _expandedNamespaces.Add($"NAMESPACE:{ns}".ToLower());
                        }
                    }
                }

                _targetUuids.Clear();
                if (!objects.IsNullOrEmpty())
                {
                    _targetUuids.AddRange(objects.Select(o => o.Uuid));
                }

                FilterTargetObjects();
            }
        }

        public async Task LoadObjectAllChildren(string uuid)
        {
            if (_client != null)
            {
                var childObjects = await _client.System.Entities.Objects.QueryChildrenByRootUuid(uuid, 0, 10000, SortOrder.Descending);
                if (!childObjects.IsNullOrEmpty())
                {
                    var obj = _objects.GetValueOrDefault(uuid);
                    if (obj != null)
                    {
                        foreach (var childObject in childObjects)
                        {
                            _objects.Remove(childObject.Uuid);
                            _objects.Add(childObject.Uuid, childObject);

                            if (!string.IsNullOrEmpty(childObject.ParentUuid))
                            {
                                _objectParents.Add(childObject.ParentUuid, childObject.Uuid);
                            }
                        }

                        var targetUuids = childObjects.Select(o => o.Uuid);

                        // Get Child Count
                        var directoryUuids = childObjects?.Where(o => o.ContentType == TrakHoundObjectContentTypes.Directory)?.Select(o => o.Uuid);
                        var childCounts = await _client.System.Entities.Objects.QueryChildCount(directoryUuids);
                        if (!childCounts.IsNullOrEmpty())
                        {
                            lock (_lock)
                            {
                                foreach (var count in childCounts)
                                {
                                    _objectChildCount.Remove(count.Target);
                                    _objectChildCount.Add(count.Target, count.Count);
                                }
                            }
                        }

                        LoadContent(childObjects);

                        // Expand Namespaces
                        var namespaces = childObjects.Select(o => o.Namespace).Distinct();
                        lock (_lock)
                        {
                            foreach (var ns in namespaces)
                            {
                                _expandedNamespaces.Add($"NAMESPACE:{ns}".ToLower());
                            }
                        }
                    }
                }
            }
        }

        private void FilterTargetObjects()
        {
            if (!_targetUuids.IsNullOrEmpty())
            {
                if (_hiddenShown)
                {
                    _filteredTargetUuids = _targetUuids;
                }
                else
                {
                    var shownUuids = new List<string>();
                    foreach (var targetUuid in _targetUuids)
                    {
                        var obj = _objects.GetValueOrDefault(targetUuid);
                        if (obj != null && !obj.Name.StartsWith('.')) shownUuids.Add(targetUuid);
                    }
                    _filteredTargetUuids = shownUuids;
                }
            }
            else
            {
                _filteredTargetUuids = _targetUuids;
            }

            if (TargetObjectsUpdated != null) TargetObjectsUpdated.Invoke(this, new EventArgs());
        }


        public void EditObject(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                _selectedObject = _objects.GetValueOrDefault(objectUuid);

                if (_selectedObject != null && EditClicked != null)
                {
                    EditClicked.Invoke(this, _selectedObject);
                }
            }
        }

        public void EditObject(ITrakHoundObjectEntity obj)
        {
            if (obj != null && EditClicked != null)
            {
                EditClicked.Invoke(this, obj);
            }
        }

        public void SelectObject(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                _selectedObject = _objects.GetValueOrDefault(objectUuid);

                if (_selectedObject != null && SelectedObjectChanged != null)
                {
                    SetLoadAdditional(_selectedObject);

                    SelectedObjectChanged.Invoke(this, _selectedObject);
                }
            }
            else
            {
                _selectedObject = null;
                if (SelectedObjectChanged != null) SelectedObjectChanged.Invoke(this, _selectedObject);
            }
        }

        public IEnumerable<ITrakHoundObjectEntity> GetTargetObjects()
        {
            if (!_filteredTargetUuids.IsNullOrEmpty())
            {
                var objects = new List<ITrakHoundObjectEntity>();

                foreach (var targetUuid in _filteredTargetUuids)
                {
                    var obj = _objects.GetValueOrDefault(targetUuid);
                    if (obj != null)
                    {
                        objects.Add(obj);
                    }
                }

                return objects.OrderBy(o => o.Path);
            }

            return null;
        }

        public TEntity GetContent<TEntity>(string objectUuid) where TEntity : ITrakHoundEntity
        {
            var entity = GetContent(objectUuid);
            if (entity != null)
            {
                return TrakHoundEntity.FromJson<TEntity>(entity.ToJson());
            }

            return default;
        }

        public ITrakHoundEntity GetContent(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                lock (_lock)
                {
                    if (_content.ContainsKey(objectUuid))
                    {
                        return _content[objectUuid];
                    }
                }
            }

            return null;
        }

        public string GetValue(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                lock (_lock)
                {
                    if (_values.ContainsKey(objectUuid))
                    {
                        return _values[objectUuid];
                    }
                }
            }

            return null;
        }

        public bool IsRecentValue(string objectUuid, long timestamp = 0)
        {
            var ts = timestamp > 0 ? timestamp : UnixDateTime.Now;

            if (!string.IsNullOrEmpty(objectUuid))
            {
                lock (_lock)
                {
                    if (_recentValues.ContainsKey(objectUuid))
                    {
                        return ts - _recentValues[objectUuid] < _recentLimit;
                    }
                }
            }

            return false;
        }

        public void UpdateObject(string objectUuid)
        {
            if (ObjectUpdated != null) ObjectUpdated.Invoke(this, objectUuid);
        }

        public void UpdateValue(string objectUuid)
        {
            //if (ValueUpdated != null) ValueUpdated.Invoke(this, objectUuid);
        }


        public async Task RefreshObject(ITrakHoundObjectEntity entity)
        {
            await LoadObject(entity.Uuid);
        }


        public async Task AddObject(ITrakHoundObjectEntity entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.Uuid))
            {
                var rootUuids = new HashSet<string>();

                lock (_lock)
                {
                    _objects.Remove(entity.Uuid);
                    _objects.Add(entity.Uuid, entity);

                    if (!string.IsNullOrEmpty(entity.ParentUuid))
                    {
                        _objectParents.Add(entity.ParentUuid, entity.Uuid);
                    }
                }

                rootUuids.Add(entity.Uuid);

                if (!string.IsNullOrEmpty(entity.ParentUuid))
                {
                    string parentUuid = entity.ParentUuid;
                    ITrakHoundObjectEntity parentObject = null;
                    bool parentFoundInTree = false;
                    bool rootParentReached = false;

                    while (!parentFoundInTree && !rootParentReached)
                    {
                        lock (_lock) parentObject = _objects.GetValueOrDefault(parentUuid);
                        if (parentObject == null)
                        {
                            // Read Missing Objects in Path
                            parentObject = await _client.System.Entities.Objects.ReadByUuid(parentUuid);
                            if (parentObject != null)
                            {
                                lock (_lock)
                                {
                                    _objects.Remove(parentObject.Uuid);
                                    _objects.Add(parentObject.Uuid, parentObject);

                                    if (!string.IsNullOrEmpty(parentObject.ParentUuid))
                                    {
                                        _objectParents.Add(parentObject.ParentUuid, parentObject.Uuid);
                                    }
                                    else
                                    {
                                        _targetUuids.Add(parentObject.Uuid);
                                        rootParentReached = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            parentFoundInTree = true;
                        }

                        if (parentObject != null)
                        {
                            rootUuids.Add(parentObject.Uuid);
                            parentUuid = parentObject.ParentUuid;
                        }
                        else break;
                    }
                }
                else
                {
                    lock (_lock)
                    {
                        _targetUuids.Add(entity.Uuid);
                    }
                }

                // Expand up to Root
                foreach (var uuid in rootUuids.Reverse())
                {
                    ExpandObject(uuid);
                }

                // Update Selected Object (if matches the updated Object)
                if (_selectedObject != null && _selectedObject.Uuid == entity.Uuid)
                {
                    _selectedObject = entity;
                }

                if (ObjectUpdated != null) ObjectUpdated.Invoke(this, entity.Uuid);

                FilterTargetObjects();
            }
        }


        public async Task<bool> AddMetadata(ITrakHoundObjectEntity objectEntity, string name, string value, string definitionId = null, string valueDefinitionId = null)
        {
            if (objectEntity != null && _client != null)
            {
                if (await _client.Entities.PublishMetadata(objectEntity.Uuid, name, value, definitionId, valueDefinitionId))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DeleteMetadata(string entityUuid, string name)
        {
            if (entityUuid != null && name != null)
            {
                var uuid = TrakHoundObjectMetadataEntity.GenerateUuid(entityUuid, name);
                if (await _client.System.Entities.Objects.Metadata.Delete(uuid, TrakHoundOperationMode.Sync))
                {
                    RemoveObjectMetadata(entityUuid, name);

                    return true;
                }
            }

            return false;
        }

        public async Task<TrakHoundDefinition> AddDefinition(string id, string description = null, string parentId = null)
        {
            if (_client != null && !string.IsNullOrEmpty(id))
            {
                if (await _client.Entities.PublishDefinition(id, description, parentId, false))
                {
                    return await _client.Entities.GetDefinition(id);
                }
            }

            return null;
        }


        public void RemoveObject(string uuid)
        { 
            if (!string.IsNullOrEmpty(uuid))
            {
                IEnumerable<string> childUuids = null;
                ITrakHoundObjectEntity targetObject = null;
                lock (_lock)
                {
                    targetObject = _objects.GetValueOrDefault(uuid);
                    if (targetObject != null)
                    {
                        // Remove Child Objects
                        var filter = targetObject.Path + "/";
                        childUuids = _objects.Values.Where(o => o.Path.StartsWith(filter))?.Select(o => o.Uuid);
                        if (!childUuids.IsNullOrEmpty())
                        {
                            foreach (var objectUuid in childUuids)
                            {
                                _objects.Remove(objectUuid);
                                _objectParents.Remove(objectUuid);
                                _objectMetadata.Remove(objectUuid);
                            }
                        }

                        // Remove Target Object
                        _objects.Remove(uuid);
                        _objectParents.Remove(uuid);
                        _objectMetadata.Remove(uuid);
                    }           
                }

                if (SelectedObject != null && SelectedObject.Uuid == targetObject.Uuid)
                {
                    SelectObject(null);
                }

                if (targetObject != null && string.IsNullOrEmpty(targetObject.ParentUuid))
                {
                    _targetUuids.Remove(uuid);
                    FilterTargetObjects();
                }
                else
                {
                    if (TargetObjectsUpdated != null) TargetObjectsUpdated.Invoke(this, EventArgs.Empty);
                }
            }
        }


        public void ExpandAll()
        {
            var uuids = new List<string>();
            var namespaces = new List<string>();

            lock (_lock)
            {
                foreach (var uuid in _targetUuids) uuids.Add(uuid);
                foreach (var ns in _expandedNamespaces) namespaces.Add(ns);
            }

            foreach (var uuid in uuids)
            {
                ExpandObject(uuid);
            }

            foreach (var ns in namespaces)
            {
                if (NamespaceExpanded != null) NamespaceExpanded.Invoke(this, ns);
            }
        }

        public void ExpandNamespace(string ns)
        {
            if (!string.IsNullOrEmpty(ns))
            {
                lock (_lock) _expandedNamespaces.Add(ns.ToLower());

                if (NamespaceExpanded != null) NamespaceExpanded.Invoke(this, ns);
            }
        }

        public void ExpandObject(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                lock (_lock) _expandedObjects.Add(uuid);

                if (ObjectExpanded != null) ObjectExpanded.Invoke(this, uuid);
            }
        }

        public void ExpandObjects(IEnumerable<string> uuids, bool refresh = true)
        {
            if (!uuids.IsNullOrEmpty())
            {
                lock (_lock)
                {
                   foreach (var uuid in uuids) _expandedObjects.Add(uuid);
                }
            }
        }

        public void CollapseAll()
        {
            var uuids = new List<string>();
            var namespaces = new List<string>();

            lock (_lock)
            {
                foreach (var uuid in _targetUuids) uuids.Add(uuid);
                foreach (var ns in _expandedNamespaces) namespaces.Add(ns);

                _objectParents.Clear();
                _expandedObjects.Clear();
                _expandedNamespaces.Clear();
            }

            foreach (var uuid in uuids)
            {
                if (ObjectCollapsed != null) ObjectCollapsed.Invoke(this, uuid);
            }

            foreach (var ns in namespaces)
            {
                if (NamespaceCollapsed != null) NamespaceCollapsed.Invoke(this, ns);
            }
        }

        public void CollapseNamespace(string ns)
        {
            if (!string.IsNullOrEmpty(ns))
            {
                lock (_lock) _expandedNamespaces.Remove(ns.ToLower());

                if (NamespaceCollapsed != null) NamespaceCollapsed.Invoke(this, ns);
            }
        }

        public void CollapseObject(string uuid, bool refresh = true)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var childUuids = _objectParents.Get(uuid);
                if (!childUuids.IsNullOrEmpty())
                {
                    foreach (var childUuid in childUuids)
                    {
                        _objects.Remove(childUuid);

                        CollapseObject(childUuid, false);
                    }
                }

                lock (_lock) _expandedObjects.Remove(uuid);

                if (refresh && ObjectCollapsed != null) ObjectCollapsed.Invoke(this, uuid);
            }
        }


        public async Task<bool> Copy()
        {
            if (_selectedObject != null)
            {
                _clipboardId = Guid.NewGuid().ToString();
                return await Client.Entities.CopyToClipboard(_clipboardId, _selectedObject.Path);
            }

            return false;
        }

        public async Task<bool> Copy(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                _clipboardId = Guid.NewGuid().ToString();
                return await Client.Entities.CopyToClipboard(_clipboardId, path);
            }

            return false;
        }

        public async Task<bool> Paste(string destinationBasePath)
        {
            if (!string.IsNullOrEmpty(_clipboardId) && !string.IsNullOrEmpty(destinationBasePath))
            {
                if (await Client.Entities.PasteClipboard(_clipboardId, destinationBasePath))
                {
                    await Client.Entities.DeleteClipboard(_clipboardId);

                    var objectUuid = TrakHoundPath.GetUuid(destinationBasePath);
                    await LoadObject(objectUuid);
                    ExpandObject(objectUuid);

                    return true;
                }
            }

            return false;
        }

        public void ToggleHidden()
        {
            _hiddenShown = !_hiddenShown;

            FilterTargetObjects();
        }


        public void LoadContent(ITrakHoundObjectEntity selectedObject)
        {
            if (selectedObject != null)
            {
                LoadContent(new ITrakHoundObjectEntity[] { selectedObject });
            }
        }

        public void LoadContent(IEnumerable<ITrakHoundObjectEntity> selectedObjs)
        {
            if (!selectedObjs.IsNullOrEmpty())
            {
                _ = Task.Run(() => LoadContentWorker(selectedObjs));
            }
        }

        //private void ContentUpdateThrottleElapsed(object sender, EventArgs e)
        //{
        //    var updates = new List<ContentUpdate>();
        //    for (var i = 0; i < 100; i++)
        //    {
        //        _contentUpdated.TryDequeue(out var update);
        //        if (update != null) updates.Add(update);
        //        else break;
        //    }

        //    if (!updates.IsNullOrEmpty())
        //    {
        //        var now = UnixDateTime.Now;

        //        //var objectUuids = new HashSet<string>();
        //        //lock (_lock)
        //        //{
        //        //    foreach (var update in updates)
        //        //    {
        //        //        //var ts = update.Timestamp > 0 ? update.Timestamp : now;

        //        //        //if (update.Entity != null)
        //        //        //{
        //        //        //    _content.Remove(update.ObjectUuid);
        //        //        //    _content.Add(update.ObjectUuid, update.Entity);
        //        //        //}

        //        //        //_values.Remove(update.ObjectUuid);
        //        //        //_values.Add(update.ObjectUuid, update.Value?.Trim());

        //        //        //if (now - ts < _recentLimit)
        //        //        //{
        //        //        //    _recentValues.Remove(update.ObjectUuid);
        //        //        //    _recentValues.Add(update.ObjectUuid, ts);
        //        //        //}

        //        //        objectUuids.Add(update.ObjectUuid);
        //        //    }
        //        //}

        //        var objectUuids = new HashSet<string>();
        //        foreach (var update in updates)
        //        {
        //            objectUuids.Add(update.ObjectUuid);
        //        }

        //        foreach (var objectUuid in objectUuids)
        //        {
        //            if (ValueUpdated != null) ValueUpdated.Invoke(this, objectUuid);
        //        }

        //        //if (objectUuids.Count() > 5)
        //        //{
        //        //    //if (Updated != null) Updated.Invoke(this, EventArgs.Empty);
        //        //}
        //        //else
        //        //{
        //        //    foreach (var objectUuid in objectUuids)
        //        //    {
        //        //        if (ValueUpdated != null) ValueUpdated.Invoke(this, objectUuid);
        //        //    }
        //        //}
        //    }
        //}

        private void ConsumerValueUpdated(string objectUuid, ITrakHoundEntity entity, string value, long timestamp = 0)
        {
            if (objectUuid != null)
            {
                var now = UnixDateTime.Now;
                var ts = timestamp > 0 ? timestamp : now;

                lock (_lock)
                {
                    if (entity != null)
                    {
                        _content.Remove(objectUuid);
                        _content.Add(objectUuid, entity);
                    }

                    _values.Remove(objectUuid);
                    _values.Add(objectUuid, value?.Trim());

                    if (now - ts < _recentLimit)
                    {
                        _recentValues.Remove(objectUuid);
                        _recentValues.Add(objectUuid, ts);
                    }
                }

                if (ValueUpdated != null) ValueUpdated.Invoke(this, objectUuid);
            }



            //var update = new ContentUpdate();
            //update.ObjectUuid = objectUuid;
            ////update.Entity = entity;
            ////update.Value = value;
            //update.Timestamp = timestamp;

            //_contentUpdated.Enqueue(update);
            //_contentUpdateThrottle.Set();
        }

        private async Task LoadContentWorker(IEnumerable<ITrakHoundObjectEntity> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                if (!_contentConsumers.IsNullOrEmpty())
                {
                    foreach (var consumer in _contentConsumers) consumer.Dispose();
                    _contentConsumers.Clear();
                }

                if (_client != null)
                {
                    var tasks = new List<Task>();

                    var contentTypes = new TrakHoundObjectContentTypeDictionary();
                    contentTypes.Add(objs);


                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.Assignment))
                    {
                        var assignmentConsumer = new ObjectAssignmentConsumer(_client, _consumerId);
                        assignmentConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(assignmentConsumer);
                        tasks.Add(assignmentConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.Assignment)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.Blob))
                    {
                        var blobConsumer = new ObjectBlobConsumer(_client, _consumerId);
                        blobConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(blobConsumer);
                        tasks.Add(blobConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.Blob)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.Boolean))
                    {
                        var booleanConsumer = new ObjectBooleanConsumer(_client, _consumerId);
                        booleanConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(booleanConsumer);
                        tasks.Add(booleanConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.Boolean)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.Duration))
                    {
                        var durationConsumer = new ObjectDurationConsumer(_client, _consumerId);
                        durationConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(durationConsumer);
                        tasks.Add(durationConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.Duration)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.Event))
                    {
                        var eventConsumer = new ObjectEventConsumer(_client, _consumerId);
                        eventConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(eventConsumer);
                        tasks.Add(eventConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.Event)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.Message))
                    {
                        var messageConsumer = new ObjectMessageConsumer(_client, _consumerId);
                        messageConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(messageConsumer);
                        tasks.Add(messageConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.Message)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.Number))
                    {
                        var numberConsumer = new ObjectNumberConsumer(_client, _consumerId);
                        numberConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(numberConsumer);
                        tasks.Add(numberConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.Number)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.Observation))
                    {
                        var observationConsumer = new ObjectObservationConsumer(_client, _consumerId);
                        observationConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(observationConsumer);
                        tasks.Add(observationConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.Observation)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.Reference))
                    {
                        var referenceConsumer = new ObjectReferenceConsumer(_client, _consumerId);
                        referenceConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(referenceConsumer);
                        tasks.Add(referenceConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.Reference)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.State))
                    {
                        var stateConsumer = new ObjectStateConsumer(_client, _consumerId);
                        stateConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(stateConsumer);
                        tasks.Add(stateConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.State)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.String))
                    {
                        var stringConsumer = new ObjectStringConsumer(_client, _consumerId);
                        stringConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(stringConsumer);
                        tasks.Add(stringConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.String)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.Timestamp))
                    {
                        var timestampConsumer = new ObjectTimestampConsumer(_client, _consumerId);
                        timestampConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(timestampConsumer);
                        tasks.Add(timestampConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.Timestamp)));
                    }

                    if (contentTypes.ContentTypes.Contains(TrakHoundObjectContentType.Vocabulary))
                    {
                        var vocabularyConsumer = new ObjectVocabularyConsumer(_client, _consumerId);
                        vocabularyConsumer.ValueUpdated += ConsumerValueUpdated;
                        _contentConsumers.Add(vocabularyConsumer);
                        tasks.Add(vocabularyConsumer.Load(contentTypes.Get(TrakHoundObjectContentType.Vocabulary)));
                    }

                    await Task.WhenAll(tasks);
                }
            }
        }


        public void SetLoadAdditional(ITrakHoundObjectEntity selectedObject)
        {
            if (selectedObject != null)
            {
                _selectedObjectLoadDelay.Set();
            }
        }

        private void SelectedObjectLoadDelayElapsed(object sender, EventArgs e)
        {
            if (_selectedObject != null)
            {
                _ = Task.Run(() => LoadAdditional(_selectedObject));
            }
        }

        public async Task LoadAdditional(ITrakHoundObjectEntity obj)
        {
            if (obj != null)
            {
                _definitionParents.Remove(obj.DefinitionUuid);

                var definitionUuids = new HashSet<string>();
                if (!string.IsNullOrEmpty(obj.DefinitionUuid)) definitionUuids.Add(obj.DefinitionUuid);

                // Load Object Metadata
                var metadataEntities = await _client.System.Entities.Objects.Metadata.QueryByEntityUuid(obj.Uuid);
                if (!metadataEntities.IsNullOrEmpty())
                {
                    foreach (var metadataEntity in metadataEntities)
                    {
                        if (!string.IsNullOrEmpty(metadataEntity.DefinitionUuid)) definitionUuids.Add(metadataEntity.DefinitionUuid);
                        if (!string.IsNullOrEmpty(metadataEntity.ValueDefinitionUuid)) definitionUuids.Add(metadataEntity.ValueDefinitionUuid);

                        _objectMetadata.Add(metadataEntity.EntityUuid, metadataEntity);
                    }
                }

                // Load Definitions
                if (!definitionUuids.IsNullOrEmpty())
                {
                    var definitions = await _client.System.Entities.Definitions.ReadByUuid(definitionUuids);
                    if (!definitions.IsNullOrEmpty())
                    {
                        var additionalDefinitionUuids = new HashSet<string>();

                        foreach (var definition in definitions)
                        {
                            _definitions.Remove(definition.Uuid);
                            _definitions.Add(definition.Uuid, definition);

                            additionalDefinitionUuids.Add(definition.Uuid);
                        }

                        // Load Inherited Types for Object
                        if (!string.IsNullOrEmpty(obj.DefinitionUuid))
                        {
                            var inheritedDefinitions = await _client.System.Entities.Definitions.QueryByChildUuid(obj.DefinitionUuid);
                            if (!inheritedDefinitions.IsNullOrEmpty())
                            {
                                foreach (var definition in inheritedDefinitions)
                                {
                                    _definitions.Remove(definition.Uuid);
                                    _definitions.Add(definition.Uuid, definition);

                                    _definitionParents.Add(obj.DefinitionUuid, definition.Uuid);
                                    additionalDefinitionUuids.Add(definition.Uuid);
                                }
                            }
                        }

                        // Load Definition Descriptions
                        var definitionDescriptions = await _client.System.Entities.Definitions.Description.QueryByDefinitionUuid(additionalDefinitionUuids);
                        if (!definitionDescriptions.IsNullOrEmpty())
                        {
                            foreach (var definitionDescription in definitionDescriptions)
                            {
                                if (definitionDescription.LanguageCode == "en")
                                {
                                    _definitionDescriptions.Remove(definitionDescription.DefinitionUuid);
                                    _definitionDescriptions.Add(definitionDescription.DefinitionUuid, definitionDescription);
                                }
                            }
                        }
                    }
                }

                // Load Source Chain
                if (!string.IsNullOrEmpty(obj.SourceUuid))
                {
                    var sources = await _client.System.Entities.Sources.QueryChain(obj.SourceUuid);
                    if (!sources.IsNullOrEmpty())
                    {
                        foreach (var source in sources)
                        {
                            _sourceChains.Add(obj.SourceUuid, source);
                        }
                    }
                }

                if (SelectedObjectChanged != null) SelectedObjectChanged(this, obj);
            }
        }


        private void ProcessRecentValues(object sender, System.Timers.ElapsedEventArgs args)
        {
            var now = UnixDateTime.Now;
            var updatedObjectUuids = new List<string>();

            lock (_lock)
            {
                var keys = _recentValues.Keys.ToList();
                foreach (var key in keys)
                {
                    var value = _recentValues[key];
                    if (now - value > _recentLimit)
                    {
                        updatedObjectUuids.Add(key);
                        _recentValues.Remove(key);
                    }
                }
            }

            if (!updatedObjectUuids.IsNullOrEmpty())
            {
                foreach (var objectUuid in updatedObjectUuids)
                {
                    UpdateValue(objectUuid);
                }
            }
        }
    }
}
