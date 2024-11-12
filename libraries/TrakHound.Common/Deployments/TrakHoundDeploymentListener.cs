// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Management;

namespace TrakHound.Deployments
{
    public class TrakHoundDeploymentListener : IDisposable
    {
        private readonly TrakHoundManagementClient _managementClient;
        private readonly string _profileId;
        private readonly int _interval;
        private readonly string _installPath;
        private readonly System.Timers.Timer _timer = new System.Timers.Timer();
        private string _installedHash;


        public TrakHoundManagementClient ManagementClient => _managementClient;

        public string ProfileId => _profileId;

        public int Interval => _interval;

        public string InstallPath => _installPath;

        public EventHandler<TrakHoundDeployment> DeploymentReceived { get; set; }


        public TrakHoundDeploymentListener(TrakHoundManagementClient managementClient, string profileId, int interval = 5000, string installPath = null)
        {
            _managementClient = managementClient;
            _profileId = profileId;
            _interval = interval;
            _installPath = installPath;

            _timer.Interval = interval;
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        public void Dispose()
        {
            if (_timer != null) _timer.Dispose();
        }



        private async void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var timer = (System.Timers.Timer)sender;
            timer.Stop();

            _installedHash = GetInstalledHash();

            var remoteInformation = await _managementClient.Deployments.GetDeployment(_profileId, null);
            if (remoteInformation != null)
            {
                if (remoteInformation.Hash != _installedHash)
                {
                    if (DeploymentReceived != null) DeploymentReceived.Invoke(this, remoteInformation);
                }
            }

            timer.Start();
        }


        private string GetInstalledHash()
        {
            if (!string.IsNullOrEmpty(_profileId))
            {
                var information = TrakHoundDeployment.ReadInformation(_installPath);
                if (information != null)
                {
                    return information.Hash;
                }
            }

            return null;
        }
    }
}
