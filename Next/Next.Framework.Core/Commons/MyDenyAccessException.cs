using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public class MyDenyAccessException : UnauthorizedAccessException
    {
        public MyDenyAccessException(string message)
            : base(message)
        {

        }
    }
}
