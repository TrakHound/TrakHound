// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Security;

namespace TrakHound.Blazor
{
    public class TrakHoundAuthenticationSessionService
    {
        private readonly Dictionary<string, ITrakHoundSession> _sessions = new Dictionary<string, ITrakHoundSession>(); // ScopeId => Session
        private readonly object _lock = new object();


        public void Add(string scopeId, ITrakHoundSession session)
        {
            if (!string.IsNullOrEmpty(scopeId) && session != null)
            {
                lock (_lock)
                {
                    _sessions.Remove(scopeId);
                    _sessions.Add(scopeId, session);
                }
            }
        }

        public ITrakHoundSession Get(string scopeId)
        {
            if (scopeId != null)
            {
                lock (_lock)
                {
                    return _sessions.GetValueOrDefault(scopeId);
                }
            }

            return null;
        }
    }
}
