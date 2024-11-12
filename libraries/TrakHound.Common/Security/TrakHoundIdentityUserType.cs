// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Security
{
    public enum TrakHoundIdentityUserType
    {
        Origin,   // The actual User
        Delegate  // An application that has delegate access from a User
    }
}
