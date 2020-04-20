using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.Interfaces;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.Services
{
    public class WebService : IWebService
    {
        protected IWebReader _webReader;
        protected IUrlValidator _validator;

        public WebService(IWebReader webReader, IUrlValidator validator)
        {
            _webReader = webReader;
            _validator = validator;
        }

        public Result<HttpWebResponse> CheckPage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return new Result<HttpWebResponse>(null, new ArgumentNullException(nameof(url)));
            }

            bool isValid;
            try
            {
                isValid = _validator.IsUrlValid(url);
            }
            catch (Exception e)
            {
                return new Result<HttpWebResponse>(null, e);
            }

            if (!isValid)
            {
                var message = $"Url <<{url}>> is invalid.";
                var e = new ArgumentException(message, nameof(url));
                return new Result<HttpWebResponse>(null, e);
            }

            try
            {
                var response = _webReader.Get(url);
                return new Result<HttpWebResponse>(response);
            }
            catch (Exception e)
            {
                return new Result<HttpWebResponse>(null, e);
            }
        }
    }
}
