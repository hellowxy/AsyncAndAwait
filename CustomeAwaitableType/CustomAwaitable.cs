//****************************************************************************************************
//ClassName:       CustomAwaitable
//Author:          Xiaoying Wang
//Guid:		       9ebb6959-7e85-4ab3-9414-a75cb00fbbd6
//DateTime:        2015/4/14 11:24:54
//Email Address:   wangxiaoying@gedu.org
//FileName:        CustomAwaitable
//CLR Version:     4.0.30319.18444
//Machine Name:    WXY-PC
//Namespace:       CustomeAwaitableType
//Function:                
//Description:    
//****************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomeAwaitableType
{
    class CustomAwaitable
    {
        private readonly bool _completeSynchronously;
        public CustomAwaitable(bool completeSynchronously)
        {
            _completeSynchronously = completeSynchronously;
        }

        public CustomAwaiter GetAwaiter()
        {
            return new CustomAwaiter(_completeSynchronously);
        }
    }
}
