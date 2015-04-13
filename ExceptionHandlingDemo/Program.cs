using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionHandlingDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = AsynchronousProcessing();
            t.Wait();
            Console.WriteLine("done");
            Console.Read();
        }

        async static Task AsynchronousProcessing()
        {
            Console.WriteLine("1. Single exception");
            //We just use the try/catch statement and get exception's details for a single task.
            try
            {
                string result = await GetInfoAsync("Task 1", 2);
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception details:{0}",ex);
            }
            Console.WriteLine();
            Console.WriteLine("2. Multiple exceptions");

            Task<string> t1 = GetInfoAsync("Task 1", 3);
            Task<string> t2 = GetInfoAsync("Task 2", 2);
            //A very common mistake is using the same approach when more than one asynchronous operation is being
            //awaited. If we use the catch block the same way as before, we will get only the first exception
            //from the underlying AggregateException object.
            try
            {
                string[] results = await Task.WhenAll(t1, t2);
                Console.WriteLine(results.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception details:{0}", ex);
            }
            Console.WriteLine();
            Console.WriteLine("2. Multiple exceptions with AggregateException");

            t1 = GetInfoAsync("Task 1", 3);
            t2 = GetInfoAsync("Task 2", 2);
            Task<string[]> t3 = Task.WhenAll(t1, t2);
            //To collect all the information, we have to use the awaited tasks' Exception property.
            //We flatten the AggregateException hierarchy, and then unwrap all the underlying exceptions
            //from it using the Flatten method of AggregateException.
            try
            {
                string[] results = await t3;
                Console.WriteLine(results.Length);
            }
            catch
            {
                var ae = t3.Exception.Flatten();
                var exceptions = ae.InnerExceptions;
                Console.WriteLine("Exceptions caught:{0}",exceptions.Count);
                foreach (var exception in exceptions)
                {
                    Console.WriteLine("Exception details:{0}",exception);
                    Console.WriteLine();
                }
            }
        }

        async static Task<string> GetInfoAsync(string name, int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            throw new Exception(string.Format("Boom from {0}!",name));
        }
    }
}
