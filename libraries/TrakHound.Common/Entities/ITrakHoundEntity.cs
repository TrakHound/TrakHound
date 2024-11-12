// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// An Entity in the TrakHound Framework
    /// </summary>
    public interface ITrakHoundEntity
    {
        /// <summary>
        /// The Unique Identifier that identifies this Entity
        /// </summary>
        string Uuid { get; }

        /// <summary>
        /// Gets the Category of the TrakHound Entity
        /// </summary>
        byte Category { get; }

        /// <summary>
        /// Gets the Class of the TrakHound Entity
        /// </summary>
        byte Class { get; }

        /// <summary>
        /// Gets the Timestamp (Unix Ticks) of when the Entity was created
        /// </summary>
        long Created { get; }

        /// <summary>
        /// Gets a unique SHA-256 hash of the TrakHound Entity
        /// </summary>
        byte[] Hash { get; }

        /// <summary>
        /// Gets (true) if the TrakHound Entity contains valid values for the required properties
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Converts the TrakHound Entity to a JSON representation
        /// </summary>
        string ToJson();
    }
}
