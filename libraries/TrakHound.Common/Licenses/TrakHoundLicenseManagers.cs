// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Reflection;
using System;

namespace TrakHound.Licenses
{
    public static class TrakHoundLicenseManagers
    {
        private readonly static Dictionary<string, ITrakHoundLicenseManager> _managers = new Dictionary<string, ITrakHoundLicenseManager>(); // PublisherId => LicenseManager
        private readonly static object _lock = new object();


        public static event EventHandler<string> ManagerAdded;

        public static event EventHandler<Exception> ManagerLoadError;


        public static void Initialize()
        {
            var licenseManagerTypes = Assemblies.GetTypes<ITrakHoundLicenseManager>();
            if (!licenseManagerTypes.IsNullOrEmpty())
            {
                foreach (var licenseManagerType in licenseManagerTypes)
                {
                    try
                    {
                        var constructor = licenseManagerType.GetConstructor(new Type[] { });
                        if (constructor != null)
                        {
                            var licenseManager = (ITrakHoundLicenseManager)constructor.Invoke(new object[] { });
                            if (!string.IsNullOrEmpty(licenseManager.PublisherId))
                            {
                                lock (_lock)
                                {
                                    _managers.Remove(licenseManager.PublisherId);
                                    _managers.Add(licenseManager.PublisherId, licenseManager);
                                }

                                if (ManagerAdded != null) ManagerAdded.Invoke(null, licenseManager.PublisherId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ManagerLoadError != null) ManagerLoadError.Invoke(null, ex);
                    }
                }
            }
        }


        public static IEnumerable<ITrakHoundLicenseManager> Get()
        {
            lock (_lock)
            {
                return _managers.Values;
            }
        }

        public static ITrakHoundLicenseManager Get(string publisherId)
        {
            if (!string.IsNullOrEmpty(publisherId))
            {
                lock (_lock)
                {
                    return _managers.GetValueOrDefault(publisherId);
                }
            }

            return null;
        }
    }
}
