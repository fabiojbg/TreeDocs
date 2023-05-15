using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Apps.Sdk.Helpers
{
    public class NamedSemaphore : IDisposable
    {
        string _semaphoreName;
        int _semaphoreCount;
        Semaphore _semaphore;
        static object _semaphorLock = new object();

        public NamedSemaphore(string name, int semaphoreCount = 1)
        {
            _semaphoreName = name;
            _semaphoreCount = semaphoreCount;
        }

        public void Wait()
        {
            if (!OperatingSystem.IsWindows)
                throw new InvalidOperationException("This operation is valid only on windows OS");

            lock (_semaphorLock)
            {
                try
                {
#pragma warning disable CA1416 // Validate platform compatibility
                    _semaphore = Semaphore.OpenExisting(_semaphoreName);
#pragma warning restore CA1416 // Validate platform compatibility

                    if (Debugger.IsAttached) Trace.TraceWarning(DateTime.Now + ": Semaphore opened: " + _semaphoreName);
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    _semaphore = new Semaphore(1, _semaphoreCount, _semaphoreName);

                    if (Debugger.IsAttached) Trace.TraceWarning(DateTime.Now + ": Semaphore create: " + _semaphoreName);
                }
            }
            if (Debugger.IsAttached) Trace.TraceWarning(DateTime.Now + ": Waiting Semaphore: " + _semaphoreName);
            _semaphore.WaitOne();
            if (Debugger.IsAttached) Trace.TraceWarning(DateTime.Now + ": Semaphore Passed: " + _semaphoreName);
        }

        public void Release()
        {
            lock (_semaphorLock)
            {
                _semaphore.Release();
            }
            if (Debugger.IsAttached) Trace.TraceWarning(DateTime.Now + ": Semaphore released: " + _semaphoreName);
        }

        public void Dispose()
        {
            Release();
            _semaphore.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
