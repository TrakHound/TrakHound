// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<IEnumerable<TrakHoundVocabularySet>> GetVocabularySets(string objectPath, string routerId = null);


        Task<bool> PublishVocabularySet(string objectPath, string definitionId, bool async = false, string routerId = null);

        Task<bool> PublishVocabularySets(IEnumerable<TrakHoundVocabularySetEntry> entries, bool async = false, string routerId = null);
    }
}
