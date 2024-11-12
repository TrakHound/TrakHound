// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TrakHound
{
    public static class TrakHoundUuid
    {
        private const byte _separator = 58;


        public static byte[] Create(params object[] parts)
        {
            if (parts != null)
            {
                var length = 0;
                string part;
                for (var i = 0; i < parts.Length; i++)
                {
                    if (parts[i] != null)
                    {
                        part = parts[i].ToString();
                        length += part.Length;
                        if (i < parts.Length - 1) length++;
                    }
                }

                var b = new byte[length];
                var x = 0;

                for (var i = 0; i < parts.Length; i++)
                {
                    if (parts[i] != null)
                    {
                        part = parts[i].ToString();
                        var partBytes = Encoding.UTF8.GetBytes(part);
                        for (var j = 0; j < part.Length; j++)
                        {
                            b[x] = partBytes[j];
                            x++;
                        }

                        if (i < parts.Length - 1)
                        {
                            b[x] = _separator;
                            x++;
                        }
                    }
                }

                return System.Security.Cryptography.SHA256.HashData(b);
            }

            return null;
        }

        public static byte[] Create(Guid guid)
        {
            var bytes = guid.ToByteArray();
            if (bytes != null)
            {
                return SHA256.HashData(bytes);
            }

            return null;
        }

        public static byte[] Create(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var bytes = Encoding.UTF8.GetBytes(uuid);
                return SHA256.HashData(bytes);
            }

            return null;
        }

        public static string ToString(byte[] uuid)
        {
            if (uuid != null)
            {
                try
                {
                    return string.Concat(uuid.Select(o => o.ToString("x2")));
                }
                catch { }
            }

            return null;
        }

        public static byte[] FromString(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                try
                {
                    if (uuid.Length % 2 == 1) throw new Exception("The binary key cannot have an odd number of digits");

                    var a = new byte[uuid.Length >> 1];

                    for (int i = 0; i < uuid.Length >> 1; ++i)
                    {
                        a[i] = (byte)((GetHexVal(uuid[i << 1]) << 4) + (GetHexVal(uuid[(i << 1) + 1])));
                    }

                    return a;
                }
                catch { }
            }

            return null;
        }

        private static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static bool IsEqual(byte[] x, byte[] y)
        {
            return ObjectExtensions.ByteArraysEqual(x, y);
        }

        public class UuidComparer : IEqualityComparer<byte[]>
        {
            public bool Equals(byte[] left, byte[] right) => IsEqual(left, right);

            public int GetHashCode(byte[] key)
            {
                if (key == null) throw new ArgumentNullException("key");
                return key.Sum(b => b);
            }
        }
    }
}
