using System.Collections.Generic;

namespace XmlSchemaValidator
{
    internal partial struct ValidationVisitor
    {
        internal struct DescendantList<T>
        {
            private static readonly List<T> _default = new List<T>(0);
            private List<T> _list;

            public void Add(T item)
            {
                if (_list == null)
                {
                    _list = new List<T>();
                }

                _list.Add(item);
            }

            public List<T>.Enumerator GetEnumerator() => _list == null ? _default.GetEnumerator() : _list.GetEnumerator();
        }
    }
}
