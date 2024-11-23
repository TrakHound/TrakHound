// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using TrakHound.Drivers;
using TrakHound.Utilities;

namespace TrakHound.Sqlite.Drivers
{
    public abstract class SqliteDriver : TrakHoundDriver, IDisposable
    {
        private const string _defaultBaseDirectory = "sqlite";

        protected readonly SqliteClient _client;
        protected readonly string _baseDirectory;


        public SqliteClient Client => _client;

        public override bool IsAvailable => true;

        public override string AvailabilityMessage => null;


        public SqliteDriver() { }

        public SqliteDriver(ITrakHoundDriverConfiguration configuration) : base(configuration)
        {
            _baseDirectory = configuration.GetParameter("directory");
            if (!string.IsNullOrEmpty(_baseDirectory)) _baseDirectory = _defaultBaseDirectory;
            if (!string.IsNullOrEmpty(_baseDirectory))
            {
                if (!Path.IsPathRooted(_baseDirectory)) _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _baseDirectory);
            }
            else
            {
                _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }

            _client = new SqliteClient();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        public string GetDatabasePath(string filename)
        {
            var path = filename;

            var dir = Configuration.GetParameter("directory");
            if (!string.IsNullOrEmpty(dir)) path = Path.Combine(dir, path);

            if (!Path.IsPathRooted(path)) path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            return path;
        }
    }
}
