// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Cron;
using TrakHound.Services;

namespace TrakHound.ScheduledTasks
{
    internal delegate void LogEventHandler(Engine engine, TrakHoundLogLevel loglevel, string message, string code = null);

    internal class Engine
    {
        private readonly ITrakHoundService _service;
        private readonly ScheduledTaskConfiguration _configuration;
        private CancellationTokenSource _stop;


        public event LogEventHandler LogReceived;


        public Engine(ITrakHoundService service, ScheduledTaskConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        public void Start()
        {
            Log(TrakHoundLogLevel.Information, $"ScheduledTask Engine ({_configuration.Id}) Started");

            _stop = new CancellationTokenSource();

            _ = Task.Run(Worker);
        }

        public void Stop()
        {
            Log(TrakHoundLogLevel.Information, $"ScheduledTask Engine ({_configuration.Id}) Stopped");

            if (_stop != null) _stop.Cancel();
        }

        private async Task Worker()
        {
            if (_configuration != null && !string.IsNullOrEmpty(_configuration.FunctionId) && !string.IsNullOrEmpty(_configuration.Schedule))
            {
                try
                {
                    var format = CronFormat.Standard;
                    if (_configuration.Schedule.Split(' ').Length > 5) format = CronFormat.IncludeSeconds;

                    var expression = CronExpression.Parse(_configuration.Schedule, format);
                    var timezone = TimeZoneInfo.Local;
                    var now = DateTime.UtcNow;
                    var last = DateTime.UtcNow;
                    var next = expression.GetNextOccurrence(last, timezone).Value;

                    Log(TrakHoundLogLevel.Information, $"({_configuration.Id}) {_configuration.FunctionId} Scheduled Task Engine Started : Schedule = {_configuration.Schedule}");
                    Log(TrakHoundLogLevel.Information, $"({_configuration.Id}) {_configuration.FunctionId} Scheduled Task Engine : Scheduled Next Run = {next.ToISO8601String()}");

                    while (!_stop.IsCancellationRequested)
                    {
                        now = DateTime.UtcNow;
                        if (now >= next)
                        {
                            var ago = now - last;
                            Log(TrakHoundLogLevel.Information, $"Scheduled Task : {next.ToISO8601String()} - {ago.ToDetailedFormattedString()} : Run Function");

                            var functionResponse = await _service.Client.Functions.Run(_configuration.FunctionId, _configuration.Parameters, timestamp: now);

                            Log(TrakHoundLogLevel.Debug, $"Function Response : ID : {functionResponse.Id}");
                            Log(TrakHoundLogLevel.Debug, $"Function Response : StatusCode : {functionResponse.StatusCode}");

                            last = now;
                            next = expression.GetNextOccurrence(last).Value;
                        }

                        await Task.Delay(500, _stop.Token);
                    }
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    Log(TrakHoundLogLevel.Error, ex.Message);
                }
            }
        }

        public void Log(TrakHoundLogLevel level, string message, string code = null)
        {
            if (LogReceived != null) LogReceived.Invoke(this, level, message, code);
        }
    }
}
