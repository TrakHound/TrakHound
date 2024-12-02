using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Functions;
using TrakHound.Volumes;

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

        // Set Function Parameters
        IReadOnlyDictionary<string, string> parameters = null;
        if (args != null && args.Length > 2) parameters = Json.Convert<IReadOnlyDictionary<string, string>>(args[2]);

        var functionConfiguration = new TrakHoundFunctionConfiguration();

        // Create a new instance of the Function
        var function = new Function(functionConfiguration, client, volume);
        function.InstanceId = instanceInformation?.Id;
        function.LogReceived += FunctionLogReceived;

        var response = await function.Run(parameters);
        if (response.Success)
        {
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine("RESPONSE:");
            System.Console.WriteLine("--------------");
            System.Console.WriteLine($"ID = {response.Id}");
            System.Console.WriteLine($"StatusCode = {response.StatusCode}");
            System.Console.WriteLine();
            System.Console.WriteLine(response.ToJson(true));
        }

        //System.Console.ReadLine();
    }

    private static void FunctionLogReceived(object sender, TrakHound.Logging.TrakHoundLogItem item)
    {
        System.Console.WriteLine($"Log : {item.Timestamp.ToLocalDateTime()} : {item.LogLevel} : {item.Code} : {item.Message}");
    }
}