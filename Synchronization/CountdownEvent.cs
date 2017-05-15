using System;
using System.Threading;
#if !NET20
using System.Diagnostics.Contracts;
#endif

namespace NeoSmart.Synchronization
{
    /// <summary>
    /// An event that fires when a pre-determined number of events (ticks) have completed/occurred. Remains set thereafter unless manually reset.
    /// </summary>
    public class CountdownEvent : EventWaitHandle
    {
        private int _counter;
        public delegate void EventTriggeredDelegate();
        private EventTriggeredDelegate _callback;

        public CountdownEvent(int start, EventTriggeredDelegate callback = null)
            : base(false, EventResetMode.ManualReset)
        {
            if (start <= 0)
            {
                throw new ArgumentException("Countdown must start from a positive integer!");
            }
#if !NET20
            Contract.EndContractBlock();
#endif
            _callback = callback;
            Reset(start);
        }

        public void Tick()
        {
            var result = Interlocked.Decrement(ref _counter);
            if (result == 0)
            {
                Set();
                _callback?.Invoke();
            }
            else if (result < 0)
            {
                throw new InvalidOperationException("CountdownEvent has been decremented more times than allowed!");
            }
        }

        public void Reset(int newStart)
        {
            if (newStart <= 0)
            {
                throw new ArgumentException("Countdown must start from a positive integer!");
            }
#if !NET20
            Contract.EndContractBlock();
#endif

            base.Reset();
            Interlocked.Exchange(ref _counter, newStart);
        }

        public new void Reset()
        {
            throw new InvalidOperationException("This method cannot be called directly for a CountdownEvent! A countdown start parameter must be provided.");
#if !NET20
            Contract.EndContractBlock();
#endif
        }
    }
}
