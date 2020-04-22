using System.Net;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.Interfaces
{
    public interface IMessageService
    {
        Result<string> ResponseToText(Response log);
        string CommandToText(Command command);
        string GetError(string message);
    }
}