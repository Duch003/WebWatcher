using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebWatcher.UI.Models
{
    public class Result<T>
    {
        public T Output { get; }
        public bool IsFine { get; }
        public Exception Exception { get; }
        public Result(T output, Exception e = null)
        {
            Output = output;
            Exception = e;
            IsFine = e is null ? true : false;
        }
    }

    public class Result
    {
        public bool IsFine { get; }
        public Exception Exception { get; }
        public Result(Exception e = null)
        {
            Exception = e;
            IsFine = e is null ? true : false;
        }
    }
}
