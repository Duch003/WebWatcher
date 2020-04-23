using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebWatcher.UI.Models
{
    public class Response
    {
        public State State { get; set; }
        public HttpStatusCode Status { get; set; }
        public DateTime DateTime { get; set; }
        public string Url { get; set; }

        public Response Clone()
        {
            return (Response)this.MemberwiseClone();
        }
    }
}
