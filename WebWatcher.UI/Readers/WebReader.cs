using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.Interfaces;

namespace WebWatcher.UI.Readers
{
    public class WebReader : IWebReader
    {
        public HttpWebResponse Get(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Timeout = 5000;
            try
            {
                return (HttpWebResponse)request.GetResponse();
                
            }
            catch (WebException we)
            {
                return (HttpWebResponse)we.Response;
            }
            catch
            {
                throw;
            }
        }
    }
}
