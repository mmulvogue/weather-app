namespace MM.WeatherService.Api.RateLimiter.AsyncLock;

/// <summary>
///     Wraps SemaphoreSlim to provide an IDisposable async compatible lock
/// </summary>
public class AsyncLock : IDisposable
{
    private readonly IDisposable _releaser;
    private readonly SemaphoreSlim _semaphoreSlim;

    internal AsyncLock()
    {
        _semaphoreSlim = new SemaphoreSlim(1);
        _releaser = new Releaser(this);
    }

    /// <summary>
    ///     Action that will be invoked when Release is called
    /// </summary>
    public Action? OnRelease { get; set; }

    /// <summary>
    ///     Disposes the underlying SemaphoreSlim
    /// </summary>
    public void Dispose()
    {
        _semaphoreSlim.Dispose();
    }

    /// <summary>
    ///     Returns a Task that awaits access to semaphore and then returns IDisposable that can be used to release semaphore
    /// </summary>
    /// <returns></returns>
    public Task<IDisposable> LockAsync()
    {
        var wait = _semaphoreSlim.WaitAsync();
        return AwaitThenReturn(wait, _releaser);

        static async Task<IDisposable> AwaitThenReturn(Task t, IDisposable r)
        {
            await t;
            return r;
        }
    }

    /// <summary>
    ///     Releases the underlying SemaphoreSlim and calls the OnRelease action
    /// </summary>
    public void Release()
    {
        _semaphoreSlim.Release();
        OnRelease?.Invoke();
    }

    private sealed class Releaser : IDisposable
    {
        private readonly AsyncLock toRelease;

        internal Releaser(AsyncLock toRelease)
        {
            this.toRelease = toRelease;
        }

        public void Dispose()
        {
            toRelease?.Release();
        }
    }
}
