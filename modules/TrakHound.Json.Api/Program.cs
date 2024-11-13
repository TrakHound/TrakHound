// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Debug.AspNetCore;
using TrakHound.Volumes;

namespace TrakHound.Debug.Api
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Create new TrakHoundClient based on the Instance BaseUrl and Router
            var clientConfiguration = new TrakHoundHttpClientConfiguration("localhost", 8472);
            var client = new TrakHoundHttpClient(clientConfiguration, null);

            var volumePath = Path.Combine(AppContext.BaseDirectory, "volume");
            var volume = new TrakHoundVolume("volume", volumePath);

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapTrakHoundApiEndpoints(client, volume);
            app.Run();
        }
    }
}
