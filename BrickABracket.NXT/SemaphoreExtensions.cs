using System;
using System.Threading;
using System.Threading.Tasks;

namespace BrickABracket.NXT
{
    public static class SemaphoreExtensions
    {
        public static SemaphoreLock Lock(this SemaphoreSlim semaphore)
        {
            semaphore.Wait();
            return new SemaphoreLock(semaphore);
        }
        public static async Task<SemaphoreLock> LockAsync(this SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            return new SemaphoreLock(semaphore);
        }
    }

    public class SemaphoreLock : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        public SemaphoreLock(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public void Dispose()
        {
            _semaphore?.Release();
        }
    }
}