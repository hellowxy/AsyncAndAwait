//****************************************************************************************************
//ClassName:       CustomAwaiter
//Author:          Xiaoying Wang
//Guid:		       3e5ab260-ee1e-4090-9ab1-427246d8b439
//DateTime:        2015/4/14 11:21:09
//Email Address:   wangxiaoying@gedu.org
//FileName:        CustomAwaiter
//CLR Version:     4.0.30319.18444
//Machine Name:    WXY-PC
//Namespace:       CustomeAwaitableType
//Function:                
//Description:    
//****************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomeAwaitableType
{
    class CustomAwaiter:INotifyCompletion
    {
        private string _result = "Completed synchronously";
        private readonly bool _completeSynchronously;

        public CustomAwaiter(bool completeSynchronously)
        {
            _completeSynchronously = completeSynchronously;
        }

        public string GetResult()
        {
            return _result;
        }

        public string GetInfo()
        {
            return string.Format("Task is running on a thread id {0}. Is thread pool thread: {1}",
                Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        }

        public bool IsCompleted
        {
            get
            {
                return _completeSynchronously;
            }
        }

        public void OnCompleted(Action continuation)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(1000);
                _result = GetInfo();
                if (continuation != null)
                {
                    continuation();
                }
            });
        }
    }
}
