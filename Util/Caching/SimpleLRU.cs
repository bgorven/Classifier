using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Collections;

namespace Utilities.Caching
{
    public class SimpleLRU<TKey, TValue> where TValue : class
    {
        private readonly int _targetCapacity;
        private int _keepAliveCount = 0;
        private readonly ConcurrentDictionary<Lazy<TValue>, object> _keepAlive;
        private ICollection<Lazy<TValue>> _keepPrev;
        private readonly ConcurrentDictionary<TKey, WeakReference<Lazy<TValue>>> _cache = new ConcurrentDictionary<TKey, WeakReference<Lazy<TValue>>>();

        /// <summary>
        /// Simplest possible concurrent LRU cache. Uses a <see cref="ConcurrentDictionary{T}"/> of weak references to 
        /// <see cref="Lazy{T}"/>, plus a <see cref="ConcurrentDictionary{T}"/> of strong references with approximately
        /// <code>targetCapacity</code> number of entries. (No guarantees about the precise capacity of the cache)
        /// </summary>
        public SimpleLRU(int targetCapacity)
        {
            _targetCapacity = targetCapacity;
            _keepAlive = new ConcurrentDictionary<Lazy<TValue>, object>();
        }

        public TValue GetOrAdd(TKey key, Func<TValue> valueFactory)
        {
            var lazy = new Lazy<TValue>(valueFactory);
            var wref = _cache.GetOrAdd(key, new WeakReference<Lazy<TValue>>(lazy));
            
            while (!wref.TryGetTarget(out lazy))
            {
                lazy = new Lazy<TValue>(valueFactory);
                _cache.TryUpdate(key, new WeakReference<Lazy<TValue>>(lazy), wref);
                _cache.TryGetValue(key, out wref);
            }
                
            KeepAlive(lazy);
            return lazy.Value;
        }

        private void KeepAlive(Lazy<TValue> value)
        {
            if (_keepAlive.TryAdd(value, null)) Interlocked.Increment(ref _keepAliveCount);
        }

        private readonly object _CheckCapacity = new object();
        private void CheckCapacity()
        {
            if (Monitor.TryEnter(_CheckCapacity) && _keepAliveCount >= _targetCapacity)
            {
                try
                {
                    _keepPrev = _keepAlive.Keys;    //No guarantees about the ACTUAL
                    _keepAliveCount = 0;            //number of items cached.
                    foreach (var k in _keepPrev)
                    {
                        _keepAlive.TryRemove(k, out var ignored);
                    }
                }
                finally
                {
                    Monitor.Exit(_CheckCapacity);
                }
            }
        }
    }
}
