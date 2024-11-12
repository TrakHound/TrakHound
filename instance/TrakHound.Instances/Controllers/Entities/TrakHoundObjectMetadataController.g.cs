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
    public partial class TrakHoundObjectMetadataController : TrakHoundObjectMetadataControllerBase
    {
        public TrakHoundObjectMetadataController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }



        protected async override Task<IActionResult> OnQueryByEntityUuid(
            ITrakHoundClient client,
            string entityUuid,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Metadata.QueryByEntityUuid(
            entityUuid);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnQueryByEntityUuid(
            ITrakHoundClient client,
            IEnumerable<string> entityUuids,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Metadata.QueryByEntityUuid(
            entityUuids);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnQueryByEntityUuid(
            ITrakHoundClient client,
            string entityUuid,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Metadata.QueryByEntityUuid(
            entityUuid,
            name,
            queryType,
            query);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnQueryByEntityUuid(
            ITrakHoundClient client,
            IEnumerable<string> entityUuids,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Metadata.QueryByEntityUuid(
            entityUuids,
            name,
            queryType,
            query);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnQueryByName(
            ITrakHoundClient client,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Metadata.QueryByName(
            name,
            queryType,
            query);

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
            var result = await client.System.Entities.Objects.Metadata.DeleteByObjectUuid(
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
            var result = await client.System.Entities.Objects.Metadata.DeleteByObjectUuid(
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
