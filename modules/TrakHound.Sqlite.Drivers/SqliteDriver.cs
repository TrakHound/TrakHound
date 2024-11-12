// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Drivers;
using TrakHound.Utilities;

namespace TrakHound.Sqlite.Drivers
{
    public abstract class SqliteDriver : TrakHoundDriver, IDisposable
    {
        protected readonly SqliteClient _client;


        public SqliteClient Client => _client;

        public override bool IsAvailable => true;

        public override string AvailabilityMessage => null;


        public SqliteDriver() { }

        public SqliteDriver(ITrakHoundDriverConfiguration configuration) : base(configuration)
        {
            var path = "trakhound-entities.db";
            var dir = configuration.GetParameter("directory");
            if (!string.IsNullOrEmpty(dir)) path = System.IO.Path.Combine(dir, path);
            if (!System.IO.Path.IsPathRooted(path)) path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            _client = new SqliteClient(path);
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();
        }
    }
}
