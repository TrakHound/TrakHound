// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json.Serialization;
using TrakHound.Entities;
using TrakHound.Modules;

namespace TrakHound.Requests
{
    public class TrakHoundSourceEntry : TrakHoundEntityEntryBase
    {
        private string _uuid;
        private string _path;


        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(Type, Sender, TrakHoundEntityCategoryName.Sources);
                return _entryId;
            }
        }

        [JsonPropertyName("path")]
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                _uuid = null;
            }
        }


        //[JsonPropertyName("uuid")]
        //public string Uuid => $"{Type}:{Sender}";

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        [JsonPropertyName("metadata")]
        public IEnumerable<TrakHoundSourceMetadata> Metadata { get; set; }

        [JsonPropertyName("child")]
        public TrakHoundSourceEntry Child { get; set; }


        public TrakHoundSourceEntry() { }

        public TrakHoundSourceEntry(string type, string sender)
        {
            Type = type;
            Sender = sender;
        }


        public static IEnumerable<ITrakHoundSourceEntity> GetEntities(TrakHoundSourceEntry entry, string parentUuid = null, long created = 0)
        {
            var entities = new List<ITrakHoundSourceEntity>();

            if (entry != null)
            {
                var timestamp = created > 0 ? created : UnixDateTime.Now;

                var entity = new TrakHoundSourceEntity(entry.Type, entry.Sender, parentUuid, timestamp);
                entities.Add(entity);

                if (entry.Child != null)
                {
                    entities.AddRange(GetEntities(entry.Child, entity.Uuid, timestamp));
                }
            }

            return entities;
        }


        public static TrakHoundSourceEntry CreateModuleSource(ITrakHoundModule module)
        {
            if (module != null && module.Package != null)
            {
                var source = new TrakHoundSourceEntry();
                source.Type = module.Package.Category.ToTitleCase();
                source.Sender = $"{module.Package.Id}:{module.Package.Version}";
                return source;
            }

            return null;
        }

        public static TrakHoundSourceEntry CreateUserSource()
        {
            if (Environment.UserName != null)
            {
                var source = new TrakHoundSourceEntry();
                source.Type = "User";
                source.Sender = Environment.UserName;
                return source;
            }

            return null;
        }

        public static TrakHoundSourceEntry CreateApplicationSource()
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName();
            if (assemblyName != null)
            {
                var source = new TrakHoundSourceEntry();
                source.Type = "Application";
                source.Sender = $"{assemblyName.Name}:{assemblyName.Version}";
                return source;
            }

            return null;
        }

        public static TrakHoundSourceEntry CreateDeviceSource()
        {
            if (Environment.MachineName != null)
            {
                var source = new TrakHoundSourceEntry();
                source.Type = "Device";
                source.Sender = Environment.MachineName;
                return source;
            }

            return null;
        }

        public static TrakHoundSourceEntry CreateNetworkSource()
        {
            var ipAddress = GetLocalIPAddress();
            if (ipAddress != null)
            {
                var source = new TrakHoundSourceEntry();
                source.Type = "Network";
                source.Sender = ipAddress;
                return source;
            }

            return null;
        }

        public static TrakHoundSourceEntry CreateInstanceSource(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                var source = new TrakHoundSourceEntry();
                source.Type = "Instance";
                source.Sender = instanceId;
                return source;
            }

            return null;
        }

        public static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch { }

            return null;
        }
    }
}
