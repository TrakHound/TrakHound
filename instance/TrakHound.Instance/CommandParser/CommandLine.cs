// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Diagnostics;

namespace TrakHound.Instance
{
    internal static class CommandLine
    {
        private const bool _verbose = false;


        public static bool Run(string cmd, string workingDirectory)
        {
            try
            {
                var startInfo = new ProcessStartInfo("cmd", "/c " + cmd);
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.WorkingDirectory = workingDirectory;

                using (var process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();

                    using (var stream = process.StandardOutput)
                    {
                        while (!stream.EndOfStream)
                        {
                            var line = stream.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (_verbose) Console.WriteLine(line);
                            }
                        }
                    }

                    using (var stream = process.StandardError)
                    {
                        while (!stream.EndOfStream)
                        {
                            var line = stream.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (_verbose) Console.WriteLine(line);
                            }
                        }
                    }

                    //using (var spinner = new Spinner(Console.CursorLeft, Console.CursorTop))
                    //{
                    //    spinner.Start();
                        process.WaitForExit();
                    //}

                    var exitCode = process.ExitCode;

                    process.Close();

                    return exitCode >= 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public class Spinner : IDisposable
        {
            private int barLength = 20;
            private int counter = 0;
            private readonly int left;
            private readonly int top;
            private readonly int delay;
            private bool active;
            private readonly Thread thread;

            public Spinner(int left, int top, int delay = 250)
            {
                this.left = left;
                this.top = top;
                this.delay = delay;
                thread = new Thread(Spin);
            }

            public void Start()
            {
                active = true;
                if (!thread.IsAlive)
                    thread.Start();
            }

            public void Stop()
            {
                active = false;
                Console.SetCursorPosition(left, top);
                Console.ResetColor();
                for (var i = 0; i < barLength; i++) Console.Write(' ');
            }

            private void Spin()
            {
                while (active)
                {
                    Turn();
                    Thread.Sleep(delay);
                }
            }

            private void Draw()
            {
                Console.SetCursorPosition(left, top);
                Console.BackgroundColor = ConsoleColor.White;
                for (var i = 0; i < counter; i++) Console.Write(' ');
                Console.ResetColor();
                for (var i = counter; i < barLength; i++) Console.Write(' ');
            }

            private void Turn()
            {
                counter++;
                if (counter > barLength) counter = 0;
                Draw();
            }

            public void Dispose()
            {
                Stop();
            }
        }

        //public class ConsoleSpiner
        //{
        //    int counter;
        //    public ConsoleSpiner()
        //    {
        //        counter = 0;
        //    }
        //    public void Turn()
        //    {
        //        counter++;
        //        switch (counter % 4)
        //        {
        //            case 0: Console.Write("/"); break;
        //            case 1: Console.Write("-"); break;
        //            case 2: Console.Write("\\"); break;
        //            case 3: Console.Write("|"); break;
        //        }
        //        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        //    }
        //}
    }
}
