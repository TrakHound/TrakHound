// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound
{
    public class TrakHoundUuidDictionary<TValue>
    {
        private const byte _uuidLength = 32;
        private byte[] _index = new byte[0];
        private TValue[] _values = new TValue[0];
        private int _count;


        public int Count => _count;


        public byte[] GetUuid(int index)
        {
            if (index >= 0 && _index != null && index < _index.Length / _uuidLength)
            {
                var x = index * _uuidLength;

                var found = new byte[_uuidLength];
                for (var i = 0; i < _uuidLength; i++)
                {
                    found[i] = _index[x + i];
                }
                return found;
            }

            return null;
        }

        public byte[][] GetUuids()
        {
            if (_index != null && _index.Length > 0)
            {
                var n = _index.Length / _uuidLength;
                var x = 0;

                var values = new byte[n][];
                byte[] value;

                for (var i = 0; i < n; i++)
                {
                    value = new byte[_uuidLength];

                    for (var j = 0; j < _uuidLength; j++)
                    {
                        value[j] = _index[x];
                        x++;
                    }

                    values[i] = value;
                }

                return values;
            }

            return null;
        }

        public string[] GetUuidStrings()
        {
            var uuids = GetUuids();
            if (uuids != null && uuids.Length > 0)
            {
                var uuidStrings = new string[uuids.Length];

                for (var i = 0; i < uuids.Length; i++)
                {
                    uuidStrings[i] = TrakHoundUuid.ToString(uuids[i]);
                }

                return uuidStrings;
            }

            return null;
        }


        public TValue GetValue(string uuid)
        {
            return GetValue(TrakHoundUuid.FromString(uuid));
        }

        public TValue GetValue(byte[] uuid)
        {
            return GetValue(IndexOf(uuid));
        }

        public TValue GetValue(int index)
        {
            if (index >= 0 && index < _values.Length)
            {
                return _values[index];
            }

            return default;
        }

        public TValue[] GetValues()
        {
            if (_values.Length > 0)
            {
                var values = new TValue[_values.Length];
                for (var i = 0; i < _values.Length; i++)
                {
                    values[i] = _values[i];
                }
                return values;
            }

            return null;
        }


        public void Add(string uuid, TValue value)
        {
            Add(TrakHoundUuid.FromString(uuid), value);
        }

        public void Add(byte[] uuid, TValue value)
        {
            if (uuid != null && uuid.Length == _uuidLength && value != null)
            {
                if (!Contains(uuid))
                {
                    // Add Index (UUID)
                    var l = _index.Length;
                    Array.Resize(ref _index, l + _uuidLength);

                    for (var i = 0; i < _uuidLength; i++)
                    {
                        _index[l + i] = uuid[i];
                    }

                    // Add Value
                    var valueIndex = _values.Length;
                    Array.Resize(ref _values, valueIndex + 1);
                    _values[valueIndex] = value;

                    _count++;
                }
            }
        }

        public void AddRange(KeyValuePair<string, TValue>[] values)
        {
            if (values != null && values.Length > 0)
            {
                var insertValues = new KeyValuePair<byte[], TValue>[values.Length];
                byte[] uuid;

                for (var i = 0; i < values.Length; i++)
                {
                    uuid = TrakHoundUuid.FromString(values[i].Key);
                    if (uuid != null && uuid.Length == _uuidLength)
                    {
                        insertValues[i] = new KeyValuePair<byte[], TValue>(uuid, values[i].Value);
                    }
                }

                AddRange(insertValues);
            }
        }

        public void AddRange(KeyValuePair<byte[], TValue>[] values)
        {
            if (values != null && values.Length > 0)
            {
                var notFound = new List<int>(values.Length);
                for (var i = 0; i < values.Length; i++)
                {
                    if (!Contains(values[i].Key)) notFound.Add(i);
                }

                if (notFound.Count > 0)
                {
                    var n = notFound.Count;

                    // Add Index (UUID)
                    var l = _index.Length;
                    Array.Resize(ref _index, l + (n * _uuidLength));

                    for (var i = 0; i < n; i++)
                    {
                        var index = notFound[i];
                        for (var j = 0; j < _uuidLength; j++)
                        {
                            _index[l + (i * _uuidLength) + j] = values[index].Key[j];
                        }
                    }


                    // Add Value
                    var valueIndex = _values.Length;
                    Array.Resize(ref _values, valueIndex + n);
                    for (var i = 0; i < n; i++)
                    {
                        _values[valueIndex + i] = values[i].Value;
                    }

                    _count += n;
                }
            }
        }


        public bool Contains(byte[] uuid)
        {
            return IndexOf(uuid) >= 0;
        }

        public int IndexOf(byte[] uuid)
        {
            if (uuid != null && uuid.Length == _uuidLength)
            {
                var s = 0;
                var found = new byte[_uuidLength];
                var f = false;
                var si = 0;

                while (s < _index.Length - 1)
                {
                    si = 0;

                    if (_index[s] == uuid[si])
                    {
                        for (var i = 0; i < _uuidLength; i++)
                        {
                            if (_index[s] == uuid[si])
                            {
                                found[i] = _index[s];
                                si++;
                                s++;
                                if (i == _uuidLength - 1) f = true;
                            }
                            else
                            {
                                s += _uuidLength - i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        s += _uuidLength;
                    }

                    if (f)
                    {
                        return (s - _uuidLength) / _uuidLength;
                    }
                }
            }

            return -1;
        }
    }
}
