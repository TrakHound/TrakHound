// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace TrakHound.Buffers
{
    class OperationFileBuffer<TItem> : IDisposable
    {
        private const int _sizeInterval = 10000;

        private const int _pageSequenceDigitCount = 20;
        private static readonly string _pageSequenceDigitFormat = CreateDigitFormat(_pageSequenceDigitCount);

        public const string DirectoryBuffer = "buffer";

        private readonly string _id;
        private readonly Func<ReadOnlyMemory<byte>, TItem> _deserializeFunction;
        private readonly Func<Stream, TItem, int> _serializeFunction;
        private readonly int _pageLimit;
        private readonly System.Timers.Timer _sizeTimer;
        private readonly object _lock = new object();

        private int _pageSize;
        private long _directorySize;


        public string Id => _id;

        public int PageSize => _pageSize;

        public int PageLimit => _pageLimit;

        public long DirectorySize => _directorySize;


        public OperationFileBuffer(
            string queueId,
            Func<ReadOnlyMemory<byte>, TItem> deserializeFunction,
            Func<Stream, TItem, int> serializeFunction,
            int pageLimit = 5000
            )
        {
            _id = queueId;
            _deserializeFunction = deserializeFunction;
            _serializeFunction = serializeFunction;
            _pageLimit = pageLimit;

            _sizeTimer = new System.Timers.Timer();
            _sizeTimer.Interval = _sizeInterval;
            _sizeTimer.Elapsed += SizeTimerElapsed;
            _sizeTimer.Start();

            _directorySize = GetDirectorySize(_id);
        }

        public void Dispose()
        {
            _sizeTimer.Elapsed -= SizeTimerElapsed;
            _sizeTimer.Dispose();
        }

        private void SizeTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _directorySize = GetDirectorySize(_id);
        }

        public ulong GetFirstPageSequence(ulong minimumSequence = 0)
        {
            var dirPath = GetDirectory(_id);
            var files = Directory.GetFiles(dirPath, "*", SearchOption.TopDirectoryOnly);
            if (!files.IsNullOrEmpty())
            {
                var nextPath = files.OrderBy(o => o).FirstOrDefault();
                if (!string.IsNullOrEmpty(nextPath))
                {
                    var filename = Path.GetFileNameWithoutExtension(nextPath);
                    var seqStr = filename.Substring(1);
                    return seqStr.ToULong();
                }
            }

            return minimumSequence;
        }

        public ulong GetLastPageSequence(ulong minimumSequence = 0)
        {
            var dirPath = GetDirectory(_id);
            var files = Directory.GetFiles(dirPath, "*", SearchOption.TopDirectoryOnly);
            if (!files.IsNullOrEmpty())
            {
                var nextPath = files.OrderByDescending(o => o).FirstOrDefault();
                if (!string.IsNullOrEmpty(nextPath))
                {
                    var filename = Path.GetFileNameWithoutExtension(nextPath);
                    var seqStr = filename.Substring(1);
                    return seqStr.ToULong();
                }
            }

            return minimumSequence;
        }


        public static string GetPageFilename(ulong sequence)
        {
            if (sequence > 0)
            {
                return $"_{sequence.ToString(_pageSequenceDigitFormat)}";
            }

            return null;
        }

        public string GetPagePath(ulong sequence, bool createIfNotExists = false)
        {
            var filename = GetPageFilename(sequence);
            if (filename != null)
            {
                var basePath = GetDirectory(_id, createIfNotExists);
                return Path.Combine(basePath, filename);
            }

            return null;
        }


        public int Read(Stream fileStream, ref byte[] readBuffer, int pageSize, ref TItem[] itemBuffer)
        {
            if (fileStream != null && readBuffer != null && itemBuffer != null)
            {
                var bufferSize = readBuffer.Length;

                int b = fileStream.ReadByte();
                readBuffer[0] = (byte)b;
                int i = 0;
                int n = 1;
                int lastItemReadCount = 0;

                TItem item;
                int itemBufferIndex = 0;

                ReadOnlyMemory<byte> lineSpan;
                var bufferSpan = new ReadOnlyMemory<byte>(readBuffer);

                while (b >= 0 && n < pageSize && n < bufferSize)
                {
                    // Find Index of \n
                    while (b >= 0 && n < pageSize && n < bufferSize)
                    {
                        b = fileStream.ReadByte();
                        readBuffer[n] = (byte)b;
                        n++;
                        if (b == 13) break;
                    }

                    if (i < n)
                    {
                        // Create Slice of Line
                        lineSpan = bufferSpan.Slice(i, n - i);

                        // Deserialize to TItem
                        item = _deserializeFunction(lineSpan);
                        if (item != null)
                        {
                            lastItemReadCount = n;

                            itemBuffer[itemBufferIndex] = item;
                            itemBufferIndex++;
                            if (itemBufferIndex >= itemBuffer.Length) break;
                        }

                        i = n;
                    }
                    else
                    {
                        break;
                    }
                }

                // Return count of Items written to itemBuffer
                return itemBufferIndex;
            }

            return 0;
        }

        public int Write(Stream stream, ref int pageSize, int pageLimit, ref TItem[] itemBuffer, int itemCount)
        {
            int written = 0;
            var newLine = new byte[] { 13 };

            for (var i = written; i < itemCount; i++)
            {
                // Serialize Item and Write to Stream
                var itemSize = _serializeFunction(stream, itemBuffer[i]);
                written++;

                // Write New Line
                stream.Write(newLine);

                pageSize += itemSize;
                if (pageSize > pageLimit)
                {
                    break;
                }
            }

            return written;
        }

        public long GetDirectorySize(string queueId)
        {
            try
            {
                if (GetDirectoryExists(queueId))
                {
                    var dir = GetDirectory(queueId);
                    var files = Directory.GetFiles(dir);
                    if (!files.IsNullOrEmpty())
                    {
                        long x = 0;

                        foreach (var file in files)
                        {
                            var fileInfo = new FileInfo(file);
                            if (fileInfo != null) x += fileInfo.Length;
                        }

                        return x;
                    }
                }
            }
            catch (Exception) { }

            return 0;
        }


        public bool GetDirectoryExists(string queueId)
        {
            string name = $"{queueId}".ToSnakeCase().ToLower();
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DirectoryBuffer, name);

            return Directory.Exists(dir);
        }

        public string GetDirectory(string queueId, bool createIfNotExists = true)
        {
            string name = $"{queueId}".ToSnakeCase().ToLower();
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DirectoryBuffer, name);
            if (createIfNotExists && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return dir;
        }


        private static string CreateDigitFormat(int digitCount)
        {
            if (digitCount > 0)
            {
                var builder = new StringBuilder();
                for (var i = 0; i < digitCount; i++)
                {
                    builder.Append('0');
                }
                return builder.ToString();
            }

            return null;
        }
    }
}
