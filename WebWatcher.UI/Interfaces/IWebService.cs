using System.Net;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.Interfaces
{
    public interface IWebService
    {
        Result<Response> CheckPage(string url);
    }
}