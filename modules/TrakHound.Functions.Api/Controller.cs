// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Entities;
using TrakHound.Requests;

namespace TrakHound.Functions
{
    public class Controller : TrakHoundApiController
    {
        private const string _defaultScheduledTasksRoute = "scheduled-tasks/api";


        [TrakHoundApiQuery("list")]
        public async Task<TrakHoundApiResponse> GetFunctions()
        {
            var informations = await Client.System.Functions.GetInformation();
            if (!informations.IsNullOrEmpty())
            {
                return Ok(informations);
            }
            else
            {
                return NotFound();
            }
        }

        [TrakHoundApiQuery("{functionId}")]
        public async Task<TrakHoundApiResponse> GetFunction([FromRoute] string functionId)
        {
            if (!string.IsNullOrEmpty(functionId))
            {
                var information = await Client.System.Functions.GetInformation(functionId);
                if (information != null)
                {
                    return Ok(information);
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

        [TrakHoundApiQuery("{functionId}/tasks")]
		public async Task<TrakHoundApiResponse> GetFunctionTasks([FromRoute] string functionId)
		{
			if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId))
			{
                var taskConfigurationIds = (await Client.Entities.GetSets($"/.Instances/{InstanceId}/Functions/{functionId}/ScheduledTasks"))?.Select(o => o.Value);
                if (!taskConfigurationIds.IsNullOrEmpty())
                {
                    var taskConfigurations = new List<FunctionTask>();

                    foreach (var taskConfigurationId in taskConfigurationIds)
                    {
                        var taskConfiguration = await Client.Api.QueryJson<FunctionTask>(Url.Combine(_defaultScheduledTasksRoute, taskConfigurationId));
                        if (taskConfiguration != null)
                        {
                            taskConfigurations.Add(taskConfiguration);
                        }
                    }

                    return Ok(taskConfigurations);
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

		[TrakHoundApiQuery("{functionId}/runs")]
        public async Task<TrakHoundApiResponse> GetFunctionRuns(
            [FromRoute] string functionId,
            [FromQuery] string start,
            [FromQuery] string stop,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 100,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending
            )
        {
            if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId))
            {
                var events = await Client.Entities.GetEvents($"/.Instances/{InstanceId}/Functions/{functionId}/Runs", start, stop, skip, take, (SortOrder)sortOrder);
                if (!events.IsNullOrEmpty())
                {
                    var runPaths = events.Select(o => o.Target);
                    if (!runPaths.IsNullOrEmpty())
                    {
                        var runModels = await Client.Entities.Get<FunctionRunModel>(runPaths);
                        if (!runModels.IsNullOrEmpty())
                        {
                            return Ok(runModels);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return NotFound();
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

        [TrakHoundApiQuery("{functionId}/runs/{runId}")]
        public async Task<TrakHoundApiResponse> GetFunctionRuns([FromRoute] string functionId, [FromRoute] string runId)
        {
            if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId) && !string.IsNullOrEmpty(runId))
            {
                var path = TrakHoundPath.Combine($"/.Instances/{InstanceId}/Functions", functionId, "Run", runId);

                var runModel = await Client.Entities.GetSingle<FunctionRunModel>(path);
                if (runModel != null)
                {
                    return Ok(runModel);
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

        [TrakHoundApiQuery("{functionId}/runs/{runId}/output")]
        public async Task<TrakHoundApiResponse> GetFunctionRunOutput([FromRoute] string functionId, [FromRoute] string runId)
        {
            if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId) && !string.IsNullOrEmpty(runId))
            {
                var path = TrakHoundPath.Combine($"/.Instances/{InstanceId}/Functions", functionId, "Run", runId, "Output");

                var entries = await Client.Entities.GetHashValues(path);
                if (!entries.IsNullOrEmpty())
                {
                    var parameters = entries.FirstOrDefault();
                    if (!parameters.Value.IsNumeric())
                    {
                        return Ok(parameters.Value);
                    }
                    else
                    {
                        return NotFound();
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

        [TrakHoundApiQuery("{functionId}/runs/{runId}/status")]
        public async Task<TrakHoundApiResponse> GetFunctionRunStatus([FromRoute] string functionId, [FromRoute] string runId)
        {
            if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId) && !string.IsNullOrEmpty(runId))
            {
                var path = TrakHoundPath.Combine($"/.Instances/{InstanceId}/Functions", functionId, "Run", runId, "Status");

                var statuses = await Client.Entities.GetStates(path, DateTime.MinValue, DateTime.MaxValue, 0, int.MaxValue);
                if (!statuses.IsNullOrEmpty())
                {
                    var functionStatuses = new List<FunctionStatus>();
                    foreach (var status in statuses)
                    {
                        var functionStatus = new FunctionStatus();
                        functionStatus.Status = status.Type.ConvertEnum<TrakHoundFunctionStatusType>();
                        functionStatus.Timestamp = status.Timestamp;
                        functionStatuses.Add(functionStatus);
                    }

                    return Ok(functionStatuses);
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

        [TrakHoundApiSubscribe("{functionId}/runs/{runId}/status")]
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeFunctionRunStatus([FromRoute] string functionId, [FromRoute] string runId)
        {
            if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId) && !string.IsNullOrEmpty(runId))
            {
                var path = TrakHoundPath.Combine($"/.Instances/{InstanceId}/Functions", functionId, "Run", runId, "Status");

                await Client.Entities.PublishObject(path, TrakHoundObjectContentType.State); // Create Object first. Probably needs to be fixed in the entity subscribe?

                var statusConsumer = await Client.Entities.SubscribeStates(path);
                if (statusConsumer != null)
                {
                    var consumerId = Guid.NewGuid().ToString();
                    var responseConsumer = new TrakHoundConsumer<IEnumerable<TrakHoundState>, TrakHoundApiResponse>(statusConsumer);
                    responseConsumer.OnReceived = (statuses) =>
                    {
                        var functionStatuses = new List<FunctionStatus>();
                        foreach (var status in statuses)
                        {
                            var functionStatus = new FunctionStatus();
                            functionStatus.Status = status.Type.ConvertEnum<TrakHoundFunctionStatusType>();
                            functionStatus.Timestamp = status.Timestamp;
                            functionStatuses.Add(functionStatus);
                        }

                        return Ok(functionStatuses);
                    };

                    return responseConsumer;
                }
            }

            return null;
        }

        [TrakHoundApiQuery("{functionId}/runs/{runId}/log")]
		public async Task<TrakHoundApiResponse> GetFunctionRunLog(
            [FromRoute] string functionId,
            [FromRoute] string runId,
            [FromQuery] string minimumLevel = "Trace"
            )
		{
			if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId) && !string.IsNullOrEmpty(runId))
			{
				var path = TrakHoundPath.Combine($"/.Instances/{InstanceId}/Functions", functionId, "Run", runId, "Log");
                var level = minimumLevel.ConvertEnum<TrakHoundLogLevel>();

                var logs = await Client.Entities.GetLogs(path, level, 0, int.MaxValue);
				if (!logs.IsNullOrEmpty())
				{
                    var functionLogs = new List<FunctionLog>();
                    foreach (var log in logs)
                    {
                        var functionLog = new FunctionLog();
                        functionLog.Level = log.Level;
                        functionLog.Message = log.Message;
                        functionLog.Timestamp = log.Timestamp;
                        functionLogs.Add(functionLog);
                    }

					return Ok(functionLogs);
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

		[TrakHoundApiSubscribe("{functionId}/runs/{runId}/log")]
		public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeFunctionRunLog(
            [FromRoute] string functionId,
			[FromRoute] string runId,
			[FromQuery] string minimumLevel = "Trace"
			)
		{
			if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId) && !string.IsNullOrEmpty(runId))
			{
				var path = TrakHoundPath.Combine($"/.Instances/{InstanceId}/Functions", functionId, "Run", runId, "Log");
				var level = minimumLevel.ConvertEnum<TrakHoundLogLevel>();

                await Client.Entities.PublishObject(path, TrakHoundObjectContentType.Log); // Create Object first. Probably needs to be fixed in the entity subscribe?

				var logConsumer = await Client.Entities.SubscribeLogs(path, level);
				if (logConsumer != null)
				{
                    var responseConsumer = new TrakHoundConsumer<IEnumerable<TrakHoundLog>, TrakHoundApiResponse>(logConsumer);
                    responseConsumer.OnReceived = (logs) =>
                    {
                        var functionLogs = new List<FunctionLog>();
                        foreach (var log in logs)
                        {
                            var functionLog = new FunctionLog();
                            functionLog.Level = log.Level;
                            functionLog.Message = log.Message;
                            functionLog.Timestamp = log.Timestamp;
                            functionLogs.Add(functionLog);
                        }
                        return Ok(functionLogs);
                    };

					return responseConsumer;
				}
			}

            return null;
		}


		[TrakHoundApiQuery("{functionId}/run")]
        public async Task<TrakHoundApiResponse> RunFunction(
            [FromRoute] string functionId,
            [FromBody(ContentType = "application/json")] Dictionary<string, string> inputParameters = null,
            [FromQuery] string runId = null,
            [FromQuery] string timestamp = null
            )
        {
            var apiResponse = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(functionId))
            {
                var started = timestamp != null ? timestamp.ToDateTime().ToUnixTime() : UnixDateTime.Now;

                var runResponse = await Client.System.Functions.Run(functionId, inputParameters, runId, started);
                var publishTransaction = new TrakHoundEntityTransaction();

                var functionPath = TrakHoundPath.Combine($"/.Instances/{InstanceId}/Functions", runResponse.PackageId);
                var enginePath = TrakHoundPath.Combine($"/.Instances/{InstanceId}/Functions", runResponse.PackageId, "Engines", runResponse.EngineId);

                var runPath = TrakHoundPath.Combine(functionPath, $"Run/{runResponse.Id}");
                publishTransaction.Add(new TrakHoundObjectEntry(runPath));
                publishTransaction.Add(new TrakHoundStringEntry(TrakHoundPath.Combine(runPath, "PackageVersion"), runResponse.PackageVersion));
                publishTransaction.Add(new TrakHoundNumberEntry(TrakHoundPath.Combine(runPath, "StatusCode"), runResponse.StatusCode));
                publishTransaction.Add(new TrakHoundReferenceEntry(TrakHoundPath.Combine(runPath, "Engine"), enginePath));
                publishTransaction.Add(new TrakHoundTimestampEntry(TrakHoundPath.Combine(runPath, "Start"), runResponse.Started));
                publishTransaction.Add(new TrakHoundTimestampEntry(TrakHoundPath.Combine(runPath, "End"), runResponse.Completed));
                publishTransaction.Add(new TrakHoundDurationEntry(TrakHoundPath.Combine(runPath, "Duration"), TimeSpan.FromTicks(runResponse.Completed - runResponse.Started)));
                
                publishTransaction.Add(new TrakHoundEventEntry(TrakHoundPath.Combine(enginePath, "Runs"), runPath));
                publishTransaction.Add(new TrakHoundEventEntry(TrakHoundPath.Combine(functionPath, "Runs"), runPath));

                // Input Parameters
                if (!inputParameters.IsNullOrEmpty())
                {
                    foreach (var parameter in inputParameters)
                    {
                        publishTransaction.Add(new TrakHoundHashEntry(TrakHoundPath.Combine(runPath, "Input"), parameter.Key, parameter.Value));
                    }
                }

                // Output Parameters
                if (!runResponse.Parameters.IsNullOrEmpty())
                {
                    foreach (var parameter in runResponse.Parameters)
                    {
                        publishTransaction.Add(new TrakHoundHashEntry(TrakHoundPath.Combine(runPath, "Output"), parameter.Key, parameter.Value));
                    }
                }

                // Set Source
                publishTransaction.Source = Source;

                await Client.Entities.Publish(publishTransaction, true);

                var jsonResponse = new TrakHoundFunctionJsonResponse(runResponse);
                
                apiResponse.StatusCode = runResponse.StatusCode;
                apiResponse.Success = true;
                apiResponse.ContentType = "application/json";
                apiResponse.Content = TrakHoundApiResponse.GetContentStream(System.Text.Encoding.UTF8.GetBytes(jsonResponse.ToJson(true)));
            }
            else
            {
                apiResponse.StatusCode = 400;
            }

            return apiResponse;
        }


		[TrakHoundApiPublish("{functionId}/tasks")]
		public async Task<TrakHoundApiResponse> AddFunctionTask(
            [FromRoute] string functionId,
            [FromQuery] string description,
            [FromQuery] string schedule,
            [FromBody] Dictionary<string, string> parameters = null,
            [FromQuery] bool enabled = true
            )
		{
			if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId))
			{
                var taskConfiguration = new FunctionTask();
                taskConfiguration.Id = Guid.NewGuid().ToString();
                taskConfiguration.Description = description;
                taskConfiguration.Enabled = enabled;
                taskConfiguration.FunctionId = functionId;
                taskConfiguration.Schedule = schedule;
                taskConfiguration.Parameters = parameters;

                var response = await Client.Api.Publish(_defaultScheduledTasksRoute, taskConfiguration);
                if (response.Success)
				{
                    var tasksPath = $"/.Instances/{InstanceId}/Functions/{functionId}/ScheduledTasks";
                    if (await Client.Entities.PublishSet(tasksPath, taskConfiguration.Id))
                    {
                        return Ok(taskConfiguration);
                    }
                    else
                    {
                        return InternalError();
                    }
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

		[TrakHoundApiPublish("{functionId}/tasks/{taskConfigurationId}/enable")]
		public async Task<TrakHoundApiResponse> EnableFunctionTask([FromRoute] string functionId, [FromRoute] string taskConfigurationId)
		{
			if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId) && !string.IsNullOrEmpty(taskConfigurationId))
			{
                var taskConfiguration = await Client.Api.QueryJson<FunctionTask>(Url.Combine(_defaultScheduledTasksRoute, taskConfigurationId));
                if (taskConfiguration != null)
                {
                    taskConfiguration.Enabled = true;

                    var response = await Client.Api.Publish(_defaultScheduledTasksRoute, taskConfiguration);
                    if (response.Success)
                    {
                        return Ok(taskConfiguration);
                    }
                    else
                    {
                        return InternalError();
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

		[TrakHoundApiPublish("{functionId}/tasks/{taskConfigurationId}/disable")]
		public async Task<TrakHoundApiResponse> DisableFunctionTask([FromRoute] string functionId, [FromRoute] string taskConfigurationId)
		{
            if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId) && !string.IsNullOrEmpty(taskConfigurationId))
            {
                var taskConfiguration = await Client.Api.QueryJson<FunctionTask>(Url.Combine(_defaultScheduledTasksRoute, taskConfigurationId));
                if (taskConfiguration != null)
                {
                    taskConfiguration.Enabled = false;

                    var response = await Client.Api.Publish(_defaultScheduledTasksRoute, taskConfiguration);
                    if (response.Success)
                    {
                        return Ok(taskConfiguration);
                    }
                    else
                    {
                        return InternalError();
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

		[TrakHoundApiDelete("{functionId}/tasks/{taskConfigurationId}")]
		public async Task<TrakHoundApiResponse> DeleteFunctionTask([FromRoute] string functionId, [FromRoute] string taskConfigurationId)
		{
			if (!string.IsNullOrEmpty(InstanceId) && !string.IsNullOrEmpty(functionId) && !string.IsNullOrEmpty(taskConfigurationId))
			{
                var response = await Client.Api.Delete(Url.Combine(_defaultScheduledTasksRoute, taskConfigurationId));
                if (response.Success)
				{
                    var tasksPath = $"/.Instances/{InstanceId}/Functions/{functionId}/ScheduledTasks";
                    if (await Client.Entities.DeleteSet(tasksPath, taskConfigurationId))
                    {
                        return Ok();
                    }
                    else
                    {
                        return InternalError();
                    }
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
