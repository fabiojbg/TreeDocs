using Apps.Sdk.DependencyInjection;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Apps.Sdk.Helpers
{
    public class SdkBackgroundWorker : System.ComponentModel.BackgroundWorker
    {
        bool _isRunning;
        public bool IsRunning => _isRunning;

        public event EventHandler OnFinishedWork;

        public SdkBackgroundWorker() : base()
        {
            this.DoWork += (sender, args) => 
            {
                _isRunning = true;
            };
            this.Disposed += (sender, args) => 
            {
                _isRunning = false;
                OnFinishedWork?.Invoke(sender, args);
            };
        }
    }
    
    public static class BackgroundWorker
    {
        public delegate void Worker();

        public delegate void WorkerWithContainerScope(ISdkContainer scope);
        public delegate Task AsyncWorkerWithContainerScope(ISdkContainer scope);
        public delegate Task<bool> AsyncResultWorkerWithContainerScope(ISdkContainer scope);
        public delegate Task<bool> AsyncResultWorker();

        public delegate Result WorkerWithResult();


        public static SdkBackgroundWorker RunWorkAsync(AsyncWorkerWithContainerScope asyncWork)
        {
            return RunAsyncWorkWithContainer("", SdkDI.Resolver, asyncWork);
        }

        public static SdkBackgroundWorker RunWork(WorkerWithContainerScope work)
        {
            return RunWorkWithContainer("", SdkDI.Resolver, work);
        }

        /// <summary>
        /// Permite a execução de uma tarefa em background com um scopo de container próprio liberado somente ao final da tarefa
        /// </summary>
        /// <param name="taskName">Nome da tarefa</param>
        /// <param name="resolver">Container que terá um scope disponibilizado para a tarefa</param>
        /// <param name="work">work to be done</param>
        /// <returns></returns>
        public static SdkBackgroundWorker RunWorkWithContainer(string taskName, ISdkContainer resolver, WorkerWithContainerScope work)
        {
            var worker = new SdkBackgroundWorker();

            worker.DoWork += (sender, args) =>
            {
                try
                {
                    var diContainer = args.Argument as ISdkContainer;
                    using( var scope = diContainer.GetChildContainer())
                    {
                        work(scope);
                    }
                }
                finally
                {
                    worker.Dispose();
                }
            };

            worker.RunWorkerAsync(resolver);

            return worker;
        }

        /// <summary>
        /// Permite a execução de uma tarefa em background com um scopo de container próprio liberado somente ao final da tarefa
        /// </summary>
        /// <param name="taskName">Nome da tarefa</param>
        /// <param name="resolver">Container que terá um scope disponibilizado para a tarefa</param>
        /// <param name="work">work to be done</param>
        /// <returns></returns>
        public static SdkBackgroundWorker RunAsyncWorkWithContainer(string taskName, ISdkContainer resolver, AsyncWorkerWithContainerScope asyncWork)
        {
            var worker = new SdkBackgroundWorker();

            worker.DoWork += async (sender, args) =>
            {
                var diContainer = args.Argument as ISdkContainer;
                try
                {
                    using (var scope = diContainer.GetChildContainer())
                    {
                        await asyncWork(scope);
                    }
                }
                finally
                {
                    worker.Dispose();
                }
            };

            worker.RunWorkerAsync(resolver);

            return worker;
        }

        public static SdkBackgroundWorker RunAsyncWorkWithContainerAndRetries(string taskName,
                                                                                ISdkContainer resolver,
                                                                                AsyncResultWorkerWithContainerScope asyncWork,
                                                                                int waitTime = 60000,
                                                                                int retries = 0,
                                                                                int waitToStart = 0
                                                                                )
        {
            var worker = new SdkBackgroundWorker();

            waitTime = Math.Min(3000, waitTime);
            worker.DoWork += async (sender, args) =>
            {
                await Task.Delay(waitToStart); 
                var diContainer = args.Argument as ISdkContainer;
                try
                {
                    using (var scope = diContainer.GetChildContainer())
                    {
                        var executionSuccess = false;
                        do
                        {
                            executionSuccess = await asyncWork(scope);
                            if( !executionSuccess )
                                await Task.Delay(waitTime);

                            retries -= 1;
                        } while (!executionSuccess && retries>=0);
                    }
                }
                finally
                {
                    worker.Dispose();
                }
            };

            worker.RunWorkerAsync(resolver);

            return worker;
        }


        public static System.ComponentModel.BackgroundWorker Run(Worker work)
        {
            var worker = new System.ComponentModel.BackgroundWorker();

            worker.DoWork += (sender, args) =>
            {
                work();
            };

            worker.RunWorkerAsync();

            return worker;
        }

        public static System.ComponentModel.BackgroundWorker TryRun(string taskName, WorkerWithResult work, int retries = 15, int waitTime = 1000, int waitToStart = 0)
        {
            var count = 0;
            var worker = new System.ComponentModel.BackgroundWorker();

            worker.DoWork += (sender, args) =>
            {
                if (waitToStart > 0)
                    Thread.Sleep(waitToStart);

                var completed = false;
                while (count <= retries && !completed)
                {
                    try
                    {
                        completed = work().Success;
                        if (completed) break;
                    }
                    catch
                    {
//                        logger?.LogError(ex, $"Error on {taskName}");
                    }

                    count++;
                    Thread.Sleep(waitTime);
                }
                worker.Dispose();
            };

            worker.RunWorkerAsync();

            return worker;
        }

        public static System.ComponentModel.BackgroundWorker TryRunUntilSucceeds(string taskName, 
                                                           WorkerWithResult work, 
                                                           int waitTime = 60000, 
                                                           int waitToStart = 0)
        {
            var count = 0;
            var worker = new System.ComponentModel.BackgroundWorker();

            worker.DoWork += (sender, args) =>
            {                
                if (waitToStart > 0)
                    Thread.Sleep(waitToStart);

                var completed = false;
                while (!completed)
                {
                    try
                    {
                        completed = work().Success;
                        if (completed) break;
                    }
                    catch
                    {
  //                      logger?.LogError(ex, $"Error on {taskName}");
                    }

                    count++;
                    Thread.Sleep(waitTime);
                }
                worker.Dispose();
            };

            worker.RunWorkerAsync();

            return worker;
        }

        public static SdkBackgroundWorker RunAsyncWorkWithRetries(string taskName,
                                                                  AsyncResultWorker asyncWork,
                                                                    int waitTime = 60000,
                                                                    int retries = 0,
                                                                    int waitToStart = 0
                                                                    )
        {
            var worker = new SdkBackgroundWorker();

            waitTime = Math.Min(3000, waitTime);
            worker.DoWork += async (sender, args) =>
            {
                await Task.Delay(waitToStart);
                var diContainer = args.Argument as ISdkContainer;
                try
                {
                    var executionSuccess = false;
                    do
                    {
                        try
                        {
                            executionSuccess = await asyncWork();
                            if (!executionSuccess)
                                await Task.Delay(waitTime);
                        }
                        catch
                        {
                            executionSuccess = false;
                            //if (taskName != null)
                            //    logger?.LogError(ex, $"Error on task {taskName}");

                            await Task.Delay(waitTime);
                        }
                        retries -= 1;
                    } while (!executionSuccess && retries >= 0);
                }
                finally
                {
                    worker.Dispose();
                }
            };

            worker.RunWorkerAsync();

            return worker;
        }

    }
}
