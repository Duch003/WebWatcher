using System.Net;

namespace WebWatcher.UI.Interfaces
{
    public interface IWebReader
    {
        HttpWebResponse Get(string url);
    }
}