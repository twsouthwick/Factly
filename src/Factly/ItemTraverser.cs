// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

#if !NO_CANCELLATION_TOKEN
using System.Threading;
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
    }
}
