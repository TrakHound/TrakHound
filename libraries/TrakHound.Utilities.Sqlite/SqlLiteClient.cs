// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TrakHound.Utilities
{
    public partial class SqliteClient
    {
        private readonly string _filePath;
        private readonly string _dataSource;


        public EventHandler<Exception> Error { get; set; }


        public SqliteClient(string filePath)
        {
            _filePath = filePath;
            _dataSource = $"Data Source={GetDatabaseName(filePath)};Mode=ReadWriteCreate";
        }


        public void Insert<T>(IEnumerable<T> items, string tableName = null, string[] keys = null)
        {
            if (!items.IsNullOrEmpty())
            {
                var table = !string.IsNullOrEmpty(tableName) ? tableName : GetTableName<T>();
                var queries = new List<string>();

                // Create Table (if not exists)
                queries.Add(CreateTableQuery<T>(table, keys));

                // Create Inserts
                queries.AddRange(CreateInsertQueries<T>(table, items));

                // Execute Queries
                ExecuteNonQuery(queries);
            }
        }

        public T Read<T>(string key, string primaryKey = "Id")
        {
            if (!string.IsNullOrEmpty(key))
            {
                var tableName = GetTableName<T>();

                var query = $"select * from {tableName} where {primaryKey} = '{key}'";

                var results = ReadList<T>(query);
                if (!results.IsNullOrEmpty())
                {
                    return results.FirstOrDefault();
                }
            }

            return default;
        }

        public T Top<T>(string property, string value, string sortProperty)
        {
            if (!string.IsNullOrEmpty(property) && !string.IsNullOrEmpty(value))
            {
                var tableName = GetTableName<T>();

                var query = $"select * from {tableName} where {PropertyToColumn(property)} = '{value}' order by {sortProperty} desc limit 1";

                var results = ReadList<T>(query);
                if (!results.IsNullOrEmpty())
                {
                    return results.FirstOrDefault();
                }
            }

            return default;
        }

        public IEnumerable<T> Query<T>(string property, string value)
        {
            if (!string.IsNullOrEmpty(property) && !string.IsNullOrEmpty(value))
            {
                var tableName = GetTableName<T>();

                var query = $"select * from {tableName} where {PropertyToColumn(property)} = '{value}'";

                return ReadList<T>(query);
            }

            return default;
        }

        public IEnumerable<T> Query<T>(IEnumerable<KeyValuePair<string, string>> pairs)
        {
            if (!pairs.IsNullOrEmpty())
            {
                var tableName = GetTableName<T>();

                var conditions = new List<string>();
                foreach (var pair in pairs)
                {
                    conditions.Add($"{PropertyToColumn(pair.Key)} = '{pair.Value}'");
                }
                var where = string.Join(" and ", conditions);

                var query = $"select * from {tableName} where {where}";

                return ReadList<T>(query);
            }

            return default;
        }

        public IEnumerable<T> QueryLike<T>(string property, string like, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            if (!string.IsNullOrEmpty(property) && !string.IsNullOrEmpty(like))
            {
                var tableName = GetTableName<T>();

                var where = "";
                if (!pairs.IsNullOrEmpty())
                {
                    var conditions = new List<string>();
                    foreach (var pair in pairs)
                    {
                        conditions.Add($"{PropertyToColumn(pair.Key)} = '{pair.Value}'");
                    }
                    where = " and " + string.Join(" and ", conditions);
                }

                var query = $"select * from {tableName} where {PropertyToColumn(property)} like '{like}'{where}";

                return ReadList<T>(query);
            }

            return default;
        }


        public bool ExecuteNonQuery(string query)
        {
            try
            {
                var indexDir = Path.GetDirectoryName(GetDatabaseName(_filePath));
                if (!Directory.Exists(indexDir)) Directory.CreateDirectory(indexDir);

                using (var connection = new SqliteConnection(_dataSource))
                {
                    // Open the connection
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = query;
                    command.ExecuteNonQuery();

                    connection.Close();
                }

                return true;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"ERROR : {ex.Message} : {query}");

                if (Error != null) Error.Invoke(this, ex);
            }
            catch (Exception ex)
            {
                if (Error != null) Error.Invoke(this, ex);
            }

            return false;
        }

        public bool ExecuteNonQuery(IEnumerable<string> queries)
        {
            if (queries != null && queries.Count() > 0)
            {
                try
                {
                    var indexDir = Path.GetDirectoryName(GetDatabaseName(_filePath));
                    if (!Directory.Exists(indexDir)) Directory.CreateDirectory(indexDir);

                    using (var connection = new SqliteConnection(_dataSource))
                    {
                        // Open the connection
                        connection.Open();

                        using (var transaction = connection.BeginTransaction())
                        {
                            foreach (var query in queries)
                            {
                                if (!string.IsNullOrEmpty(query))
                                {
                                    var insertCommand = connection.CreateCommand();
                                    insertCommand.Transaction = transaction;
                                    insertCommand.CommandText = query;
                                    insertCommand.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }

                        connection.Close();
                    }

                    return true;
                }
                catch (SqliteException ex)
                {
                    if (Error != null) Error.Invoke(this, ex);
                }
                catch (Exception ex)
                {
                    if (Error != null) Error.Invoke(this, ex);
                }
            }

            return false;
        }


        public IEnumerable<string> ReadList(string query, int timeout = 0)
        {
            if (!string.IsNullOrEmpty(query))
            {
                try
                {
                    var list = new List<string>();

                    using (var connection = new SqliteConnection(_dataSource))
                    {
                        // Open the connection
                        connection.Open();

                        using var command = new SqliteCommand(query, connection);
                        command.CommandTimeout = timeout > 0 ? timeout : 60;

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            bool first = true;

                            while (first || reader.NextResult())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        var item = reader.GetValue(0);
                                        if (item != null) list.Add(item.ToString());
                                    }
                                }

                                first = false;
                            }
                        }
                    }

                    return list;
                }
                catch (SqliteException ex)
                {
                    if (Error != null) Error.Invoke(this, ex);
                }
                catch (Exception ex)
                {
                    if (Error != null) Error.Invoke(this, ex);
                }
            }

            return null;
        }

        public IEnumerable<T> ReadList<T>(string query, int timeout = 0)
        {
            if (!string.IsNullOrEmpty(query))
            {
                try
                {
                    var list = new List<T>();

                    using (var connection = new SqliteConnection(_dataSource))
                    {
                        // Open the connection
                        connection.Open();

                        using var command = new SqliteCommand(query, connection);
                        command.CommandTimeout = timeout > 0 ? timeout : 60;

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            bool first = true;

                            while (first || reader.NextResult())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        var item = Read<T>(reader);
                                        if (item != null) list.Add(item);
                                    }
                                }

                                first = false;
                            }
                        }
                    }

                    return list;
                }
                catch (SqliteException ex)
                {
                    if (Error != null) Error.Invoke(this, ex);
                }
                catch (Exception ex)
                {
                    if (Error != null) Error.Invoke(this, ex);
                }
            }

            return null;
        }

        public string ReadString(string query, int timeout = 0)
        {
            if (!string.IsNullOrEmpty(query))
            {
                try
                {
                    using (var connection = new SqliteConnection(_dataSource))
                    {
                        // Open the connection
                        connection.Open();

                        using var command = new SqliteCommand(query, connection);
                        command.CommandTimeout = timeout > 0 ? timeout : 60;

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            bool first = true;

                            while (first || reader.NextResult())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        var obj = reader.GetValue(0);
                                        if (obj != null)
                                        {
                                            return obj.ToString();
                                        }
                                    }
                                }

                                first = false;
                            }
                        }
                    }
                }
                catch (SqliteException ex)
                {
                    if (Error != null) Error.Invoke(this, ex);
                }
                catch (Exception ex)
                {
                    if (Error != null) Error.Invoke(this, ex);
                }
            }

            return null;
        }

        public IEnumerable<string> ReadStringList(string query, int timeout = 0)
        {
            if (!string.IsNullOrEmpty(query))
            {
                try
                {
                    var list = new List<string>();

                    using (var connection = new SqliteConnection(_dataSource))
                    {
                        // Open the connection
                        connection.Open();

                        using var command = new SqliteCommand(query, connection);
                        command.CommandTimeout = timeout > 0 ? timeout : 60;

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            bool first = true;

                            while (first || reader.NextResult())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        var obj = reader.GetValue(0);
                                        if (obj != null) list.Add(obj.ToString());
                                    }
                                }

                                first = false;
                            }
                        }
                    }

                    return list;
                }
                catch (SqliteException ex)
                {
                    if (Error != null) Error.Invoke(this, ex);
                }
                catch (Exception ex)
                {
                    if (Error != null) Error.Invoke(this, ex);
                }
            }

            return Enumerable.Empty<string>();
        }

        public T Read<T>(SqliteDataReader reader)
        {
            try
            {
                var obj = (T)Activator.CreateInstance(typeof(T));

                // Get object's properties
                var properties = typeof(T).GetProperties().ToList();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var column = reader.GetName(i);
                    var value = reader.GetValue(i);

                    if (value != DBNull.Value)
                    {
                        var property = properties.Find(o => PropertyToColumn(o.Name) == column);
                        if (property != null && value != null)
                        {
                            object val = default(T);

                            if (property.PropertyType == typeof(string))
                            {
                                string s = value.ToString();
                                if (!string.IsNullOrEmpty(s)) val = s;
                            }
                            //else if (property.PropertyType == typeof(DateTime))
                            //{
                            //    long ms = (long)value;
                            //    val = UnixTimeExtensions.EpochTime.AddMilliseconds(ms);
                            //}
                            else
                            {
                                val = Convert.ChangeType(value, property.PropertyType);
                            }

                            property.SetValue(obj, val, null);
                        }
                    }
                }

                return obj;
            }
            catch (SqliteException ex)
            {
                if (Error != null) Error.Invoke(this, ex);
            }
            catch (Exception ex)
            {
                if (Error != null) Error.Invoke(this, ex);
            }

            return default;
        }


        public static string CreateTableQuery<T>(string tableName, params string[] keys)
        {
            if (!string.IsNullOrEmpty(tableName) && !keys.IsNullOrEmpty())
            {
                // Get object's properties
                var properties = typeof(T).GetProperties().ToList();
                if (!properties.IsNullOrEmpty())
                {
                    var columns = new List<string>();

                    foreach (var property in properties)
                    {
                        columns.Add($"[{PropertyToColumn(property.Name)}] {GetDataType(property.PropertyType)}");
                    }

                    var columnsList = string.Join(',', columns);

                    var primaryKeys = $"PRIMARY KEY({string.Join(',', keys.Select(o => PropertyToColumn(o)))})";

                    return $"create table if not exists {tableName} ({columnsList}, {primaryKeys});";
                }
            }

            return null;
        }

        public static IEnumerable<string> CreateInsertQueries<T>(string tableName, IEnumerable<T> items)
        {
            if (!items.IsNullOrEmpty())
            {
                var queries = new List<string>();

                // Get object's properties
                var properties = typeof(T).GetProperties().ToList();

                foreach (var item in items)
                {
                    var columns = new List<string>();
                    var values = new List<string>();

                    if (!properties.IsNullOrEmpty())
                    {
                        foreach (var property in properties)
                        {
                            columns.Add($"[{PropertyToColumn(property.Name)}]");

                            var value = property.GetValue(item);
                            if (value != null)
                            {
                                var escapedValue = value.ToString().Replace("'", "''");

                                values.Add($"'{escapedValue}'");
                            }
                            else
                            {
                                values.Add("null");
                            }
                        }
                    }

                    var columnsList = string.Join(',', columns);
                    var valuesList = string.Join(',', values);

                    queries.Add($"insert or replace into {tableName} ({columnsList}) values ({valuesList});");
                }

                return queries;
            }

            return null;
        }

        public static IEnumerable<string> CreateDeleteQueries(string tableName, IEnumerable<string> keys, string primaryKey = "Id")
        {
            if (!string.IsNullOrEmpty(tableName) && !keys.IsNullOrEmpty())
            {
                var queries = new List<string>();

                foreach (var key in keys)
                {
                    queries.Add($"delete from {tableName} where {PropertyToColumn(primaryKey)} = '{key}';");
                }

                return queries;
            }

            return null;
        }


        private static string PropertyToColumn(string propertyName)
        {
            if (propertyName != propertyName.ToUpper())
            {
                // Split string by Uppercase characters
                var parts = Regex.Split(propertyName, @"(?<!^)(?=[A-Z])");
                string s = string.Join("_", parts);
                return s.ToLower();
                //return $"[{s.ToLower()}]";
            }
            else return propertyName.ToLower();
        }

        private static string GetDataType(Type type)
        {
            if (type != null)
            {
                if (type == typeof(string)) return "text";
                if (type == typeof(byte)) return "integer";
                if (type == typeof(short)) return "integer";
                if (type == typeof(ushort)) return "integer";
                if (type == typeof(int)) return "integer";
                if (type == typeof(uint)) return "integer";
                if (type == typeof(long)) return "integer";
                if (type == typeof(ulong)) return "integer";
                if (type == typeof(double)) return "real";
                if (type == typeof(float)) return "real";
                if (type == typeof(decimal)) return "real";
            }

            return null;
        }

        public static string GetTableName<T>()
        {
            return PropertyToColumn(typeof(T).Name);
        }

        private static string GetDatabaseName(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var x = path;
                if (!System.IO.Path.HasExtension(x)) return $"{x}.db";

                return x;
            }

            return null;
        }
    }
}
