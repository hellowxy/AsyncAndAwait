/**************************************************************************
 * an await expression is required to be awaitable. An expression t is awaitable if one of the flowwing holds:
 * t is of compile time type dynamic or
 * t has an accessible instance or extension method called GetAwaiter with no parameters and no type parameters,
 * and a return type A for which all of the following hold:
 * 1.A implements the interface System.Runtime.CompilerServices.INotifyCompletion
 * 2.A has an accessible, readable instance property IsCompleted of type bool
 * 3.A has an accessible instance method GetResult with no parameters and no type parameters* 
 * 
 * If the isCompleted property returns true, we just call the GetResult method synchronously. This prevents us from 
 * allocating resources for asynchronous task execution if the operation has already been completed. Otherwise, we 
 * register a callback action to the OnCompleted method of CustomAwaiter and start the asynchronous operation. When
 * it completes, it calls the provided callback, which will get the result by calling the GetResult method on the CustomAwaiter Object.
 * 
 * Whenever you write asynchronous functions, the most natural approach is to use the standard Task type.
 ****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomeAwaitableType
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = AsynchronousProcessing();
            t.Wait();
            Console.ReadLine();
        }

        async static Task AsynchronousProcessing()
        {
            var sync = new CustomAwaitable(true);
            string result = await sync;
            Console.WriteLine(result);

            var async = new CustomAwaitable(false);
            result = await async;
            Console.WriteLine(result);
        }
    }
}
