using Caliburn.Micro;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.Interfaces
{
    public interface IUCLogPanelViewModel : IScreen
    {
        string Notes { get; set; }
        void Handle(Message<IUCMessageBoxViewModel, Command> message);
        void Handle(Message<IUCLogPanelViewModel, ConsoleLog> message);
    }
}