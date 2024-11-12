// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Configurations;

namespace TrakHound.Clients
{
    public class TrakHoundHttpClientConfiguration
    {
        public const string DefaultHostname = "localhost";
        public const int DefaultPort = 8472;
        public const string DefaultPath = "/";
        public const bool DefaultUseSSL = false;
        public const int DefaultConnectionTimeout = 60000;

        public string Hostname { get; set; }

        public int Port { get; set; }

        public string Path { get; set; }

        public bool UseSSL { get; set; }

        public int ConnectionTimeout { get; set; }


        public TrakHoundHttpClientConfiguration(string hostname = DefaultHostname, int port = DefaultPort, string path = DefaultPath, bool useSSL = DefaultUseSSL)
        {
            Hostname = hostname;
            Port = port;
            Path = path;
            UseSSL = useSSL;
            ConnectionTimeout = DefaultConnectionTimeout;
        }


        public string GetHttpBaseUrl()
        {
            var protocol = UseSSL ? "https" : "http";

            return Url.Combine($"{protocol}://{Hostname}:{Port}", Path);
        }

        public string GetWebSocketBaseUrl()
        {
            var protocol = UseSSL ? "wss" : "ws";

            return Url.Combine($"{protocol}://{Hostname}:{Port}", Path);
        }
    }

    public static class TrakHoundClientConfigurationExtensions
    {
        public static TrakHoundHttpClientConfiguration GetClientConfiguration(this TrakHoundEnvironmentConfiguration environmentConfiguration)
        {
            if (environmentConfiguration != null && environmentConfiguration.Instance != null)
            {
                return new TrakHoundHttpClientConfiguration(
                    environmentConfiguration.Instance.Hostname,
                    environmentConfiguration.Instance.Port,
                    environmentConfiguration.Instance.Path,
                    environmentConfiguration.Instance.UseSSL
                    );
            }
            else
            {
                return new TrakHoundHttpClientConfiguration();
            }
        }
    }
}
