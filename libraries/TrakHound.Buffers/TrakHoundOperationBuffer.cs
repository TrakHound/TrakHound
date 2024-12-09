// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TrakHound.Logging;

namespace TrakHound.Buffers
{
    public interface ITrakHoundOperationBuffer : IDisposable
    {
        string Id { get; }

        TrakHoundBufferMetrics Metrics { get; }
    }

    public interface ITrakHoundOperationBuffer<TItem> : ITrakHoundOperationBuffer
    {
        bool Add(TItem item);

        bool Add(IEnumerable<TItem> items);
    }


    public abstract class TrakHoundOperationBuffer<TItem> : ITrakHoundOperationBuffer<TItem>
    {
        private const int _metricsWindowsSize = 10000; // 10 Seconds
        private const int _metricsInterval = 1000; // 1 Second

        private const int _firstPageSequence = 100;
        private const int _firstRecoveryPageSequence = 1;

        private const int DefaultMaxItemsPerInterval = 5000;
        private const int DefaultQueuedItemLimit = DefaultMaxItemsPerInterval * 5;
        private const int DefaultProcessInterval = 5000;
        private const int DefaultRetryInterval = 10000;
        private const bool DefaultFileBufferEnabled = false;
        private const bool DefaultFileBufferForceEnabled = false;
        private const int DefaultFileBufferReadInterval = 100;
        private const int DefaultFileBufferPageSize = 5000000; // 5 MB
        private const int DefaultFileBufferWriteInterval = 100;

        private readonly TrakHoundBufferConfiguration _configuration;
        private readonly ITrakHoundLogger _logger = new TrakHoundLogger<TrakHoundOperationBuffer<TItem>>();
        private readonly TrakHoundBufferMetrics _metrics;
        private readonly MovingStatistics _queueItemStatistics;
        private readonly MovingStatistics _fileBufferItemReadStatistics;
        private readonly MovingStatistics _fileBufferByteReadStatistics;
        private readonly MovingStatistics _fileBufferItemWriteStatistics;
        private readonly MovingStatistics _fileBufferByteWriteStatistics;
        private readonly System.Timers.Timer _metricsTimer;
        private readonly object _lock = new object();
        
        private readonly OperationItemQueue<TItem> _workingQueue;
        private readonly int _maxItemsPerInterval;
        private readonly int _queuedItemLimit;
        private readonly int _processInterval;
        private readonly int _processRetryInterval;
        private readonly bool _fileBufferEnabled;
        private readonly bool _fileBufferForceEnabled;
        private readonly int _fileBufferReadInterval;
        private readonly int _fileBufferWriteInterval;
        private readonly int _fileBufferPageLimit;
        private readonly DelayEvent _writeCloseDelay;

        private string _id;
        private CancellationTokenSource stop;
        private bool _isStarted;

        private OperationFileBuffer<TItem> _fileBuffer;
        private FileBufferStatus _fileBufferStatus;
        private ulong _fileBufferReadPageSequence;
        private ulong _fileBufferWritePageSequence = _firstPageSequence;
        private Stream _writeStream;
        private int _writePageSize;
        private long _lastQueueTotalItemCount;
        private long _lastFileBufferTotalReadItemCount;
        private long _lastFileBufferTotalReadByteCount;
        private long _lastFileBufferTotalWriteItemCount;
        private long _lastFileBufferTotalWriteByteCount;

        enum FileBufferStatus
        {
            Idle,
            Active
        }


        /// <summary>
        /// Gets the Unique Identifier for the Buffer
        /// </summary>
        public string Id => _id;

        public TrakHoundBufferMetrics Metrics => _metrics;

        protected Func<bool> AvailabilityFunction { get; set; }

        protected Func<ReadOnlyMemory<TItem>, Task<bool>> ProcessFunction { get; set; }

        protected Func<ReadOnlyMemory<byte>, TItem> DeserializeFunction { get; set; }

        protected Func<Stream, TItem, int> SerializeFunction { get; set; }

        /// <summary>
        /// The interval at which the queue is processed
        /// </summary>
        public int ProcessInterval => _processInterval;

        /// <summary>
        /// The interval at which the queue is processed after a failure
        /// </summary>
        public int ProcessRetryInterval => _processRetryInterval;

        /// <summary>
        /// The maximum number of items to read from the queue at a time
        /// </summary>
        public int MaxItemsPerInterval => _maxItemsPerInterval;

        /// <summary>
        /// The maximum number of items to keep in the Queue at any point in time
        /// </summary>
        public int QueuedItemLimit => _queuedItemLimit;

        /// <summary>
        /// Gets the number of Items that are currently in the Queue
        /// </summary>
        public long QueuedItemCount => _workingQueue.Count;

        /// <summary>
        /// Determines whether the "Store and Forward" File Buffer is enabled
        /// </summary>
        public bool FileBufferEnabled => _fileBufferEnabled;

        /// <summary>
        /// Determines whether new Items are forced to the File Buffer instead of the Working Queue
        /// </summary>
        public bool FileBufferForceEnabled => _fileBufferForceEnabled;

        /// <summary>
        /// The size (in bytes) of the Maximum Page Size to use for the File Buffer
        /// </summary>
        public int FileBufferPageSize => _fileBufferPageLimit;

        /// <summary>
        /// The Interval at which the File Buffer is read
        /// </summary>
        public int FileBufferReadInterval => _fileBufferReadInterval;

        /// <summary>
        /// The Interval at which the File Buffer is written
        /// </summary>
        public int FileBufferWriteInterval => _fileBufferWriteInterval;


        public TrakHoundOperationBuffer(string bufferId, string driverId, string name = null, string operationType = null, TrakHoundBufferConfiguration bufferConfiguration = null)
        {
            _id = bufferId;

            // Set configuration properties to defaults
            _processInterval = DefaultProcessInterval;
            _processRetryInterval = DefaultRetryInterval;
            _maxItemsPerInterval = DefaultMaxItemsPerInterval;
            _queuedItemLimit = DefaultQueuedItemLimit;

            _fileBufferEnabled = DefaultFileBufferEnabled;
            _fileBufferForceEnabled = DefaultFileBufferForceEnabled;
            _fileBufferPageLimit = DefaultFileBufferPageSize;
            _fileBufferReadInterval = DefaultFileBufferReadInterval;
            _fileBufferWriteInterval = DefaultFileBufferWriteInterval;

            // Set configuration properties based on BufferConfiguration
            _configuration = bufferConfiguration;
            if (_configuration != null)
            {
                if (_configuration.Interval >= 0) _processInterval = _configuration.Interval;
                if (_configuration.RetryInterval >= 0) _processRetryInterval = _configuration.RetryInterval;
                if (_configuration.MaxItemsPerInterval > 0) _maxItemsPerInterval = _configuration.MaxItemsPerInterval;
                if (_configuration.QueuedItemLimit > 0) _queuedItemLimit = _configuration.QueuedItemLimit;

                _fileBufferEnabled = _configuration.FileBufferEnabled;
                _fileBufferForceEnabled = _configuration.FileBufferForceEnabled;
                if (_configuration.FileBufferPageSize > 0) _fileBufferPageLimit = _configuration.FileBufferPageSize;
                if (_configuration.FileBufferReadInterval > 0) _fileBufferReadInterval = _configuration.FileBufferReadInterval;
                if (_configuration.FileBufferWriteInterval > 0) _fileBufferWriteInterval = _configuration.FileBufferWriteInterval;
            }

            // Initialize Queues
            _workingQueue = new OperationItemQueue<TItem>(_queuedItemLimit);

            _queueItemStatistics = new MovingStatistics(_metricsWindowsSize / _metricsInterval);
            _fileBufferItemReadStatistics = new MovingStatistics(_metricsWindowsSize / _metricsInterval);
            _fileBufferByteReadStatistics = new MovingStatistics(_metricsWindowsSize / _metricsInterval);
            _fileBufferItemWriteStatistics = new MovingStatistics(_metricsWindowsSize / _metricsInterval);
            _fileBufferByteWriteStatistics = new MovingStatistics(_metricsWindowsSize / _metricsInterval);

            _metrics = new TrakHoundBufferMetrics();
            _metrics.BufferId = _id;
            _metrics.DriverId = driverId;
            _metrics.Name = name;
            _metrics.OperationType = operationType;
            _metrics.Queue.ItemLimit = _queuedItemLimit;

            _metricsTimer = new System.Timers.Timer();
            _metricsTimer.Interval = _metricsInterval;
            _metricsTimer.Elapsed += MetricsTimerElapsed;
            _metricsTimer.Start();

            _writeCloseDelay = new DelayEvent(5000);
            _writeCloseDelay.Elapsed += WriteCloseDelayElapsed;

            // Subscribe to ProcessExit (to Flush Queue on process exit)
            AppDomain.CurrentDomain.ProcessExit += OnExit;
        }

        private void OnExit(object sender, EventArgs args)
        {
            Stop();
        }


        protected void Start()
        {
            if (!_isStarted && ProcessFunction != null)
            {
                var now = UnixDateTime.Now;

                _metrics.StartTime = now;
                _metrics.LastUpdated = now;

                _isStarted = true;
                _fileBufferStatus = FileBufferStatus.Idle; // Needs to read if there are file buffer files available or not!!

                _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : Starting..");

                stop = new CancellationTokenSource();

                _ = Task.Run(() => ProcessWorker(stop.Token));

                if (_fileBufferEnabled && DeserializeFunction != null && SerializeFunction != null)
                {
                    _fileBuffer = new OperationFileBuffer<TItem>(Id, DeserializeFunction, SerializeFunction, _fileBufferPageLimit);
                    if (_fileBuffer != null)
                    {
                        _metrics.FileBuffer.IsEnabled = true;
                        _metrics.FileBuffer.PageSize = _fileBufferPageLimit;

                        _ = Task.Run(() => BufferReadWorker(stop.Token));
                    }
                }

                _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : Started");
            }
        }

        protected void Stop()
        {
            if (_isStarted)
            {
                var now = UnixDateTime.Now;

                _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : Stopping..");

                if (stop != null) stop.Cancel();

                if (_writeStream != null) _writeStream.Dispose();

                // Flush the File Buffer
                if (_fileBuffer != null)
                {
                    _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : Flushing Queue to File Buffer..");

                    FlushBuffer();

                    _fileBuffer.Dispose();
                }

                _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : Stopped");

                _isStarted = false;
                _metrics.StopTime = now;
                _metrics.LastUpdated = now;
            }
        }

        public void Dispose()
        {
            _metricsTimer.Elapsed -= MetricsTimerElapsed;
            _metricsTimer.Dispose();

            Stop();
            GC.SuppressFinalize(this);
        }


        private void MetricsTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Queue Metrics
            var queueTotalItemCount = _metrics.Queue.TotalItemCount;
            var queueItemDiff = queueTotalItemCount - _lastQueueTotalItemCount;
            _lastQueueTotalItemCount = queueTotalItemCount;
            _queueItemStatistics.Push(queueItemDiff);
            _metrics.Queue.ItemRate = Math.Round(Math.Max(0, _queueItemStatistics.Mean), 4);

            // FileBuffer Read Item Metrics
            var fileBufferReadTotalItemCount = _metrics.FileBuffer.TotalReadCount;
            var fileBufferReadItemDiff = fileBufferReadTotalItemCount - _lastFileBufferTotalReadItemCount;
            _lastFileBufferTotalReadItemCount = fileBufferReadTotalItemCount;
            _fileBufferItemReadStatistics.Push(fileBufferReadItemDiff);
            _metrics.FileBuffer.ItemReadRate = Math.Round(Math.Max(0, _fileBufferItemReadStatistics.Mean), 4);

            // FileBuffer Read Byte Metrics
            var fileBufferReadTotalByteCount = _metrics.FileBuffer.TotalBytesRead;
            var fileBufferReadByteDiff = fileBufferReadTotalByteCount - _lastFileBufferTotalReadByteCount;
            _lastFileBufferTotalReadByteCount = fileBufferReadTotalByteCount;
            _fileBufferByteReadStatistics.Push(fileBufferReadByteDiff);
            _metrics.FileBuffer.ByteReadRate = Math.Round(Math.Max(0, _fileBufferByteReadStatistics.Mean), 4);

            // FileBuffer Write Item Metrics
            var fileBufferWriteTotalItemCount = _metrics.FileBuffer.TotalWriteCount;
            var fileBufferWriteItemDiff = fileBufferWriteTotalItemCount - _lastFileBufferTotalWriteItemCount;
            _lastFileBufferTotalWriteItemCount = fileBufferWriteTotalItemCount;
            _fileBufferItemWriteStatistics.Push(fileBufferWriteItemDiff);
            _metrics.FileBuffer.ItemWriteRate = Math.Round(Math.Max(0, _fileBufferItemWriteStatistics.Mean), 4);

            // FileBuffer Write Byte Metrics
            var fileBufferWriteTotalByteCount = _metrics.FileBuffer.TotalBytesWritten;
            var fileBufferWriteByteDiff = fileBufferWriteTotalByteCount - _lastFileBufferTotalWriteByteCount;
            _lastFileBufferTotalWriteByteCount = fileBufferWriteTotalByteCount;
            _fileBufferByteWriteStatistics.Push(fileBufferWriteByteDiff);
            _metrics.FileBuffer.ByteWriteRate = Math.Round(Math.Max(0, _fileBufferByteWriteStatistics.Mean), 4);
        }


        public bool Add(TItem item)
        {
            if (_fileBufferStatus == FileBufferStatus.Idle && !_fileBufferForceEnabled)
            {
                var now = UnixDateTime.Now;
                    
                // Add to Entity Queue          
                if (!_workingQueue.Add(item))
                {
                    if (_fileBuffer != null)
                    {
                        _fileBufferStatus = FileBufferStatus.Active;
                        _metrics.FileBuffer.LastUpdated = now;

                        return BufferWrite(item);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    _metrics.Queue.TotalItemCount++;
                    _metrics.Queue.ItemCount++;
                    _metrics.Queue.LastUpdated = now;

                    return true;
                }
            }
            else
            {
                return BufferWrite(item);
            }
        }

        public bool Add(IEnumerable<TItem> items)
        {
            if (!items.IsNullOrEmpty())
            {
                var success = true;

                foreach (var item in items)
                {
                    success = Add(item);
                    if (!success) break;
                }

                return success;
            }

            return false;
        }


        private async void ProcessWorker(CancellationToken cancellationToken)
        {
            var processFunction = ProcessFunction;
            var workerStopwatch = new System.Diagnostics.Stopwatch();
            var processStopwatch = new System.Diagnostics.Stopwatch();

            try
            {
                var itemBuffer = new TItem[MaxItemsPerInterval];
                string itemLabel = "";

                long now;
                int count = 0;
                int totalCount = 0;

                do
                {
                    now = UnixDateTime.Now;

                    int interval = Math.Max(1, ProcessInterval);
                    workerStopwatch.Restart();

                    try
                    {
                        // Check if Driver is "Available"
                        if (AvailabilityFunction == null || AvailabilityFunction())
                        {
                            totalCount = _workingQueue.Count;

                            // Get (n) Items from working queue (if not previously failed)
                            if (count < 1)
                            {
                                count = _workingQueue.Get(ref itemBuffer, 0, _maxItemsPerInterval);

                                if (count > 0)
                                {
                                    itemLabel = count > 1 ? "Items" : "Item";
                                    _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : {count} / {totalCount} Items Read From Queue");
                                }
                            }

                            if (count > 0)
                            {
                                _metrics.Queue.IsActive = true;

                                processStopwatch.Restart();

                                // Process the Items from Working Queue
                                var response = await processFunction(new ReadOnlyMemory<TItem>(itemBuffer, 0, count));

                                processStopwatch.Stop();
                                double duration = ((double)processStopwatch.ElapsedTicks) / 10000;

                                if (response)
                                {
                                    _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : {count} {itemLabel} Processed Successfully in {duration}ms");

                                    // Remove Items from Working Queue
                                    _workingQueue.Remove(count);

                                    _metrics.Queue.ItemCount -= count;
                                    _metrics.Queue.LastUpdated = now;

                                    // Reset the Count (to read new items from Working Queue)
                                    count = 0;

                                    // Set Interval
                                    interval = _processInterval;
                                }
                                else
                                {
                                    _logger.Log(TrakHoundLogLevel.Debug, $"Buffer {Id} : Error Processing {count} {itemLabel} in {duration}ms");

                                    // Set Interval to RetryInterval
                                    interval = _processRetryInterval;
                                }
                            }
                            else
                            {
                                //_queueItemStatistics.Push(0);
                                _metrics.Queue.IsActive = false;

                                // Set Interval
                                interval = _processInterval;
                            }
                        }
                        else
                        {
                            _metrics.Queue.IsActive = false;
                            _fileBufferStatus = FileBufferStatus.Active;
                        }
                    }
                    catch (TaskCanceledException) { }
                    catch (Exception ex)
                    {
                        _logger.Log(TrakHoundLogLevel.Error, $"Buffer {Id} : Worker Error : {ex.Message}");
                    }

                    workerStopwatch.Stop();
                    _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : Worker Completed in {workerStopwatch.ElapsedMilliseconds}ms");

                    if (workerStopwatch.ElapsedMilliseconds < interval)
                    {
                        var waitInterval = interval - (int)workerStopwatch.ElapsedMilliseconds;

                        await Task.Delay(waitInterval, cancellationToken);
                    }

                } while (!cancellationToken.IsCancellationRequested);
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                _logger.Log(TrakHoundLogLevel.Error, $"Buffer {Id} : Worker Loop Error : {ex.Message}");
            }

            _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} Stopped");
        }

        private async void BufferReadWorker(CancellationToken cancel)
        {
            var interval = Math.Max(FileBufferReadInterval, 1);

            long now;

            Stream readStream = null;
            string pagePath = null;
            int pageSize = 0;

            var readBufferSize = _fileBufferPageLimit;
            var readBuffer = new byte[readBufferSize];

            var itemBufferSize = MaxItemsPerInterval;
            var itemBuffer = new TItem[itemBufferSize];

            await Task.Delay(interval / 2, cancel); // Delay to Offset from Process Worker

            do
            {
                try
                {
                    now = UnixDateTime.Now;

                    _metrics.FileBuffer.RemainingSize = _fileBuffer.DirectorySize;

                    await Task.Delay(interval, cancel);

                    if (AvailabilityFunction == null || AvailabilityFunction())
                    {
                        ulong readPageSequence;
                        ulong writePageSequence;

                        lock (_lock)
                        {
                            readPageSequence = _fileBufferReadPageSequence;
                            writePageSequence = _fileBufferWritePageSequence;
                        }

                        if (readStream == null)
                        {
                            // Get Next Page Sequence
                            if (readPageSequence < 1)
                            {
                                readPageSequence = _fileBuffer.GetFirstPageSequence();
                                lock (_lock) _fileBufferReadPageSequence = readPageSequence;
                            }

                            if (readPageSequence >= writePageSequence)
                            {
                                if (CloseWriteStream())
                                {
                                    writePageSequence = 0;
                                }
                            }

                            if (readPageSequence > 0 && (writePageSequence == 0 || readPageSequence < writePageSequence))
                            {
                                _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : File Buffer Reading from Page Sequence = {readPageSequence}");

                                pagePath = _fileBuffer.GetPagePath(readPageSequence);
                                if (File.Exists(pagePath))
                                {
                                    readStream = new FileStream(pagePath, FileMode.Open, FileAccess.Read);
                                    pageSize = (int)readStream.Length;

                                    //_fileBufferStatus = FileBufferStatus.Active;
                                }
                                else
                                {
                                    _fileBufferStatus = FileBufferStatus.Idle;

                                    readPageSequence = 0;
                                    lock (_lock) _fileBufferReadPageSequence = readPageSequence;
                                }
                            }
                        }

                        if (readStream != null)
                        {
                            if ((_workingQueue.Limit - _workingQueue.Count) >= itemBufferSize)
                            {
                                _metrics.FileBuffer.IsReadActive = true;
                                _metrics.FileBuffer.ReadPageSequence = readPageSequence;
                                _metrics.LastUpdated = now;

                                int readCount = 0;
                                int totalReadCount = 0;

                                do
                                {
                                    now = UnixDateTime.Now;
                                    var originalStreamPosition = readStream.Position;

                                    // Read from File Buffer
                                    readCount = _fileBuffer.Read(readStream, ref readBuffer, pageSize, ref itemBuffer);

                                    _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : {readCount} Items Read From File Buffer");

                                    for (var i = 0; i < readCount; i++)
                                    {
                                        // Add to Queue
                                        _workingQueue.Add(itemBuffer[i]);

                                        _metrics.Queue.TotalItemCount++;
                                        _metrics.Queue.ItemCount++;
                                        _metrics.Queue.LastUpdated = now;
                                    }

                                    if (readCount > 0) totalReadCount += readCount;

                                    _metrics.FileBuffer.TotalReadCount += readCount;
                                    _metrics.FileBuffer.TotalBytesRead += readStream.Position - originalStreamPosition;
                                    _metrics.LastUpdated = now;

                                } while (readCount > 0 && (_workingQueue.Limit - _workingQueue.Count) >= itemBufferSize);

                                if (readCount < 1)
                                {
                                    readStream.Dispose();
                                    readStream = null;
                                    pageSize = 0;
                                    readPageSequence = 0;
                                    lock (_lock) _fileBufferReadPageSequence = readPageSequence;

                                    _metrics.FileBuffer.IsReadActive = false;
                                    _metrics.FileBuffer.ReadPageSequence = readPageSequence;
                                    _metrics.LastUpdated = now;

                                    File.Delete(pagePath);
                                    GC.Collect();
                                }
                            }
                        }
                    }
                    else
                    {
                        _fileBufferStatus = FileBufferStatus.Active;
                    }

                    _metrics.FileBuffer.ItemReadRate = Math.Round(Math.Max(0, _fileBufferItemReadStatistics.Mean * (((double)1000) / _fileBufferReadInterval)), 2);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    _logger?.LogError($"Buffer {Id} : BufferReadWorker() : Exception : {ex.Message}");

                    pageSize = 0;

                    // Dispose of Read Stream
                    try
                    {
                        if (readStream != null)
                        {
                            readStream.Flush();
                            readStream.Dispose();
                            readStream = null;
                            GC.Collect();
                        }
                    }
                    catch { }
                    
                    lock (_lock) _fileBufferReadPageSequence = 0;
                }

            } while (!stop.IsCancellationRequested);

            // Dispose of Read Stream
            try
            {
                if (readStream != null)
                {
                    readStream.Flush();
                    readStream.Dispose();
                    readStream = null;
                    GC.Collect();
                }
            }
            catch { }
        }

        private bool BufferWrite(TItem item)
        {
            if (item != null)
            {
                var items = new TItem[1];
                items[0] = item;

                return BufferWrite(items);
            }

            return false;
        }

        private bool BufferWrite(TItem[] items)
        {
            if (_fileBuffer != null && items != null && items.Length > 0)
            {
                long now = UnixDateTime.Now;

                ulong readPageSequence;
                ulong writePageSequence;

                lock (_lock)
                {
                    readPageSequence = _fileBufferReadPageSequence;
                    writePageSequence = _fileBufferWritePageSequence;
                }

                if (writePageSequence == 0 || writePageSequence <= readPageSequence)
                {
                    writePageSequence = _fileBuffer.GetLastPageSequence(_firstPageSequence);
                    if (writePageSequence <= readPageSequence) writePageSequence = readPageSequence + 1;
                    lock (_lock) _fileBufferWritePageSequence = writePageSequence;

                    _metrics.FileBuffer.WritePageSequence = writePageSequence;
                    _metrics.LastUpdated = now;
                }

                try
                {
                    var itemsWritten = 0;

                    while (itemsWritten < items.Length)
                    {
                        _writeCloseDelay.Set();

                        if (_writeStream == null)
                        {
                            _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : BufferWrite() : File Buffer Writing to Page Sequence = {writePageSequence}");

                            var filePath = _fileBuffer.GetPagePath(writePageSequence, true);

                            _writeStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                            if (_writeStream != null)
                            {
                                _writePageSize = (int)_writeStream.Length;
                            }
                        }

                        if (_writeStream != null)
                        {
                            _metrics.FileBuffer.IsWriteActive = true;

                            var originalStreamLength = _writeStream.Length;
                            var writeCount = items.Length - itemsWritten;

                            itemsWritten = _fileBuffer.Write(_writeStream, ref _writePageSize, _fileBufferPageLimit, ref items, writeCount);

                            _metrics.FileBuffer.TotalWriteCount += itemsWritten;
                            _metrics.FileBuffer.TotalBytesWritten += _writeStream.Length - originalStreamLength;
                            _metrics.LastUpdated = now;

                            if (_writePageSize > _fileBufferPageLimit)
                            {
                                _writeStream.Flush();
                                _writeStream.Dispose();
                                _writeStream = null;
                                GC.Collect();

                                writePageSequence = _fileBuffer.GetLastPageSequence(_firstPageSequence) + 1;
                                _writePageSize = 0;

                                _metrics.FileBuffer.IsWriteActive = false;
                                _metrics.FileBuffer.WritePageSequence = writePageSequence;

                                break;
                            }
                        }
                    }

                    lock (_lock) _fileBufferWritePageSequence = writePageSequence;

                    return true;
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    _logger.Log(TrakHoundLogLevel.Error, $"Buffer {Id} : BufferWrite() : Exception : {ex.Message}");
                }
            }

            return false;
        }

        private void FlushBuffer()
        {
            if (_fileBuffer != null)
            {
                var itemBuffer = new TItem[_workingQueue.Limit];

                FileStream writeStream = null;
                int pageSize = 0;

                ulong pageSequence = _firstRecoveryPageSequence;

                try
                {
                    var count = _workingQueue.Take(ref itemBuffer, 0, _workingQueue.Limit);

                    if (count > 0)
                    {
                        var written = 0;

                        while (written < count)
                        {
                            if (writeStream == null)
                            {
                                _logger.Log(TrakHoundLogLevel.Trace, $"Buffer {Id} : FlushBuffer() : File Buffer Writing to Page Sequence = {pageSequence}");

                                var filePath = _fileBuffer.GetPagePath(pageSequence, true);

                                writeStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                                if (writeStream != null)
                                {
                                    pageSize = (int)writeStream.Length;
                                }
                            }

                            if (writeStream != null)
                            {
                                written = _fileBuffer.Write(writeStream, ref pageSize, _fileBufferPageLimit, ref itemBuffer, count);

                                if (pageSize > _fileBufferPageLimit)
                                {
                                    writeStream.Flush();
                                    writeStream.Dispose();
                                    writeStream = null;
                                    GC.Collect();

                                    pageSequence++;
                                    pageSize = 0;
                                    break;
                                }
                            }
                        }

                        if (writeStream != null)
                        {
                            writeStream.Flush();
                            writeStream.Dispose();
                            writeStream = null;
                            GC.Collect();
                        }
                    }
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    _logger.Log(TrakHoundLogLevel.Error, $"Buffer {Id} : FlushBuffer() : Exception : {ex.Message}");
                }
            }
        }

        private bool CloseWriteStream()
        {
            try
            {
                lock (_lock)
                {
                    _fileBufferWritePageSequence = 0;
                    _fileBufferStatus = FileBufferStatus.Idle;
                }

                if (_writeStream != null)
                {
                    _writeStream.Flush();
                    _writeStream.Dispose();
                    _writeStream = null;
                    GC.Collect();
                }

                _metrics.FileBuffer.IsWriteActive = false;
                _metrics.FileBuffer.WritePageSequence = 0;
                _metrics.LastUpdated = UnixDateTime.Now;

                return true;
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                _logger.Log(TrakHoundLogLevel.Error, $"Buffer {Id} : FlushBuffer() : Exception : {ex.Message}");
            }

            return false;
        }

        private void WriteCloseDelayElapsed(object sender, EventArgs e)
        {
            if (!CloseWriteStream())
            {
                _writeCloseDelay.Set(); // Reset if Error Occurred
            }
        }
    }
}
