// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Logging
{
    public class TrakHoundLogProvider
    {
        private static readonly object _lock = new object();
        private static readonly TrakHoundLogProvider _instance = new TrakHoundLogProvider();
        public static readonly Dictionary<string, ITrakHoundLogger> _loggers = new Dictionary<string, ITrakHoundLogger>();
        public static TrakHoundLogLevel MinimumLogLevel = TrakHoundLogLevel.Information;


        /// <summary>
        /// Event Handler for when a new Log Entry is received
        /// </summary>
        public EventHandler<TrakHoundLogItem> LogEntryReceived { get; set; }


        public static TrakHoundLogProvider Get()
        {
            return _instance;
        }

        public static ITrakHoundLogger GetLogger(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                lock (_lock)
                {
                    return _loggers.GetValueOrDefault(name);
                }
            }

            return null;
        }


        public static void AddLogger(ITrakHoundLogger logger)
        {
            if (!string.IsNullOrEmpty(logger.Name))
            {
                logger.LogEntryReceived += _instance.EntryReceived;
                lock (_lock)
                {
                    _loggers.Remove(logger.Name);
                    _loggers.Add(logger.Name, logger);
                }
            }
        }

        private void EntryReceived(object sender, TrakHoundLogItem entry)
        {
            _instance.LogEntryReceived?.Invoke(sender, entry);
        }

        public static void CreateConsoleLogger(TrakHoundLogLevel logLevel = TrakHoundLogLevel.Information)
        {
            var logger = Get();
            logger.LogEntryReceived += (sender, e) =>
            {
                //    if (e.LogLevel <= logLevel)
                //    {
                //        Console.WriteLine(e.Message);

                //        //if (e.LogLevel == TrakHoundLogLevel.Error)
                //        //{
                //        //    Console.Write($"-    Error    -");
                //        //}
                //        //else if (e.LogLevel == TrakHoundLogLevel.Warning)
                //        //{
                //        //    Console.Write($"-   Warning   -");
                //        //}
                //        //else
                //        //{
                //        //    Console.Write("- ");
                //        //    Console.Write($"{e.LogLevel}");
                //        //    Console.Write(" -");
                //        //}

                //        //Console.Write(" : ");
                //        //Console.Write($"[ {e.Sender} ]");
                //        //Console.Write(" : ");
                //        //Console.Write(e.Message);
                //        //Console.WriteLine();
                //    }


                //Console.WriteLine(_loggers.ToList().Count);



                if (e.LogLevel <= logLevel)
                {
                    //Console.WriteLine(e.Message);

                    lock (_lock)
                    {

                        if (e.LogLevel == TrakHoundLogLevel.Error)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"-    Error    -");
                        }
                        else if (e.LogLevel == TrakHoundLogLevel.Warning)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write($"-   Warning   -");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write("- ");
                            Console.ResetColor();
                            Console.Write($"{(TrakHoundLogLevel)e.LogLevel}");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(" -");
                        }

                        Console.ResetColor();
                        Console.Write(" : ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[ {e.Sender} ]");
                        Console.ResetColor();
                        Console.Write(" : ");
                        Console.Write(e.Message);
                        Console.WriteLine();
                    }
                }
            };
        }
    }
}
