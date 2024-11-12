// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundObject> GetObject(string path, string routerId = null);

        Task<IEnumerable<TrakHoundObject>> GetObjects(string path, int skip = 0, int take = 1000, string routerId = null);

        Task<IEnumerable<TrakHoundObject>> GetObjects(IEnumerable<string> paths, int skip = 0, int take = 1000, string routerId = null);


        Task<IEnumerable<TrakHoundObject>> GetObjectsByParentUuid(string parentUuid, string routerId = null);

        Task<IEnumerable<TrakHoundObject>> GetObjectsByParentUuid(IEnumerable<string> parentUuids, string routerId = null);


        Task<Dictionary<string, string>> GetObjectUuids(string path, int skip = _defaultSkip, int take = _defaultTake, string routerId = null);

        Task<Dictionary<string, string>> GetObjectUuids(IEnumerable<string> paths, int skip = _defaultSkip, int take = _defaultTake, string routerId = null);

        Task<Dictionary<string, string>> GetObjectContentTypes(string path, int skip = _defaultSkip, int take = _defaultTake, string routerId = null);

        Task<Dictionary<string, string>> GetObjectContentTypes(IEnumerable<string> paths, int skip = _defaultSkip, int take = _defaultTake, string routerId = null);


        Task<TrakHoundObjectResponse> PublishObject(
            string path,
            TrakHoundObjectContentType contentType = TrakHoundObjectContentType.Directory,
            string definitionId = null,
            Dictionary<string, string> metadata = null, 
            bool async = false, 
            string routerId = null
            );

        Task<TrakHoundObjectResponse> PublishObject(TrakHoundObjectEntry entry, bool async = false, string routerId = null);

        Task<TrakHoundObjectResponse> PublishObject(IEnumerable<TrakHoundObjectEntry> entries, bool async = false, string routerId = null);


        Task<bool> DeleteObject(string path, string routerId = null);

        Task<bool> DeleteObjects(IEnumerable<string> paths, string routerId = null);
    }
}
