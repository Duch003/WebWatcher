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

        public Result<Response> CheckPage(string url)
        {
            if (!_validator.IsUrlValid(url))
            {
                var message = $"Url <<{url}>> is invalid.";
                return new Result<Response>(null, new ArgumentException(message, nameof(url)));
            }

            try
            {
                var response = _webReader.Get(url);
                return new Result<Response>(response);
            }
            catch (Exception e)
            {
                return new Result<Response>(null, e);
            }
        }
    }
}
