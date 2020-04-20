using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.Interfaces;

namespace WebWatcher.UI.Validators
{
    public class UrlValidator : IUrlValidator
    {
        public bool IsUrlValid(string url)
            => !string.IsNullOrWhiteSpace(url) && Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && ( uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps );
    }
}
