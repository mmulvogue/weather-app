using System.Collections.Concurrent;

namespace MM.WeatherService.Api.RateLimiter.AsyncLock;

/// <summary>
///     Manages locks that are specific to a provided key.
///     Allows lock requests using different keys to execute concurrently
/// </summary>
public class AsyncKeyedLock
{
    private readonly ConcurrentDictionary<string, ActiveLock> _activeLocks = new();

    /// <summary>
    ///     Acquires a key based lock that will be released when the returned object is disposed
    /// </summary>
    /// <param name="key">the key to acquire lock against</param>
    /// <returns></returns>
    public Task<IDisposable> LockAsync(string key)
    {
        return GetLock(key).LockAsync();
    }

    private AsyncLock GetLock(string key)
    {
        // if there is an existing lock in-place for the key return that and increment reference count
        if (_activeLocks.TryGetValue(key, out var activeLock) && TryIncrementLockReferenceCount(activeLock))
            return activeLock.Lock;

        // Add a new lock object if it doesn't exist already for the current key
        // Uses a retry logic to allow for concurrency
        var toAdd = new ActiveLock
        {
            Lock = new AsyncLock
            {
                OnRelease = () => OnLockRelease(key)
            },
            ReferenceCount = 1
        };

        if (_activeLocks.TryAdd(key, toAdd)) return toAdd.Lock;

        while (!(_activeLocks.TryGetValue(key, out activeLock) && TryIncrementLockReferenceCount(activeLock)))
            if (_activeLocks.TryAdd(key, toAdd))
                return toAdd.Lock;

        return activeLock.Lock;
    }

    /// <summary>
    ///     Thread-safe incrementer for active lock reference count
    /// </summary>
    /// <param name="activeLock"></param>
    /// <returns></returns>
    private bool TryIncrementLockReferenceCount(ActiveLock activeLock)
    {
        if (Monitor.TryEnter(this))
        {
            ++activeLock.ReferenceCount;
            Monitor.Exit(this);
            return true;
        }

        return false;
    }

    // Allow next in queue to use the lock
    protected void OnLockRelease(string key)
    {
        if (_activeLocks.TryGetValue(key, out var activeLock))
        {
            Monitor.Enter(activeLock);
            if (--activeLock.ReferenceCount == 0)
            {
                // If lock is no longer active remove and dispose it
                _activeLocks.TryRemove(key, out _);
                Monitor.Exit(activeLock);
                activeLock.Lock.Dispose();
                return;
            }

            Monitor.Exit(activeLock);
        }
    }

    internal class ActiveLock
    {
        public int ReferenceCount { get; set; }
        public AsyncLock Lock { get; set; }
    }
}
