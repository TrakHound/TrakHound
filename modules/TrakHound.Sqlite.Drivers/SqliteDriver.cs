// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using TrakHound.Drivers;
using TrakHound.Entities;
using TrakHound.Utilities;

namespace TrakHound.Sqlite.Drivers
{
    public abstract class SqliteDriver : TrakHoundDriver, IDisposable
    {
        private const string _defaultBaseDirectory = "sqlite";

        protected readonly string _baseDirectory;
        protected readonly SqliteClient _client;


        public SqliteClient Client => _client;

        public override bool IsAvailable => true;

        public override string AvailabilityMessage => null;


        public SqliteDriver() { }

        public SqliteDriver(ITrakHoundDriverConfiguration configuration) : base(configuration)
        {
            // Set Base Directory for Sqlite DataSource
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

        protected string GetDataSource<TEntity>() where TEntity : ITrakHoundEntity
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            return Path.Combine(_baseDirectory, "entities", $"trakhound_{entityCategory}_{entityClass}.sqlite".ToLower());
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();
        }
    }
}
