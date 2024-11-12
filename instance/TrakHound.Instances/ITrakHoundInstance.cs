// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Apps;
using TrakHound.Buffers;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Drivers;
using TrakHound.Functions;
using TrakHound.Management;
using TrakHound.Modules;
using TrakHound.Packages;
using TrakHound.Security;
using TrakHound.Services;
using TrakHound.Volumes;

namespace TrakHound.Instances
{
    public interface ITrakHoundInstance
    {
        string Id { get; }

        string Version { get; }

        TrakHoundInstanceType Type { get; }

        TrakHoundInstanceStatus Status { get; }

        DateTime LastUpdated { get; }

        DateTime CreatedTime { get; }

        DateTime? StartTime { get; }

        DateTime? StopTime { get; }


        TrakHoundInstanceInformation Information { get; }

        TrakHoundInstanceConfiguration Configuration { get; }

        ITrakHoundConfigurationProfile ConfigurationProfile { get; }


        TrakHoundPackageManager PackageManager { get; }
        ITrakHoundModuleProvider ModuleProvider { get; }
        ITrakHoundSecurityManager SecurityManager { get; }
        ITrakHoundDriverProvider DriverProvider { get; }
        ITrakHoundBufferProvider BufferProvider { get; }
        ITrakHoundAppProvider AppProvider { get; }
        ITrakHoundApiProvider ApiProvider { get; }
        TrakHoundFunctionManager FunctionManager { get; }
        TrakHoundServiceManager ServiceManager { get; }
        ITrakHoundVolumeProvider VolumeProvider { get; }

        ITrakHoundClientProvider ClientProvider { get; }
        //ITrakHoundProducer Producer { get; }


        event EventHandler Starting;

        event EventHandler Started;

        event EventHandler Stopping;

        event EventHandler Stopped;

        event TrakHoundInstanceStatusHandler StatusUpdated;

        event TrakHoundInstanceLogHandler LogUpdated;


        Task Start();

        Task Stop();


        Task InstallPackage(TrakHoundPackage package, bool downloadPackage = true, TrakHoundManagementClient managementClient = null);

        void CleanPackages();
    }
}
