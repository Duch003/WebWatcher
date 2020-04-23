using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.Interfaces;

namespace WebWatcher.UI.Services
{
    public class DateTimeSerivce : IDateTimeSerivce
    {
        public DateTime GetNow() => DateTime.Now;
    }
}
