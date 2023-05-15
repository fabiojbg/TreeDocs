using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Sdk.Helpers
{
    public static class AsyncHelper
    {
        public static T RunSync<T>(Func<Task<T>> action)
        {
            Task<T> task = action();

            task.Wait();

            if (task.IsFaulted)
                throw task.Exception;

            if (task.IsCanceled)
                throw new OperationCanceledException();

            return task.Result;
        }

        public static void RunSync(Func<Task> action)
        {
            Task task = action();

            task.Wait();

            if (task.IsFaulted)
                throw task.Exception;

            if (task.IsCanceled)
                throw new OperationCanceledException();
        }

    }
}
