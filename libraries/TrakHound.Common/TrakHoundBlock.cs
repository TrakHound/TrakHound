// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound
{
    public class TrakHoundBlock
    {
        public string Id { get; set; }

        public string Key { get; set; }

        public string Label { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public TimeSpan Duration
        {
            get
            {
                return End - Start;
            }
        }


        public bool IsBetween(DateTime from, DateTime to)
        {
            if (to > from && End > Start)
            {
                if (Start >= from && Start < to) return true;
                if (End >= from && End < to) return true;
                if (Start <= from && End >= to) return true;
            }

            return false;
        }


        public static IEnumerable<TrakHoundBlock> Generate<T>(
            IEnumerable<T> items,
            Func<T, string> keySelector,
            Func<T, DateTime> timestampSelector,
            DateTime from,
            DateTime to,
            DateTime now,
            TimeSpan? minimumDuration = null,
            Func<T, string> idSelector = null,
            Func<T, string> labelSelector = null
            )
        {
            if (!items.IsNullOrEmpty() && now > from && from < to)
            {
                var blocks = new List<TrakHoundBlock>();

                var filtered = new List<T>();
                var fItems = items.Where(o => timestampSelector(o) >= from && timestampSelector(o) <= to);
                if (!fItems.IsNullOrEmpty()) filtered.AddRange(fItems);
                if (!filtered.Any(o => timestampSelector(o) == from))
                {
                    var lastFound = items.Where(o => timestampSelector(o) < from);
                    if (!lastFound.IsNullOrEmpty()) filtered.Add(lastFound.OrderByDescending(o => timestampSelector(o)).FirstOrDefault());
                }

                if (!filtered.IsNullOrEmpty())
                {
                    var ordered = filtered.OrderBy(o => timestampSelector(o)).ToList();
                    var prev = ordered[0];
                    var max = ordered.Count - 1;

                    if (ordered.Count > 1) // Multiple Item Found
                    {
                        for (var i = 1; i <= max; i++)
                        {
                            var current = ordered[i];
                            var start = timestampSelector(prev) > from ? timestampSelector(prev) : from;
                            var stop = timestampSelector(current) < to ? timestampSelector(current) : to;
                            if (stop > now) stop = now;


                            var block = new TrakHoundBlock();
                            block.Key = keySelector(prev);
                            if (idSelector != null) block.Id = idSelector(prev);
                            if (labelSelector != null) block.Label = labelSelector(prev);
                            block.Start = start;
                            block.End = stop;
                            blocks.Add(block);

                            prev = current;
                        }

                        var lastStart = timestampSelector(prev) > from ? timestampSelector(prev) : from;
                        var lastStop = now < to ? now : to;

                        var lastBlock = new TrakHoundBlock();
                        lastBlock.Key = keySelector(prev);
                        if (idSelector != null) lastBlock.Id = idSelector(prev);
                        if (labelSelector != null) lastBlock.Label = labelSelector(prev);
                        lastBlock.Start = lastStart;
                        lastBlock.End = lastStop;
                        blocks.Add(lastBlock);
                    }
                    else // Only one Item Found
                    {
                        var onlyStart = timestampSelector(prev) > from ? timestampSelector(prev) : from;
                        var onlyStop = now < to ? now : to;

                        var onlyObj = new TrakHoundBlock();
                        onlyObj.Key = keySelector(prev);
                        if (idSelector != null) onlyObj.Id = idSelector(prev);
                        if (labelSelector != null) onlyObj.Label = labelSelector(prev);
                        onlyObj.Start = onlyStart;
                        onlyObj.End = onlyStop;
                        blocks.Add(onlyObj);
                    }
                }

                // Filter out blocks that don't meet the Minimum Duration
                var filteredBlocks = new List<TrakHoundBlock>();
                if (minimumDuration != null)
                {
                    foreach (var block in blocks)
                    {
                        if (block.Duration >= minimumDuration)
                        {
                            filteredBlocks.Add(block);
                        }
                    }
                }
                else
                {
                    filteredBlocks = blocks;
                }

                // Process Blocks to join empty spaces
                var processedBlocks = new List<TrakHoundBlock>();
                TrakHoundBlock prevBlock = null;
                TrakHoundBlock currentBlock = null;
                for (var i = 0; i < filteredBlocks.Count; i++)
                {
                    currentBlock = filteredBlocks[i];
                    if (prevBlock != null)
                    {
                        if (currentBlock.Key == prevBlock.Key)
                        {
                            // This "removes" adjacent blocks with the same key
                            prevBlock.End = currentBlock.End;
                        }
                        else
                        {
                            // Fill any gaps left from filtering
                            prevBlock.End = currentBlock.Start;
                            processedBlocks.Add(prevBlock);
                            prevBlock = currentBlock;
                        }
                    }
                    else
                    {
                        prevBlock = currentBlock;
                    }
                }

                // Add Last Block
                if (prevBlock != null) processedBlocks.Add(prevBlock);

                return processedBlocks;
            }

            return null;
        }

        public static IEnumerable<TrakHoundBlock> Generate<T>(
            IEnumerable<T> items,
            Func<T, string> keySelector,
            Func<T, DateTime> startTimestampSelector,
            Func<T, DateTime> endTimestampSelector,
            DateTime from,
            DateTime to,
            DateTime now,
            Func<T, string> labelSelector = null
            )
        {
            if (!items.IsNullOrEmpty() && from < to)
            {
                var blocks = new List<TrakHoundBlock>();

                var filtered = new List<T>();
                var fItems = items.Where(o => startTimestampSelector(o) >= from && endTimestampSelector(o) <= DateTime.MinValue || startTimestampSelector(o) >= from && startTimestampSelector(o) < to || startTimestampSelector(o) <= from && endTimestampSelector(o) >= to || startTimestampSelector(o) >= from && endTimestampSelector(o) <= to || startTimestampSelector(o) <= from && endTimestampSelector(o) > from && endTimestampSelector(o) < to);
                if (!fItems.IsNullOrEmpty()) filtered.AddRange(fItems);

                if (!filtered.IsNullOrEmpty())
                {
                    foreach (var item in filtered)
                    {
                        var start = startTimestampSelector(item) > from ? startTimestampSelector(item) : from;
                        var stop = endTimestampSelector(item) < to && endTimestampSelector(item) > DateTime.MinValue ? endTimestampSelector(item) : to;

                        var block = new TrakHoundBlock();
                        block.Key = keySelector(item);
                        if (labelSelector != null) block.Label = labelSelector(item);
                        block.Start = start;
                        block.End = stop > now ? now : stop;
                        blocks.Add(block);
                    }
                }

                return blocks;
            }

            return null;
        }
    }
}
