// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.CLI
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            TrakHoundTemp.Clear();

            TrakHoundCliConfiguration.Load();

#if DEBUG
            await Debug();
#else
            await Run(args);
#endif
        }

        private static async Task Run(string[] args)
        {
            var cmdArgs = new List<string>();
            if (args != null && args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg.Contains(' ')) cmdArgs.Add($"\"{arg}\"");
                    else cmdArgs.Add(arg);
                }
            }

            var cmd = string.Join(' ', cmdArgs);
            await CommandParser.Run(cmd);
        }

        private static async Task Debug()
        {
            while (true)
            {
                var cmd = Console.ReadLine();
                if (!string.IsNullOrEmpty(cmd))
                {
                    await CommandParser.Run(cmd);
                }
            }
        }
    }
}