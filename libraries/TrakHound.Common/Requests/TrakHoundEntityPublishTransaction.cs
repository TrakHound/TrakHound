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
                                if (!existingEntry.Indexes.IsNullOrEmpty())
                                {
                                    var indexEntries = new List<TrakHoundIndexEntry>();
                                    indexEntries.AddRange(existingEntry.Indexes);
                                    if (!x.Indexes.IsNullOrEmpty()) indexEntries.AddRange(x.Indexes);

                                    x.Indexes = indexEntries;
                                }

                                _object.Remove(x.EntryId);
                                _object.Add(x.EntryId, x);
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
    }
}
