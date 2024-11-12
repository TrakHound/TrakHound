// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Functions;
using TrakHound.Volumes;

namespace TrakHound.Scripts.Python.Function
{
    public class Function : TrakHoundFunction
    {
        private const string _functionName = "_run";
        private const string _scriptBasePath = "scripts";
        private const string _scriptPathKey = "scriptPath";
        private readonly Microsoft.Scripting.Hosting.ScriptEngine _pythonEngine;


        public class RunRequest
        {
            public IReadOnlyDictionary<string, string> Parameters { get; set; }
            public ITrakHoundClient Client { get; set; }
        }


        public Function(ITrakHoundFunctionConfiguration configuration, ITrakHoundClient client, ITrakHoundVolume volume)
            : base(configuration, client, volume) 
        {
            _pythonEngine = IronPython.Hosting.Python.CreateEngine();
        }

        protected async override Task<TrakHoundFunctionResponse> OnRun(IReadOnlyDictionary<string, string> parameters)
        {
            Log(TrakHoundLogLevel.Information, "Running TrakHound.Python.Function Function..");

            var scriptPath = parameters?.GetValueOrDefault(_scriptPathKey);
            if (!string.IsNullOrEmpty(scriptPath))
            {
                var src = await Volume.ReadString(Path.Combine(_scriptBasePath, scriptPath));
                if (!string.IsNullOrEmpty(src))
                {
                    var scope = _pythonEngine.CreateScope();
                    _pythonEngine.Execute(src, scope);

                    var runRequest = new RunRequest();
                    runRequest.Parameters = parameters;
                    runRequest.Client = Client;

                    var runFunction = scope.GetVariable<Func<RunRequest, TrakHoundFunctionResponse>>(_functionName);
                    if (runFunction != null)
                    {
                        return runFunction(runRequest);
                    }
                }
                else
                {
                    Log(TrakHoundLogLevel.Error, $"No Script Found : Path = {scriptPath}");
                }
            }

            return Ok("TrakHound.Python.Function Function Completed");
        }
    }
}
