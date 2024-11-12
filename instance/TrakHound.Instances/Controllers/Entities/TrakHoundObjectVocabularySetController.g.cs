// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Instances;

namespace TrakHound.Http.Entities
{
    public partial class TrakHoundObjectVocabularySetController : TrakHoundObjectVocabularySetControllerBase
    {
        public TrakHoundObjectVocabularySetController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }



        protected async override Task<IActionResult> OnQueryByObject(
            ITrakHoundClient client,
            string path,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.VocabularySet.QueryByObject(
            path,
            skip,
            take,
            sortOrder);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnQueryByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.VocabularySet.QueryByObjectUuid(
            objectUuid,
            skip,
            take,
            sortOrder);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnQueryByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.VocabularySet.QueryByObjectUuid(
            objectUuids,
            skip,
            take,
            sortOrder);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnDeleteByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.VocabularySet.DeleteByObjectUuid(
            objectUuid);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }



        protected async override Task<IActionResult> OnDeleteByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.VocabularySet.DeleteByObjectUuid(
            objectUuids);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }
    }
}
