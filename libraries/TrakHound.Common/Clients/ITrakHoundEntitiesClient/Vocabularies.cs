// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundVocabulary> GetVocabulary(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundVocabulary>> GetVocabularies(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundVocabulary>> GetVocabularies(IEnumerable<string> objectPaths, string routerId = null);


        Task<bool> PublishVocabulary(string objectPath, string definitionId, bool async = false, string routerId = null);

        Task<bool> PublishVocabularies(IEnumerable<TrakHoundVocabularyEntry> entries, bool async = false, string routerId = null);
    }
}
