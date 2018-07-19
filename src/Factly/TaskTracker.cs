// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Factly
{
    internal class TaskTracker
    {
        private readonly ConcurrentHashSet<Task> _tasks = new ConcurrentHashSet<Task>();
        private readonly Action _action;

        public TaskTracker(Action action)
        {
            _action = action;
        }

        public int Count => _tasks.Count;

        public void Add(Task task, CancellationToken token)
        {
            AddAsync(task, token);
        }

        public Task AddAsync(Task task, CancellationToken token)
        {
            var continuation = task.ContinueWith(
                Continuation,
                token,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Current);

            _tasks.Add(task);

            return continuation;
        }

        private void Continuation(Task t)
        {
            _tasks.Remove(t);
            _action();
        }
    }
}
