// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Diagnostics;

namespace TrakHound
{
	public static class CommandProcess
	{
		public static bool Run(string command, string workingDirectory = null)
		{
			if (!string.IsNullOrEmpty(command))
			{
				try
				{
					// Set Process Start Info
					var startInfo = new ProcessStartInfo("cmd", "/c " + command);
					//startInfo.RedirectStandardOutput = true;
					//startInfo.RedirectStandardError = true;
					startInfo.UseShellExecute = false;
					startInfo.WorkingDirectory = workingDirectory;

					// Start the Process\
					var process = new Process();
					process.StartInfo = startInfo;
					process.Start();

					// Block until Process is completed
					process.WaitForExit();

					return true;
				}
				catch (Exception ex)
				{

				}
			}

			return false;
		}
	}
}
