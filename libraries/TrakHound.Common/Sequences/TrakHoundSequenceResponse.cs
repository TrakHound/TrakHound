// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Sequences
{
    public struct TrakHoundSequenceResponse
    {
        public string SequenceId { get; set; }

        public double Value { get; set; }

        public long Created { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(SequenceId) && Value != 0 && Created > 0;


        public TrakHoundSequenceResponse(string sequenceId, double value, long created)
        {
            SequenceId = sequenceId;
            Value = value;
            Created = created;
        }
    }
}
