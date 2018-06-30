using System;
using System.Collections.Generic;

namespace XmlSchemaValidator
{
    internal class ListObserver<T> : IObserver<T>
    {
        private readonly List<T> _list = new List<T>();

        public IReadOnlyCollection<T> Items => _list;

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value) => _list.Add(value);
    }
}
