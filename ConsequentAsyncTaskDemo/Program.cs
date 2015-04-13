using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsequentAsyncTaskDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = AsynchronyWithTPL();
            t.Wait();
            t = AsynchronyWithAwait();
            t.Wait();
            Console.Read();
        }

        static Task AsynchronyWithTPL()
        {
            var containerTask = new Task(() =>
            {
                Task<string> t = GetInfoAsync("TPL 1");
                t.ContinueWith(task =>
                {
                    Console.WriteLine(t.Result);
                    Task<string> t2 = GetInfoAsync("TPL 2");
                    t2.ContinueWith(innerTask => Console.WriteLine(innerTask.Result), TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.AttachedToParent);
                    t2.ContinueWith(innerTask =>
                    { if (innerTask.Exception != null) Console.WriteLine(innerTask.Exception.InnerException); }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent);
                });
                t.ContinueWith(task => Console.WriteLine(t.Exception.InnerException),
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent);
            });
            containerTask.Start();
            return containerTask;
        }

        static async Task AsynchronyWithAwait()
        {
            try
            {
                string result = await GetInfoAsync("Async 1");
                Console.WriteLine(result);
                result = await GetInfoAsync("Async 2");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static async Task<string> GetInfoAsync(string name)
        {
            Console.WriteLine("Task {0} started!",name);
            await Task.Delay(TimeSpan.FromSeconds(2));
            if (name == "TPL 2")
            {
                throw new Exception("Boom!");
            }
            return string.Format("Task {0} is running on a thread id {1}. Is thread pool thread:{2}",
                name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        }
    }
}
