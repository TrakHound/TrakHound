// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Configurations
{
    public interface ITrakHoundConfigurationProfile : IDisposable
    {
        IEnumerable<ITrakHoundConfiguration> Configurations { get; }

        string Id { get; }

        string Path { get; }

        string Name { get; set; }

        string Version { get; set; }

        bool Enabled { get; set; }


        event EventHandler<ITrakHoundConfiguration> ConfigurationAdded;

        event EventHandler<ITrakHoundConfiguration> ConfigurationRemoved;


        IEnumerable<TConfiguration> Get<TConfiguration>(string category) where TConfiguration : ITrakHoundConfiguration;

        TConfiguration Get<TConfiguration>(string category, string configurationId) where TConfiguration : ITrakHoundConfiguration;

        object Get(string category, string configurationId);


        void Add(ITrakHoundConfiguration configuration, bool save = false);

        void Add(string category, string configurationPath);

        void Remove(string category, string configurationId);

        void Clear(string category);


        IEnumerable<TConfiguration> Read<TConfiguration>(string category) where TConfiguration : ITrakHoundConfiguration;

        TConfiguration Read<TConfiguration>(string category, string configurationId) where TConfiguration : ITrakHoundConfiguration;


        void Save(string category, string configurationId);


        void Delete(string category, string configurationId);


        void Load<TConfiguration>(string category) where TConfiguration : ITrakHoundConfiguration;
    }
}
