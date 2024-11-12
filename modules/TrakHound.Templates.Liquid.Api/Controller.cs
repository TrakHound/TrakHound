// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Fluid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Api;

namespace TrakHound.Templates.Liquid.Api
{
    public class Controller : TrakHoundApiController
    {
        private static readonly FluidParser _parser = new FluidParser();
        private readonly string _templatesDirectory = "templates";


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> Get(
            [FromQuery] string templateId,
            [FromBody(ContentType = "application/json")] Dictionary<string, object> model
            )
        {
            if (!string.IsNullOrEmpty(templateId))
            {
                var templatePath = Path.Combine(_templatesDirectory, templateId);
                var templateBody = await Volume.ReadString(templatePath);
                if (!string.IsNullOrEmpty(templateBody))
                {
                    try
                    {
                        if (_parser.TryParse(templateBody, out var template, out var error))
                        {
                            var context = new TemplateContext(model);

                            return Ok(template.Render(context));
                        }
                        else
                        {
                            return InternalError(error);
                        }
                    }
                    catch (Exception ex)
                    {
                        return InternalError(ex.Message);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiPublish("templates")]
        public async Task<TrakHoundApiResponse> AddTemplate(
            [FromQuery] string templateId,
            [FromBody] string templateBody
            )
        {
            if (!string.IsNullOrEmpty(templateId) && !string.IsNullOrEmpty(templateBody))
            {
                var path = Path.Combine(_templatesDirectory, templateId);
                if (await Volume.WriteString(path, templateBody))
                {
                    return Created();
                }
                else
                {
                    return InternalError();
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
