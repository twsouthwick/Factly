// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Factly
{
    internal class AsyncAutoResetEvent
    {
        private readonly Queue<TaskCompletionSource<bool>> _waits = new Queue<TaskCompletionSource<bool>>();
        private bool _signaled;

        public Task WaitAsync(CancellationToken token)
        {
            lock (_waits)
            {
                if (_signaled)
                {
                    _signaled = false;
                    return Task.CompletedTask;
                }
                else
                {
                    var tcs = GetCompletionSource(token);
                    _waits.Enqueue(tcs);
                    return tcs.Task;
                }
            }
        }

        public void Set()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (_waits)
            {
                if (_waits.Count > 0)
                {
                    toRelease = _waits.Dequeue();
                }
                else if (!_signaled)
                {
                    _signaled = true;
                }
            }

            if (toRelease != null)
            {
                toRelease.SetResult(true);
            }
        }

        private static TaskCompletionSource<bool> GetCompletionSource(CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>();

            if (!token.CanBeCanceled)
            {
                return tcs;
            }

            var registration = token.Register(
                () =>
                {
                    tcs.TrySetCanceled(token);
                }, useSynchronizationContext: false);

            tcs.Task.ContinueWith(_ => registration.Dispose(), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            return tcs;
        }
    }
}
