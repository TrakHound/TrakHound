// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Licenses
{
    public interface ITrakHoundLicenseManager
    {
        string PublisherId { get; }


        void Refresh();


        string GetDeviceId();

        IEnumerable<TrakHoundLicenseInformation> GetLicenses();

        TrakHoundLicenseInformation GetLicenseByLicenseCode(string licenseCode);

        IEnumerable<TrakHoundLicenseInformation> GetLicensesByProduct(string productId);

        IEnumerable<TrakHoundLicenseInformation> GetLicensesByDevice(string deviceId);


        string GetAssignmentId(string productId, string assignmentKey);

        IEnumerable<TrakHoundLicenseAssignmentInformation> GetAssignments();

        TrakHoundLicenseAssignmentInformation GetAssignment(string assignmentId);

        IEnumerable<TrakHoundLicenseAssignmentInformation> GetAssignmentsByProduct(string productId);

        IEnumerable<TrakHoundLicenseAssignmentInformation> GetAssignmentsByDevice(string deviceId);

        IEnumerable<TrakHoundLicenseAssignmentInformation> GetAssignmentsByLicense(string licenseCode);


        TrakHoundLicenseInformation AddLicense(string productId, string deviceId, string licenseCode);

        void RemoveLicense(string licenseCode);

        void ClearLicenses();


        TrakHoundLicenseAssignmentInformation AddAssignment(string productId, string deviceId, string assignmentKey, string description = null);

        void RemoveAssignment(string assignmentId);


        TrakHoundLicenseAssignmentResult AssignLicense(string licenseCode, string assignmentId);

        void UnassignLicense(string assignmentId);

        void UnassignAll(string licenseCode);

        void UnassignAll();


        TrakHoundLicenseValidationResult ValidateAssignment(string assignmentId);

        TrakHoundLicenseValidationResult ValidateLicenseCode(string licenseCode);
    }
}
