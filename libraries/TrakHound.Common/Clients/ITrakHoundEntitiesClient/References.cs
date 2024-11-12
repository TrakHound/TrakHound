// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundReference> GetReference(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundReference>> GetReferences(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundReference>> GetReferences(IEnumerable<string> objectPaths, string routerId = null);


        Task<string> GetReferenceTarget(string objectPath, string routerId = null);


        Task<bool> PublishReference(string objectPath, string targetPath, bool async = false, string routerId = null);

        Task<bool> PublishReferences(IEnumerable<TrakHoundReferenceEntry> entries, bool async = false, string routerId = null);
    }
}
