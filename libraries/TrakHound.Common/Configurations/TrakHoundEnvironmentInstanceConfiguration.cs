// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Configurations
{
    public class TrakHoundEnvironmentInstanceConfiguration
    {
        public const string DefaultHostname = "localhost";
        public const int DefaultPort = 8472;
        public const string DefaultPath = "/";
        public const bool DefaultUseSSL = false;


        public string Hostname { get; set; }

        public int Port { get; set; }

        public string Path { get; set; }

        public bool UseSSL { get; set; }


        public static TrakHoundEnvironmentInstanceConfiguration GetDefault()
        {
            var configuration = new TrakHoundEnvironmentInstanceConfiguration();
            configuration.Hostname = DefaultHostname;
            configuration.Port = DefaultPort;
            configuration.Path = DefaultPath;
            configuration.UseSSL = DefaultUseSSL;
            return configuration;
        }
    }
}
