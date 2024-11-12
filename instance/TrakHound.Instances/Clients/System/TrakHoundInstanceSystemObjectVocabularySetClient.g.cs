// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectVocabularySetClient : TrakHoundInstanceEntityClient<ITrakHoundObjectVocabularySetEntity>, ITrakHoundSystemObjectVocabularySetClient
    {


        public TrakHoundInstanceSystemObjectVocabularySetClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectVocabularySetEntity>> QueryByObject(
            string path,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.VocabularySets.QueryByObject(new string[] { path },skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectVocabularySetEntity>> QueryByObjectUuid(
            string objectUuid,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.VocabularySets.QueryByObjectUuid(new string[] { objectUuid },skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectVocabularySetEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.VocabularySets.QueryByObjectUuid(objectUuids,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<bool> DeleteByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.VocabularySets.DeleteByObjectUuid(new string[] { objectUuid });
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

        public async Task<bool> DeleteByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.VocabularySets.DeleteByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

    }
}
