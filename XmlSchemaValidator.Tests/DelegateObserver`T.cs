using System;

namespace XmlSchemaValidator
{
    internal class DelegateObserver<T> : IObserver<T>
    {
        private readonly Action<T> _action;

        public DelegateObserver(Action<T> action)
        {
            _action = action;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value) => _action(value);
    }
}
