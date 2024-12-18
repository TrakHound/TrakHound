// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using NLog;
using System.Reflection;
using System.Text.RegularExpressions;
using TrakHound.Logging;

namespace TrakHound.Instance.Logging
{
    public class TrakHoundNLogProvider : TrakHoundLogProviderBase
    {
        private readonly Logger _logger;
        private readonly string _instanceId;


        public TrakHoundNLogProvider(string instanceId, Logger logger)
        {
            _logger = logger;
            _instanceId = instanceId;
        }


        public override string GetLoggerId()
        {
            return $"trakhound.instance.{_instanceId}";
        }

        public override string GetLoggerId(string name)
        {
            return $"{GetLoggerId()}/{name}";
        }

        private string GetLoggerName(string loggerId)
        {
            if (!string.IsNullOrEmpty(loggerId))
            {
                var prefix = $"trakhound.instance.{_instanceId}";
                if (loggerId.StartsWith(prefix))
                {
                    return loggerId.Substring(prefix.Length + 1);
                }
            }

            return null;
        }

        protected override void HandleEntryReceived(ITrakHoundLogger logger, TrakHoundLogItem entry)
        {
            var logEvent = new LogEventInfo();
            logEvent.LoggerName = logger.Id;
            logEvent.Message = entry.Message;

            switch (entry.LogLevel)
            {
                case TrakHoundLogLevel.Critical: logEvent.Level = NLog.LogLevel.Fatal; break;
                case TrakHoundLogLevel.Error: logEvent.Level = NLog.LogLevel.Error; break;
                case TrakHoundLogLevel.Warning: logEvent.Level = NLog.LogLevel.Warn; break;
                case TrakHoundLogLevel.Information: logEvent.Level = NLog.LogLevel.Info; break;
                case TrakHoundLogLevel.Debug: logEvent.Level = NLog.LogLevel.Debug; break;
                case TrakHoundLogLevel.Trace: logEvent.Level = NLog.LogLevel.Trace; break;
            }

            _logger.Log(logEvent);
        }

        public override IEnumerable<string> QueryLoggerIds(string pattern)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                var loggerIds = GetLoggerIds();
                if (!loggerIds.IsNullOrEmpty())
                {
                    return loggerIds.Where(o => MatchLogger(pattern, o));
                }
            }

            return null;            
        }

        public override IEnumerable<string> QueryLoggerIds(IEnumerable<string> patterns)
        {
            if (!patterns.IsNullOrEmpty())
            {
                var loggerIds = GetLoggerIds();
                if (!loggerIds.IsNullOrEmpty())
                {
                    return loggerIds.Where(o => MatchLogger(patterns, o));
                }
            }

            return null;
        }

        //public override IEnumerable<string> QueryLoggerIds(string pattern)
        //{
        //    var loggerIds = new List<string>();

        //    var loggers = GetLoggers();
        //    if (!loggers.IsNullOrEmpty()) loggerIds.AddRange(loggers.Select(o => o.Id));

        //    var loggerBasePath = GetLogsDirectory();
        //    var loggerPaths = GetLoggerPaths(loggerBasePath);
        //    if (!loggerPaths.IsNullOrEmpty())
        //    {
        //        var fileLoggerIds = loggerPaths.Select(o => Path.GetRelativePath(loggerBasePath, o).Replace('\\', '/'));
        //        if (!fileLoggerIds.IsNullOrEmpty()) loggerIds.AddRange(fileLoggerIds);
        //    }

        //    return loggerIds.Where(o => MatchLogger(pattern, o));
        //}


        public override IEnumerable<TrakHoundLogItem> QueryLogs(string name, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var logItems = new List<TrakHoundLogItem>();

                var logLevels = Enum.GetValues<TrakHoundLogLevel>();
                foreach (var logLevel in logLevels)
                {
                    var levelItems = QueryLogsByLevel(name, logLevel, from, to, skip, take, sortOrder);
                    if (!levelItems.IsNullOrEmpty())
                    {
                        logItems.AddRange(levelItems);
                    }
                }

                return logItems;
            }

            return null;
        }

        public override IEnumerable<TrakHoundLogItem> QueryLogsByMinimumLevel(string name, TrakHoundLogLevel minimumLogLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var logItems = new List<TrakHoundLogItem>();

                for (var i = 0; i < (int)minimumLogLevel; i++)
                {
                    var logLevel = (TrakHoundLogLevel)i;
                    var levelItems = QueryLogsByLevel(name, logLevel, from, to, skip, take, sortOrder);
                    if (!levelItems.IsNullOrEmpty())
                    {
                        logItems.AddRange(levelItems);
                    }
                }

                return logItems;
            }

            return null;
        }

        public override IEnumerable<TrakHoundLogItem> QueryLogsByLevel(string name, TrakHoundLogLevel logLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var loggerId = GetLoggerId(name);
            if (!string.IsNullOrEmpty(loggerId))
            {
                try
                {
                    var directoryPath = Path.Combine(GetLogsDirectory(), name);
                    if (Directory.Exists(directoryPath))
                    {
                        var nlogLevel = GetLogLevel(logLevel);
                        if (nlogLevel != null)
                        {
                            var files = Directory.GetFiles(directoryPath, $"*.{nlogLevel.ToLower()}.log");
                            if (!files.IsNullOrEmpty())
                            {
                                var matchedFiles = files.Where(o => MatchLogFile(o, from, to)).Skip(skip).Take(take);
                                if (!matchedFiles.IsNullOrEmpty())
                                {
                                    var logItems = new List<TrakHoundLogItem>();

                                    foreach (var matchedFile in matchedFiles)
                                    {
                                        var logLines = File.ReadLines(matchedFile);
                                        if (!logLines.IsNullOrEmpty())
                                        {
                                            foreach (var logLine in logLines)
                                            {
                                                var nextDelimiter = logLine.IndexOf('|');
                                                var timestamp = logLine.Substring(0, nextDelimiter).ToDateTime().ToUnixTime();
                                                var message = logLine.Substring(nextDelimiter + 1).Trim();

                                                logItems.Add(new TrakHoundLogItem(loggerId, logLevel, message, timestamp: timestamp));
                                            }
                                        }
                                    }

                                    return logItems;
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }


        private bool MatchLogger(string pattern, string loggerId)
        {
            if (!string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(loggerId))
            {
                var loggerName = GetLoggerName(loggerId);
                if (loggerName != null)
                {
                    try
                    {
                        var mPattern = Regex.Escape(pattern.ToLower());
                        mPattern = mPattern.Replace("\\*", ".*");
                        mPattern = '^' + mPattern + '$';

                        var mLoggerName = loggerName.ToLower();

                        return Regex.IsMatch(mLoggerName, mPattern);
                    }
                    catch { }
                }
            }

            return false;
        }

        private bool MatchLogger(IEnumerable<string> patterns, string loggerId)
        {
            if (!patterns.IsNullOrEmpty() && !string.IsNullOrEmpty(loggerId))
            {
                foreach (var pattern in patterns)
                {
                    if (MatchLogger(pattern, loggerId)) return true;
                }
            }

            return false;
        }

        private static bool MatchLogFile(string filePath, DateTime from, DateTime to)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var filename = Path.GetFileNameWithoutExtension(filePath); // Remove .log
                filename = Path.GetFileNameWithoutExtension(filename); // Remove .{logLevel}

                var fileTimestamp = filename.ToDateTime();
                return fileTimestamp >= from && fileTimestamp < to;
            }

            return false;
        }

        private static string GetLogsDirectory()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "logs");
        }

        private static IEnumerable<string> GetLoggerPaths(string basePath)
        {
            var loggerPaths = new List<string>();

            if (!string.IsNullOrEmpty(basePath))
            {
                if (Directory.Exists(basePath))
                {
                    var subDirectories = Directory.GetDirectories(basePath);
                    if (!subDirectories.IsNullOrEmpty())
                    {
                        foreach (var subDirectory in subDirectories)
                        {
                            loggerPaths.Add(subDirectory);
                            loggerPaths.AddRange(GetLoggerPaths(subDirectory));
                        }
                    }
                }
            }

            return loggerPaths;
        }

        private static string GetLogLevel(TrakHoundLogLevel logLevel)
        {
            switch (logLevel)
            {
                case TrakHoundLogLevel.Critical: return NLog.LogLevel.Fatal.ToString();
                case TrakHoundLogLevel.Error: return NLog.LogLevel.Error.ToString();
                case TrakHoundLogLevel.Warning: return NLog.LogLevel.Warn.ToString();
                case TrakHoundLogLevel.Information: return NLog.LogLevel.Info.ToString();
                case TrakHoundLogLevel.Debug: return NLog.LogLevel.Debug.ToString();
                case TrakHoundLogLevel.Trace: return NLog.LogLevel.Trace.ToString();
            }

            return null;
        }
    }
}
