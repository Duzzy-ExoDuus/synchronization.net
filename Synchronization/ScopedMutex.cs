using System;
using System.Threading;

namespace NeoSmart.Synchronization
{
    /// <summary>
    /// A scoped, named mutex for use in using blocks in lieu of a named lock, which does not exist. Safely handles mutex exceptions and releases on Dispose()
    /// </summary>
    public class ScopedMutex : IDisposable
    {
        private readonly Mutex _mutex;
        private bool _locked;
        public bool SafeWait { get; set; }

        /// <summary>
        /// Creates or opens a mutex identified by <paramref name="name"/>. Unless <paramref name="obtain"/> is explicitly set to false, blocks until the mutex is acquired.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obtain"></param>
        public ScopedMutex(string name, bool obtain = true)
        {
            _mutex = new Mutex(false, name);
            _locked = false;
            SafeWait = true;

            if (obtain)
            {
                WaitOne();
            }
        }

        public bool WaitOne()
        {
            try
            {
                _mutex.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                if (!SafeWait)
                {
                    throw;
                }
            }
            finally
            {
                _locked = true; //Regardless of AbandonedMutexException
            }

            return true;
        }

        /// <summary>
        /// Manually releases the resources, unblocking the mutex to be obtained by another thread. Use with caution, suggestion is to not call directly and only use ScopedMutex in a using block.
        /// </summary>
        public void Release()
        {
            _mutex.ReleaseMutex();
            _locked = false;
        }

        public void Dispose()
        {
            if (_locked)
            {
                Release();
            }
#if !NET20
            _mutex.Dispose();
#endif
        }
    }
}
