// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.Json;

namespace TrakHound.Entities
{
    public struct EntityIndexPublishRequest
    {
        public string Uuid { get; }

        public string Target { get; set; }

        public string Subject { get; set; }

        public int DataType { get; set; }

        public string Value { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(Uuid) && !string.IsNullOrEmpty(Target) && !string.IsNullOrEmpty(Subject) && !string.IsNullOrEmpty(SourceUuid) && Created > 0;


        public EntityIndexPublishRequest(string target, string value, string subject, string sourceUuid = null, long created = 0)
        {
            Uuid = GenerateUuid(target, value, subject);
            Target = target != null ? target.ToLower() : null;
            DataType = (int)EntityIndexDataType.String;
            Subject = subject != null ? subject.ToLower() : null;
            Value = value;
            SourceUuid = sourceUuid;
            Created = created;
        }

        public EntityIndexPublishRequest(string target, byte value, string subject, string sourceUuid = null, long created = 0)
        {
            Uuid = GenerateUuid(target, value, subject);
            Target = target != null ? target.ToLower() : null;
            DataType = (int)EntityIndexDataType.Byte;
            Subject = subject != null ? subject.ToLower() : null;
            Value = value.ToString();
            SourceUuid = sourceUuid;
            Created = created;
        }

        public EntityIndexPublishRequest(string target, short value, string subject, string sourceUuid = null, long created = 0)
        {
            Uuid = GenerateUuid(target, value, subject);
            Target = target != null ? target.ToLower() : null;
            DataType = (int)EntityIndexDataType.Int16;
            Subject = subject != null ? subject.ToLower() : null;
            Value = value.ToString();
            SourceUuid = sourceUuid;
            Created = created;
        }

        public EntityIndexPublishRequest(string target, int value, string subject, string sourceUuid = null, long created = 0)
        {
            Uuid = GenerateUuid(target, value, subject);
            Target = target != null ? target.ToLower() : null;
            DataType = (int)EntityIndexDataType.Int32;
            Subject = subject != null ? subject.ToLower() : null;
            Value = value.ToString();
            SourceUuid = sourceUuid;
            Created = created;
        }

        public EntityIndexPublishRequest(string target, long value, string subject, string sourceUuid = null, long created = 0)
        {
            Uuid = GenerateUuid(target, value, subject);
            Target = target != null ? target.ToLower() : null;
            DataType = (int)EntityIndexDataType.Int64;
            Subject = subject != null ? subject.ToLower() : null;
            Value = value.ToString();
            SourceUuid = sourceUuid;
            Created = created;
        }

        public EntityIndexPublishRequest(string target, decimal value, string subject, string sourceUuid = null, long created = 0)
        {
            Uuid = GenerateUuid(target, value, subject);
            Target = target != null ? target.ToLower() : null;
            DataType = (int)EntityIndexDataType.Decimal;
            Subject = subject != null ? subject.ToLower() : null;
            Value = value.ToString();
            SourceUuid = sourceUuid;
            Created = created;
        }

        public EntityIndexPublishRequest(string target, float value, string subject, string sourceUuid = null, long created = 0)
        {
            Uuid = GenerateUuid(target, value, subject);
            Target = target != null ? target.ToLower() : null;
            DataType = (int)EntityIndexDataType.Float;
            Subject = subject != null ? subject.ToLower() : null;
            Value = value.ToString();
            SourceUuid = sourceUuid;
            Created = created;
        }

        public EntityIndexPublishRequest(string target, double value, string subject, string sourceUuid = null, long created = 0)
        {
            Uuid = GenerateUuid(target, value, subject);
            Target = target != null ? target.ToLower() : null;
            DataType = (int)EntityIndexDataType.Double;
            Subject = subject != null ? subject.ToLower() : null;
            Value = value.ToString();
            SourceUuid = sourceUuid;
            Created = created;
        }

        public EntityIndexPublishRequest(string target, DateTime value, string subject, string sourceUuid = null, long created = 0)
        {
            Uuid = GenerateUuid(target, value, subject);
            Target = target != null ? target.ToLower() : null;
            DataType = (int)EntityIndexDataType.Timestamp;
            Subject = subject != null ? subject.ToLower() : null;
            Value = value.ToUnixTime().ToString();
            SourceUuid = sourceUuid;
            Created = created;
        }

        public EntityIndexPublishRequest(string target, TimeSpan value, string subject, string sourceUuid = null, long created = 0)
        {
            Uuid = GenerateUuid(target, value, subject);
            Target = target != null ? target.ToLower() : null;
            DataType = (int)EntityIndexDataType.Duration;
            Subject = subject != null ? subject.ToLower() : null;
            Value = value.TotalNanoseconds.ToString();
            SourceUuid = sourceUuid;
            Created = created;
        }

        public EntityIndexPublishRequest(string target, EntityIndexDataType dataType, object value, string subject, string sourceUuid = null, long created = 0)
        {
            Uuid = GenerateUuid(target, value, subject);
            Target = target != null ? target.ToLower() : null;
            DataType = (int)dataType;
            Subject = subject != null ? subject.ToLower() : null;
            Value = value?.ToString();
            SourceUuid = sourceUuid;
            Created = created;
        }


        public static string GenerateUuid(string target, object value, string subject)
        {
            if (!string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(subject))
            {
                return $"{target.ToLower()}:{value}:{subject.ToLower()}".ToSHA256Hash();
            }

            return null;
        }


        public static int WriteJson(Stream stream, EntityIndexPublishRequest request)
        {
            try
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartArray();
                    writer.WriteStringValue(request.Target);
                    writer.WriteNumberValue(request.DataType);
                    writer.WriteStringValue(request.Subject);
                    writer.WriteStringValue(request.Value);
                    writer.WriteStringValue(request.SourceUuid);
                    writer.WriteNumberValue(request.Created);
                    writer.WriteEndArray();
                    writer.Flush();
                    return (int)writer.BytesCommitted + writer.BytesPending;
                }
            }
            catch { }

            return 0;
        }

        public static EntityIndexPublishRequest FromJson(ReadOnlySpan<byte> jsonBytes)
        {
            if (jsonBytes != null)
            {
                try
                {
                    var reader = new Utf8JsonReader(jsonBytes); reader.Read();
                    if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        reader.Read();
                        var target = reader.GetString(); reader.Read();
                        var dataType = reader.GetInt32(); reader.Read();
                        var subject = reader.GetString(); reader.Read();
                        var value = reader.GetString(); reader.Read();
                        var sourceUuid = reader.GetString(); reader.Read();
                        var created = reader.GetInt64();

                        return new EntityIndexPublishRequest(target, (EntityIndexDataType)dataType, value, subject, sourceUuid, created);
                    }
                }
                catch { }
            }            

            return default;
        }
    }
}
