Synchronization.NET
----

A synchronization library for .NET Frameworks 2.0+ and .NET Core/ASP.NET Core.

Synchronization.NET is developed and maintained by Mahmoud Al-Qudsi of NeoSmart Technologies, and is released under the MIT open source license. Synchronization.NET [is available on NuGet](http://nuget.org/packages/synchronization.net).

### Key Classes

**NeoSmart.Synchronization.CountdownEvent**

A `CountdownEvent` object is initialized to a positive integer value `n`, and triggers when `CountdownEvent.Tick()` has been called `n` times. Useful for waiting on `n` objects to complete a task or to carry out an event every `n` times.

**NeoSmart.Synchronization.ScopedMutex**

A `ScopedMutex` instance is to be used in lieu of a named lock (which does not exist) for cross-process locking of a sensitive/critical region via a simple `using` block. A `ScopedMutex` internally handles `AbandonedMutexException`, making it safe to use as-is without a try-catch-finally block.