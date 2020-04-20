using System.Net;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.Interfaces
{
    public interface IConsoleLogService
    {
        Result<ConsoleLog> HttpWebResponseToConsoleLog(HttpWebResponse response);
        Result<string> LogToText(ConsoleLog log);
        string CommandToText(Command command);
        string GetError(string message);
    }
}