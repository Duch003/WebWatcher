using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebWatcher.UI.Models
{
    public class Message<TTarget, TValue>
    {
        public TValue Value { get; set; }

        public Message(TValue value)
        {
            Value = value;
        }
    }
}
