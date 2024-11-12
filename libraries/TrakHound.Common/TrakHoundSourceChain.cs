// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using TrakHound.Requests;

namespace TrakHound
{
    public class TrakHoundSourceChain
    {
        private readonly List<SourceChainSegment> _segments = new List<SourceChainSegment>();


        public enum SourceType
        {
            Instance,
            Api,
            Function,
            Service,
            Device,
            User
        }

        class SourceChainSegment
        {
            public string Type { get; set; }

            public string Sender { get; set; }
        }


        public void Add(string type, string sender)
        {
            if (!string.IsNullOrEmpty(sender))
            {
                var segment = new SourceChainSegment();
                segment.Type = type;
                segment.Sender = sender;
                _segments.Add(segment);
            }
        }

        public void Add(SourceType type, string sender)
        {
            if (!string.IsNullOrEmpty(sender))
            {
                var segment = new SourceChainSegment();
                segment.Type = type.ToString();
                segment.Sender = sender;
                _segments.Add(segment);
            }
        }

        public void Add(TrakHoundSourceEntry entry)
        {
            if (entry != null)
            {
                var segment = new SourceChainSegment();
                segment.Type = entry.Type;
                segment.Sender = entry.Sender;
                _segments.Add(segment);

                if (entry.Child != null)
                {
                    Add(entry.Child);
                }
            }
        }

        public void Add(TrakHoundSourceChain chain)
        {
            if (chain != null && !chain._segments.IsNullOrEmpty())
            {
                foreach (var segment in chain._segments)
                {
                    _segments.Add(segment);
                }
            }
        }


        public string GetUuid()
        {
            var entry = GetEntry();
            if (entry != null)
            {
                var entities = TrakHoundSourceEntry.GetEntities(entry);
                if (!entities.IsNullOrEmpty())
                {
                    return entities.LastOrDefault()?.Uuid;
                }
            }

            return null;
        }


        public TrakHoundSourceEntry GetEntry()
        {
            if (!_segments.IsNullOrEmpty())
            {
                var rootSegment = _segments[0];

                var rootEntry = new TrakHoundSourceEntry();
                rootEntry.Type = rootSegment.Type;
                rootEntry.Sender = rootSegment.Sender;

                var lastEntry = rootEntry;
                for (var i = 1; i < _segments.Count; i++)
                {
                    var childSegment = _segments[i];
                    var childEntry = new TrakHoundSourceEntry();
                    childEntry.Type = childSegment.Type;
                    childEntry.Sender = childSegment.Sender;
                    lastEntry.Child = childEntry;
                    lastEntry = childEntry;
                }

                return rootEntry;
            }

            return null;
        }
    }
}
