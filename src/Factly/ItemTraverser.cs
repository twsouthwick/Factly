// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

#if !NO_CANCELLATION_TOKEN
using System.Threading;
#endif

#if FEATURE_PARALLEL_VALIDATION
using System.Collections.Concurrent;
using System.Threading.Tasks;
#endif

namespace Factly
{
    internal static class ItemTraverser
    {
        public static void Traverse<TItem>(this IEnumerable<TItem> initialItems, Func<TItem, IEnumerable<TItem>> process, CancellationToken token)
        {
            var visited = new HashSet<TItem>();
            var items = new Queue<TItem>(initialItems);

            while (items.Count > 0)
            {
                var item = items.Dequeue();

                token.ThrowIfCancellationRequested();

                if (visited.Add(item))
                {
                    foreach (var additional in process(item))
                    {
                        items.Enqueue(additional);
                    }
                }
            }
        }

#if FEATURE_PARALLEL_VALIDATION
        public static async Task TraverseAsync<TItem>(this IEnumerable<TItem> initialItems, Func<TItem, IEnumerable<TItem>> process, int numThreads, CancellationToken token)
        {
            var visited = new ConcurrentHashSet<TItem>();
            var items = new ConcurrentQueue<TItem>(initialItems);
            var resetEvent = new AsyncAutoResetEvent();
            var tasks = new TaskTracker(resetEvent.Set);

            while (true)
            {
                while (tasks.Count < numThreads && !items.IsEmpty)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        while (!items.IsEmpty)
                        {
                            token.ThrowIfCancellationRequested();

                            if (items.TryDequeue(out var item))
                            {
                                if (visited.Add(item))
                                {
                                    foreach (var additional in process(item))
                                    {
                                        items.Enqueue(additional);
                                    }
                                }
                            }
                        }
                    }), token);
                }

                if (tasks.Count == 0 && items.IsEmpty)
                {
                    return;
                }

                await resetEvent.WaitAsync(token).ConfigureAwait(false);
            }
        }
#endif
    }
}
