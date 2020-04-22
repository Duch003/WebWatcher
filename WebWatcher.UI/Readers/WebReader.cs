using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.Interfaces;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.Readers
{
    public class WebReader : IWebReader
    {
        public Response Get(string url)
        {
            HttpWebResponse response;
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Timeout = 5000;
            try
            {
                response = (HttpWebResponse)request.GetResponse();                
            }
            catch (WebException we)
            {
                response = (HttpWebResponse)we.Response;
            }
            catch
            {
                throw;
            }

            return MapToResponse(response);
        }

        private Response MapToResponse(HttpWebResponse httpWebResponse)
        {
            if (httpWebResponse is null)
            {
                throw new ArgumentNullException(nameof(httpWebResponse));
            }

            var output = new Response
            {
                DateTime = DateTime.Now,
                Status = httpWebResponse.StatusCode,
                Url = httpWebResponse.ResponseUri.ToString()
            };

            if ((int)httpWebResponse.StatusCode >= 200 && (int)httpWebResponse.StatusCode < 300)
            {
                output.State = State.Ok;
            }
            else if ((int)httpWebResponse.StatusCode >= 400 && (int)httpWebResponse.StatusCode < 600)
            {
                output.State = State.Error;
            }
            else
            {
                output.State = State.Warning;
            }

            return output;
        }
    }
}
