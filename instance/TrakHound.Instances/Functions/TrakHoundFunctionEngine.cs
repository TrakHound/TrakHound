// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Logging;

namespace TrakHound.Functions
{
    public class TrakHoundFunctionEngine : IDisposable
    {
        private readonly string _runId;
        private readonly string _engineId;
        private readonly string _packageId;
        private readonly string _packageVersion;
        private readonly ITrakHoundFunction _function;
        private readonly ITrakHoundClient _client;
        private readonly IReadOnlyDictionary<string, string> _parameters;
        private CancellationTokenSource _stop;


        public string RunId => _runId;

        public string EngineId => _engineId;

        public string PackageId => _packageId;

        public string PackageVersion => _packageVersion;


        public ITrakHoundFunction Function => _function;

        public ITrakHoundClient Client => _client;


        internal event FunctionStatusDelegete StatusReceived;

        internal event FunctionLogDelegete LogReceived;


        public TrakHoundFunctionEngine(ITrakHoundFunction function, IReadOnlyDictionary<string, string> parameters, string packageId, string packageVersion, string runId)
        {
            _function = function;

            if (function != null)
            {
                _runId = !string.IsNullOrEmpty(runId) ? runId : Guid.NewGuid().ToString();
                _engineId = function.Id;
                _packageId = packageId;
                _packageVersion = packageVersion;
                _client = function.Client;
                _parameters = parameters;

                function.LogReceived += FunctionLogReceived;
            }
        }

        public void Dispose()
        {
            if (_function != null) _function.Dispose();
        }


        public async Task<TrakHoundFunctionResponse> Run(long timestamp = 0)
        {
            var response = new TrakHoundFunctionResponse();

            long started = 0;
            long completed = 0;

            if (_client != null && _function != null)
            {
                try
                {
                    if (timestamp > 0) started = timestamp;
                    else started = UnixDateTime.Now;

                    if (StatusReceived != null) StatusReceived.Invoke(_function, _runId, new TrakHoundStatusItem(TrakHoundFunctionStatusType.Started, started));
                    if (LogReceived != null) LogReceived.Invoke(_function, _runId, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Information, "Function Started", timestamp: started));

                    // Run the Function
                    response = await _function.Run(_parameters);

                    completed = UnixDateTime.Now;

                    if (StatusReceived != null) StatusReceived.Invoke(_function, _runId, new TrakHoundStatusItem(TrakHoundFunctionStatusType.Completed, completed));
                    if (LogReceived != null) LogReceived.Invoke(_function, _runId, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Information, "Function Completed", timestamp: completed));
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    completed = UnixDateTime.Now;

                    if (StatusReceived != null) StatusReceived.Invoke(_function, _runId, new TrakHoundStatusItem(TrakHoundFunctionStatusType.Error, completed));
                    if (LogReceived != null) LogReceived.Invoke(_function, _runId, new TrakHoundLogItem("Engine", TrakHoundLogLevel.Critical, ex.Message, ex.HResult.ToString(), completed));
                }
            }

            response.Id = _runId;
            response.PackageId = _packageId;
            response.PackageVersion = _packageVersion;
            response.EngineId = _engineId;
            response.Started = started;
            response.Completed = completed;

            return response;
        }

        private void FunctionStatusReceived(object sender, TrakHoundStatusItem item)
        {
            if (StatusReceived != null) StatusReceived.Invoke(_function, _runId, item);
        }

        private void FunctionLogReceived(object sender, TrakHoundLogItem item)
        {
            if (LogReceived != null) LogReceived.Invoke(_function, _runId, item);
        }
    }
}
