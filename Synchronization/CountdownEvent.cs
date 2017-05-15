#if !NET40 && !NET45
using System;
using System.Threading;

namespace NeoSmart.Synchronization
{
    /// <summary>
    /// An event that fires when a pre-determined number of events (ticks) have completed/occurred. Remains set thereafter unless manually reset.
    /// </summary>
    public class CountdownEvent : EventWaitHandle
    {
        private int _counter;
        private int _start;
        public delegate void EventTriggeredDelegate();
        public event EventTriggeredDelegate CountdownCompleted;

        public int CurrentCount => _counter; //no need for interlocked, guaranteed atomic reads
        public WaitHandle WaitHandle => this;
        public int InitialCount => _start;
        public bool IsSet
        {
            get
            {
                return WaitOne(0);
            }
        }

        public CountdownEvent(int start)
            : base(false, EventResetMode.ManualReset)
        {
            if (start <= 0)
            {
                throw new ArgumentException("Countdown must start from a positive integer!");
            }

            _start = start;
            Reset(start);
        }

        public void Tick()
        {
            var result = Interlocked.Decrement(ref _counter);
            if (result == 0)
            {
                Set();
                CountdownCompleted?.Invoke();
            }
            else if (result < 0)
            {
                throw new InvalidOperationException("CountdownEvent has been decremented more times than allowed!");
            }
        }

        //for compatibility with the new System.Threading.CountdownEvent
        public void Signal(int amount = 1)
        {
            for (int i = 0; i < amount; ++i)
            {
                Tick();
            }
        }

        public void Reset(int newStart)
        {
            if (newStart <= 0)
            {
                throw new ArgumentException("Countdown must start from a positive integer!");
            }

            base.Reset();
            Interlocked.Exchange(ref _counter, newStart);
        }

        public new void Reset()
        {
            Reset(_start);
        }

        public bool Wait()
        {
            return WaitOne();
        }

        public bool Wait(TimeSpan time)
        {
            return WaitOne(time);
        }

        public bool Wait(int milliseconds)
        {
            return WaitOne(milliseconds);
        }

        public void AddCount(int amount = 1)
        {
            while (true)
            {
                int oldValue = _counter;
                if (oldValue == 0)
                {
                    throw new InvalidOperationException("Cannot increment completed CountdownEvent instance!");
                }
                if (Interlocked.CompareExchange(ref _counter, oldValue + amount, oldValue) != oldValue)
                {
                    continue;
                }
                break;
            }
        }

        public bool TryAddCount(int amount = 1)
        {
            if (amount == 0)
            {
                return false;
            }
            AddCount(amount);
            return true;
        }

#if !NET20
        public bool Wait(CancellationToken token)
        {
            return WaitAny(new[] { token.WaitHandle, this }) == 1;
        }

        public bool Wait(int milliseconds, CancellationToken token)
        {
            return WaitAny(new[] { token.WaitHandle, this }, milliseconds) == 1;
        }
#endif
    }
}
#endif