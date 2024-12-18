﻿// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Services;
using TrakHound.Volumes;

namespace TrakHound.ScheduledTasks
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            // Create new TrakHoundClient based on the Instance BaseUrl and Router
            var clientConfiguration = new TrakHoundHttpClientConfiguration("localhost", 8472);
            var client = new TrakHoundHttpClient(clientConfiguration, null);

            var volumePath = Path.Combine(AppContext.BaseDirectory, "volume");
            var volume = new TrakHoundVolume("volume", volumePath);

            var instanceInformation = await client.System.Instances.GetHostInformation();

            var serviceConfiguration = new TrakHoundServiceConfiguration();

            // Create a new instance of the Function
            var service = new Service(serviceConfiguration, client, volume);
            service.InstanceId = instanceInformation?.Id;
            service.LogReceived += ServiceLogReceived;

            await service.Start();

            Console.WriteLine("Press Key to Stop Service..");
            Console.ReadLine();

            await service.Stop();

            Console.WriteLine("Stopping Service..");
            await Task.Delay(1000);
        }

        private static void ServiceLogReceived(object sender, TrakHound.Logging.TrakHoundLogItem item)
        {
            System.Console.WriteLine($"Log : {item.Timestamp.ToLocalDateTime()} : {item.LogLevel} : {item.Code} : {item.Message}");
        }
    }
}