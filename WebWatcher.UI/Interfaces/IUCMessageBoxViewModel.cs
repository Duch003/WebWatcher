using Caliburn.Micro;
using System;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.Interfaces
{
    public interface IUCMessageBoxViewModel : IScreen
    {
        string State { get; set; }
        string Message { get; set; }
        void Handle(Message<IUCMessageBoxViewModel, Response> message);
        void Handle(Message<IUCMessageBoxViewModel, Exception> message);
        void Handle(Message<IUCMessageBoxViewModel, Command> message);
    }
}