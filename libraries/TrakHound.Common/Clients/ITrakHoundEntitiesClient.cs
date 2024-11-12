// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        private const int _defaultSkip = 0;
        private const int _defaultTake = 1000;
        private const SortOrder _defaultSortOrder = SortOrder.Ascending;


        IEnumerable<ITrakHoundClientMiddleware> Middleware { get; }


        Task<IEnumerable<TReturn>> Query<TReturn>(string query);

        Task<IEnumerable<TrakHoundObject>> QueryObjects(string query, int skip = _defaultSkip, int take = _defaultTake);

        Task<TReturn> GetSingle<TReturn>(string path);

        Task<IEnumerable<TReturn>> Get<TReturn>(string path, int skip = _defaultSkip, int take = _defaultTake);

        Task<IEnumerable<TReturn>> Get<TReturn>(IEnumerable<string> paths, int skip = _defaultSkip, int take = _defaultTake);

        Task<ITrakHoundConsumer<IEnumerable<TReturn>>> Subscribe<TReturn>(string expression, int interval = 1000, int count = 1000);

        Task<TReturn> GetByObjectUuid<TReturn>(string objectUuid);

        Task<IEnumerable<TReturn>> GetByObjectUuid<TReturn>(IEnumerable<string> objectUuids);

        Task<TrakHoundEntityCollection> GetObjectContent(IEnumerable<ITrakHoundObjectEntity> targetObjs, IEnumerable<ITrakHoundObjectEntity> contentObjs);


        Task<bool> Publish(object model, bool async = false, string routerId = null);

        Task<bool> Publish(TrakHoundEntityTransaction collection, bool async = false, string routerId = null);


        Task<string> GetJson(string basePath, string routerId = null);

        Task<TResult> GetJson<TResult>(string basePath, string routerId = null);

        Task<bool> PublishJson(string basePath, string json, bool async = false, string routerId = null);


        Task<bool> CopyToClipboard(string clipboardId, string path);

        Task<bool> PasteClipboard(string clipboardId, string destinationBasePath);

        Task<bool> DeleteClipboard(string clipboardId);


        void AddMiddleware(ITrakHoundClientMiddleware middleware);
    }
}
