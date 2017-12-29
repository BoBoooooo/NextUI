using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public class CommonResult
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public string ObjectData { get; set; }
    }
}
