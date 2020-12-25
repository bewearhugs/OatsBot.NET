﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Collections.Concurrent
{
    [DebuggerDisplay("Count={" + nameof(Count) + "}")]
    public class ConcurrentPool<T>
    {
        private readonly object _syncLock = new();
        private readonly List<T> _list = new();

        public void Add(T item)
        {
            lock (_syncLock)
                _list.Add(item);
        }

        public void Remove(T item)
        {
            lock (_syncLock)
                _list.Remove(item);
        }

        public int Count
        {
            get
            {
                lock (_syncLock)
                    return _list.Count;
            }
        }

        public bool All(Func<T, bool> condition)
        {
            lock (_syncLock)
                return _list.All(condition);
        }

        public bool Tag(Predicate<T> condition, Action<T> action, out T result)
        {
            lock (_syncLock)
            {
                result = _list.Find(condition);
                if (result == null)
                    return false;

                action(result);
                return true;
            }
        }

        public T[] ToArray()
        {
            lock (_syncLock)
                return _list.ToArray();
        }

        public void Clear()
        {
            lock (_syncLock)
                _list.Clear();
        }
    }
}