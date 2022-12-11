using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace MM.WeatherService.Api.RateLimiter
{
    public class AsyncKeyedLock
    {
        private readonly ConcurrentDictionary<string, AsyncKeyedLockReleaser> _lockDictionary = new();

        public async ValueTask<IDisposable> LockAsync(string key, int millisecondsTimeout)
        {
            var releaser = GetLockReleaser(key);
            await releaser.LockAwaiter();
            return releaser;
        }

        private AsyncKeyedLockReleaser GetLockReleaser(string key)
        {
            if (_lockDictionary.TryGetValue(key, out var releaser) && TryIncrementReleaserReference(releaser))
            {
                return releaser;
            }

            var toAdd = new AsyncKeyedLockReleaser(key, this);
            if (_lockDictionary.TryAdd(key, toAdd))
            {
                return toAdd;
            }

            while (!(_lockDictionary.TryGetValue(key, out releaser) && TryIncrementReleaserReference(releaser)))
            {
                if (_lockDictionary.TryAdd(key, toAdd))
                {
                    return toAdd;
                }
            }

            return releaser;
        }

        private bool TryIncrementReleaserReference(AsyncKeyedLockReleaser releaser)
        {
            if (Monitor.TryEnter(this))
            {
                ++releaser.ReferenceCount;
                Monitor.Exit(this);
                return true;
            }

            return false;
        }

        // Allow next in queue to use the lock
        protected void ReturnLockReleaser(AsyncKeyedLockReleaser releaser)
        {
            Monitor.Enter(releaser);

            if (--releaser.ReferenceCount == 0)
            {
                // If no more waiters for lock then remove releaser
                _lockDictionary.TryRemove(releaser.Key, out _);
            }

            Monitor.Exit(releaser);
            releaser.ReleaseLock();
        }

        public class AsyncKeyedLockReleaser : IDisposable
        {
            private readonly SemaphoreSlim _semaphoreSlim;
            private int _referenceCount = 1;
            private readonly AsyncKeyedLock _lockManager;

            internal AsyncKeyedLockReleaser(string key, AsyncKeyedLock lockManager)
            {
                Key = key;
                _lockManager = lockManager;
                _semaphoreSlim = new SemaphoreSlim(1);
            }

            public ConfiguredTaskAwaitable LockAwaiter()
            {
                return _semaphoreSlim.WaitAsync().ConfigureAwait(false);
            }

            public void ReleaseLock()
            {
                _semaphoreSlim.Release();
            }

            public string Key { get; }

            public int ReferenceCount
            {
                get => _referenceCount;
                internal set => _referenceCount = value;
            }

            public void Dispose()
            {
                _lockManager.ReturnLockReleaser(this);
            }
        }
    }
    
}
