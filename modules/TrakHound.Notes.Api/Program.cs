using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Debug.AspNetCore;
using TrakHound.Volumes;

internal class Program
{
    public static async Task Main(string[] args)
    {
        // Create new TrakHoundClient based on the Instance BaseUrl and Router
        var clientConfiguration = new TrakHoundHttpClientConfiguration("localhost", 8472);

        var client = new TrakHoundHttpClient(clientConfiguration, null);
        client.AddMiddleware(new TrakHoundSourceMiddleware());

        var volumePath = Path.Combine(AppContext.BaseDirectory, "volume");
        var volume = new TrakHoundVolume("volume", volumePath);

        var instanceInformation = await client.System.Instances.GetHostInformation();

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.MapTrakHoundApiEndpoints(client, volume, instanceInformation?.Id);
        app.Run();
    }
}