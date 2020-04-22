using System.Net;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.Interfaces
{
    public interface IWebReader
    {
        Response Get(string url);
    }
}