// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Blazor.Components;
using TrakHound.Blazor.ExplorerInternal.Services;
using TrakHound.Clients;
using TrakHound.Entities.QueryEngines;

namespace TrakHound.Blazor.ExplorerInternal.Components.Entities.Objects
{
    public class ExplorerObjectsService
    {
        private const string _defaultHomeQuery = "select >> from [/];";

        private readonly ExplorerService _explorerService;
        private readonly string _serverId;
        private readonly string _routerId;
        private readonly ITrakHoundClient _client;

        private string _query;
        private string _queryId;
        private string _expression;
        private string _uuid;

        private string _previousQuery;
        private string _previousQueryId;
        private string _previousExpression;
        private string _previousUuid;

        private bool _isQueryLoading;
        private QueryResponseInformation _queryResponseInformation;
        private TrakHoundQueryResponse _queryResponse;
        private ITrakHoundConsumer<TrakHoundQueryResponse> _queryConsumer;

        private IEnumerable<QueryExplorer.QueryInformation> _savedQueries;


        public ExplorerService ExplorerService => _explorerService;

        public string ServerId => _serverId;

        public string RouterId => _routerId;

        public ITrakHoundClient Client => _client;


        public string Query => _query;

        public string QueryId => _queryId;


        public bool IsQueryLoading => _isQueryLoading;

        public QueryResponseInformation QueryResponseInformation => _queryResponseInformation;

        public TrakHoundQueryResponse QueryResponse => _queryResponse;

        public ITrakHoundConsumer<TrakHoundQueryResponse> QueryConsumer => _queryConsumer;

        public IEnumerable<QueryExplorer.QueryInformation> SavedQueries => _savedQueries;


        public event EventHandler QueryUpdated;

        public event EventHandler QueryLoading;

        public event EventHandler QueryLoaded;

        public event EventHandler QueriesUpdated;

        public event EventHandler<QueryExplorer.QueryInformation> QuerySaved;

        public event EventHandler<QueryExplorer.QueryInformation> SavedQuerySelected;

        public event EventHandler<string> SaveQueryClicked;


        public ExplorerObjectsService(ITrakHoundClient client)
        {
            _client = client;

            //LoadSavedQueries();
        }


        #region "Query"

        public void SetQuery(string query)
        {
            _queryId = null;
            _expression = null;
            _uuid = null;
            _query = query;

            if (QueryUpdated != null) QueryUpdated.Invoke(this, EventArgs.Empty);
        }

        public void SetQueryId(string queryId)
        {
            _expression = null;
            _uuid = null;
            _queryId = queryId;
        }

        public void SetExpression(string expression)
        {
            _query = null;
            _queryId = null;
            _uuid = null;
            _expression = expression;
        }

        public void SetUuid(string uuid)
        {
            _query = null;
            _queryId = null;
            _expression = null;
            _uuid = uuid;
        }

        #endregion

        #region "Load"

        public async Task LoadByQuery(bool forceReload = false)
        {
            if (forceReload || Query != _previousQuery)
            {
                if (_queryConsumer != null)
                {
                    _queryConsumer.Received -= QueryConsumerReceived;
                    _queryConsumer.Dispose();
                    _queryConsumer = null;
                }

                if (string.IsNullOrEmpty(_query)) _query = _defaultHomeQuery;

                _queryResponse = new TrakHoundQueryResponse();
                _isQueryLoading = true;
                if (QueryLoading != null) QueryLoading.Invoke(this, EventArgs.Empty);

                if (Client != null)
                {
                    var stpw = System.Diagnostics.Stopwatch.StartNew();
                    _queryResponse = await Client.System.Entities.Query(_query);
                    stpw.Stop();

                    _queryResponseInformation = new QueryResponseInformation();
                    _queryResponseInformation.Success = _queryResponse.Success;
                    _queryResponseInformation.Count = GetResultCount(_queryResponse);
                    _queryResponseInformation.Duration = stpw.Elapsed;
                }

                _previousQuery = _query;

                _isQueryLoading = false;
                if (QueryLoaded != null) QueryLoaded.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task LoadByQueryId(string queryId, bool forceReload = false)
        {
            if (!string.IsNullOrEmpty(queryId))
            {
                if (forceReload || _queryId != _previousQueryId)
                {
                    _previousQueryId = _queryId;

                    if (_queryConsumer != null)
                    {
                        _queryConsumer.Received -= QueryConsumerReceived;
                        _queryConsumer.Dispose();
                        _queryConsumer = null;
                    }

                    var queryInformation = await Client.Api.QueryJson<QueryExplorer.QueryInformation>($"queries/{queryId}");
                    await LoadByQuery(queryInformation);
                }
            }
        }

        public async Task LoadByQuery(QueryExplorer.QueryInformation queryInformation, bool forceReload = false)
        {
            if (queryInformation != null)
            {
                //_queryId = queryInformation.Id;
                //_query = queryInformation.Query;
                //_previousQueryId = QueryId;
                //_previousQuery = Query;

                //_queryResponse = null;
                //_isQueryLoading = true;
                //if (QueryLoading != null) QueryLoading.Invoke(this, EventArgs.Empty);

                //var stpw = System.Diagnostics.Stopwatch.StartNew();
                //_queryResponse = await Client.System.Entities.Query(queryInformation.Query);
                //stpw.Stop();

                //_queryResponseInformation = new QueryResponseInformation();
                //_queryResponseInformation.Success = _queryResponse != null;
                //if (_queryResponse != null)
                //{
                //    _queryResponseInformation.Count = GetResultCount(_queryResponse);
                //}
                //_queryResponseInformation.Duration = stpw.Elapsed;

                //if (SavedQuerySelected != null) SavedQuerySelected.Invoke(this, queryInformation);

                //_isQueryLoading = false;
                //if (QueryLoaded != null) QueryLoaded.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task LoadHomeQuery(bool forceReload = false)
        {
            var homeQuery = await GetHomeQuery();
            if (!string.IsNullOrEmpty(homeQuery))
            {
                _query = homeQuery;
                await LoadByQuery(forceReload);
            }
            else
            {
                _query = _defaultHomeQuery;
                await LoadByQuery(forceReload);
            }
        }

        private static int GetResultCount(TrakHoundQueryResponse response)
        {
            var count = 0;

            if (!response.Results.IsNullOrEmpty())
            {
                foreach (var result in response.Results)
                {
                    if (result.Rows != null)
                    {
                        count += result.Rows.GetLength(0); // Get Row Count
                    }
                }
            }

            return count;
        }

        #endregion

        #region "Subscribe"

        public async Task SubscribeByQuery()
        {
            if (!string.IsNullOrEmpty(_query))
            {
                _queryResponse = new TrakHoundQueryResponse();
                _isQueryLoading = true;
                if (QueryLoading != null) QueryLoading.Invoke(this, EventArgs.Empty);

                var stpw = System.Diagnostics.Stopwatch.StartNew();
                _queryConsumer = await Client?.System.Entities.Subscribe(_query);
                stpw.Stop();

                if (_queryConsumer != null)
                {
                    _queryConsumer.Received += QueryConsumerReceived;
                }

                _previousQuery = _query;

                _isQueryLoading = false;
                if (QueryLoaded != null) QueryLoaded.Invoke(this, EventArgs.Empty);
            }
        }

        public void Unsubscribe()
        {
            if (_queryConsumer != null)
            {
                _queryConsumer.Received -= QueryConsumerReceived;
                _queryConsumer.Dispose();
                _queryConsumer = null;
            }

            if (QueryLoaded != null) QueryLoaded.Invoke(this, EventArgs.Empty);
        }

        private void QueryConsumerReceived(object sender, TrakHoundQueryResponse queryResponse)
        {
            _queryResponse = queryResponse;

            _queryResponseInformation = new QueryResponseInformation();
            _queryResponseInformation.Success = _queryResponse.Success;
            _queryResponseInformation.Count = GetResultCount(_queryResponse);

            if (QueryLoaded != null) QueryLoaded.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region "Saved Queries"

        public async Task LoadSavedQueries()
        {
            if (Client != null)
            {
                _savedQueries = await Client.Api.QueryJson<IEnumerable<QueryExplorer.QueryInformation>>("queries");

                if (QueriesUpdated != null) QueriesUpdated.Invoke(this, EventArgs.Empty);
            }
        }

        public void SaveQuery()
        {
            if (SaveQueryClicked != null) SaveQueryClicked.Invoke(this, Query);
        }

        public async Task<string> GetHomeQuery()
        {
            if (Client != null)
            {
                var homeQuery = await Client.Api.QueryString("queries/home");

                if (string.IsNullOrEmpty(homeQuery)) homeQuery = _defaultHomeQuery;

                return homeQuery;
            }

            return null;
        }

        #endregion

    }
}
