// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public partial class TrakHoundEntityPublishTransaction
    {
        [JsonPropertyName("object")]
        public IEnumerable<TrakHoundObjectEntry> Object
        {
            get => _object.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            var existingEntry = _object.GetValueOrDefault(x.EntryId);
                            if (existingEntry != null)
                            {
                                var o = existingEntry;
                                if (x.Priority >= o.Priority)
                                {
                                    o.Priority = x.Priority;
                                    o.ContentType = x.ContentType;
                                    o.DefinitionId = x.DefinitionId;
                                }

                                if (!existingEntry.Indexes.IsNullOrEmpty())
                                {
                                    var indexEntries = new List<TrakHoundIndexEntry>();
                                    indexEntries.AddRange(existingEntry.Indexes);
                                    if (!x.Indexes.IsNullOrEmpty()) indexEntries.AddRange(x.Indexes);

                                    o.Indexes = indexEntries;
                                }
                                else if (!x.Indexes.IsNullOrEmpty())
                                {
                                    o.Indexes = x.Indexes;
                                }

                                if (!existingEntry.Metadata.IsNullOrEmpty())
                                {
                                    var metadata = new Dictionary<string, string>();

                                    foreach (var m in existingEntry.Metadata)
                                    {
                                        metadata.Remove(m.Key);
                                        metadata.Add(m.Key, m.Value);
                                    }

                                    if (!x.Indexes.IsNullOrEmpty())
                                    {
                                        foreach (var m in x.Metadata)
                                        {
                                            if (!metadata.ContainsKey(m.Key) || x.Priority >= o.Priority)
                                            {
                                                metadata.Remove(m.Key);
                                                metadata.Add(m.Key, m.Value);
                                            }
                                        }
                                    }

                                    o.Metadata = metadata;
                                }

                                _object.Remove(o.EntryId);
                                _object.Add(o.EntryId, o);
                            }
                            else
                            {
                                _object.Add(x.EntryId, x);
                            }
                        }
                    }
                }
            }
        }

        public void Add(TrakHoundObjectEntry entry)
        {
            if (entry != null)
            {
                var existingEntry = _object.GetValueOrDefault(entry.EntryId);
                if (existingEntry != null)
                {
                    var o = existingEntry;
                    if (entry.Priority >= o.Priority)
                    {
                        o.Priority = entry.Priority;
                        o.ContentType = entry.ContentType;
                        o.DefinitionId = entry.DefinitionId;
                    }

                    if (!existingEntry.Indexes.IsNullOrEmpty())
                    {
                        var indexEntries = new List<TrakHoundIndexEntry>();
                        indexEntries.AddRange(existingEntry.Indexes);
                        if (!entry.Indexes.IsNullOrEmpty()) indexEntries.AddRange(entry.Indexes);

                        o.Indexes = indexEntries;
                    }
                    else if (!entry.Indexes.IsNullOrEmpty())
                    {
                        o.Indexes = entry.Indexes;
                    }

                    if (!existingEntry.Metadata.IsNullOrEmpty())
                    {
                        var metadata = new Dictionary<string, string>();

                        foreach (var m in existingEntry.Metadata)
                        {
                            metadata.Remove(m.Key);
                            metadata.Add(m.Key, m.Value);
                        }

                        if (!entry.Indexes.IsNullOrEmpty())
                        {
                            foreach (var m in entry.Metadata)
                            {
                                if (!metadata.ContainsKey(m.Key) || entry.Priority >= o.Priority)
                                {
                                    metadata.Remove(m.Key);
                                    metadata.Add(m.Key, m.Value);
                                }
                            }
                        }

                        o.Metadata = metadata;
                    }

                    _object.Remove(o.EntryId);
                    _object.Add(o.EntryId, o);
                }
                else
                {
                    _object.Add(entry.EntryId, entry);
                }
            }
        }
    }
}
