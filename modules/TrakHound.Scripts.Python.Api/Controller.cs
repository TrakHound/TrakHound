// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Api;

namespace TrakHound.Scripts.Python.Api
{
    public class Controller : TrakHoundApiController
    {
        private const string _scriptBasePath = "scripts";
        private const string _functionId = "TrakHound.Scripts.Python.Function";


        [TrakHoundApiQuery("list")]
        public async Task<TrakHoundApiResponse> ListScripts()
        {
            var scriptFiles = await Volume.ListFiles(_scriptBasePath);
            if (!scriptFiles.IsNullOrEmpty())
            {
                return Ok(scriptFiles);
            }
            else
            {
                return NotFound("No Script Files Found");
            }
        }

        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetScript([FromQuery] string scriptPath)
        {
            var contents = await Volume.ReadString(Path.Combine(_scriptBasePath, scriptPath));
            if (contents != null)
            {
                return Ok(contents);
            }
            else
            {
                return NotFound($"Script File Not Found : scriptPath = {scriptPath}");
            }
        }

        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> AddScript([FromQuery] string scriptPath, [FromBody] string scriptContent)
        {
            if (!string.IsNullOrEmpty(scriptPath) && !string.IsNullOrEmpty(scriptContent))
            {
                if (await Volume.WriteString(Path.Combine(_scriptBasePath, scriptPath), scriptContent))
                {
                    return Ok($"Script Added Successfully : scriptPath = {scriptPath}");
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

        [TrakHoundApiQuery("run")]
        public async Task<TrakHoundApiResponse> RunScript(
            [FromQuery] string scriptPath,
            [FromBody("application/json")] Dictionary<string, string> parameters
            )
        {
            if (!string.IsNullOrEmpty(scriptPath))
            {
                var runParameters = new Dictionary<string, string>();
                runParameters.Add("scriptPath", scriptPath);

                if (!parameters.IsNullOrEmpty())
                {
                    foreach (var parameter in parameters)
                    {
                        if (!runParameters.ContainsKey(parameter.Key))
                        {
                            runParameters.Add(parameter.Key, parameter.Value);
                        }
                    }
                }

                var runResponse = await Client.Functions.Run(_functionId, runParameters);

                var apiResponse = new TrakHoundApiResponse();
                apiResponse.Success = true;
                apiResponse.StatusCode = runResponse.StatusCode;
                apiResponse.ContentType = "application/json";
                apiResponse.Content = TrakHoundApiResponse.GetJsonContentStream(runResponse.Parameters);
                return apiResponse;
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
