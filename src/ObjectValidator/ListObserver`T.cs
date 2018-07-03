using System;
using System.Collections.Generic;

namespace ObjectValidator
{
    internal class ListObserver<T> : IObserver<T>
    {
        public List<T> Items { get; } = new List<T>();

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value) => Items.Add(value);
    }
}
