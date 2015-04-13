using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
/*********************************
 * When you run the program. you might notice that both tasks are likely to be served by the same worker thread
 * from a thread pool. let's comment out the await Task.Delay line inside the GetInfoAsync method and uncomment the 
 * await Task.Run, we will see that in this case both the tasks will be served by different worker threads.
 * 
 * Task.Delay uses a timer under the hood, and processing goes as follows: we get the worker thread from a thread pool,
 * which awaits the Task.Delay method to return a result. Then, the Task.Delay method starts the timer and specifies a
 * piece of code that wil be called when the timer counts the number of seconds specified to the Task.Delay method. Then
 * we immediately return the worker thread to a thread pool. When the timer event runs, we get any available worker
 * thread from a thread pool once again(which could be the same thread we used first) and run the code provided to the
 * timer on it.
 * 
 * When we use the Task.Run method, we get a worker thread from a thread pool and make it block for a number
 * of seconds, provided to the Thread.Sleep method. Then, we get a second worker thread and block it as well.
 * In this scenario, we consume two worker threads and they do absolutely nothing, not being able to perform any
 * other task while waiting.
 * ****************************/
namespace ParallelAsyncTaskDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = AsynchronousProcessing();
            t.Wait();
            Console.Read();
        }

        async static Task AsynchronousProcessing()
        {
            Task<string> t1 = GetInfoAsync("Task 1", 3);
            Task<string> t2 = GetInfoAsync("Task 2", 5);
            //We use a Task.WhenAll helper method to create another task that will complete only
            //when all of the underlying tasks complete.
            string[] results = await Task.WhenAll(t1, t2);
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }

        async static Task<string> GetInfoAsync(string name, int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            //await Task.Run(() => Thread.Sleep(TimeSpan.FromSeconds(seconds)));
            return string.Format("Task {0} is running on a thread id {1}. Is thread pool thread:{2}",
                name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        }
    }
}
