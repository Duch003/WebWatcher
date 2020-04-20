using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WebWatcher.UI.Interfaces;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.ViewModels
{

    public class UCLogPanelViewModel : Screen, IHandle<Message<IUCLogPanelViewModel, ConsoleLog>>, 
        IHandle<Message<IUCMessageBoxViewModel, Command>>, IUCLogPanelViewModel
    {
        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyOfPropertyChange();
            }
        }

        private readonly IEventAggregator _eventAggregator;
        private readonly IConsoleLogService _logService;

        public UCLogPanelViewModel(IEventAggregator eventAggregator, IConsoleLogService logService)
        {
            _eventAggregator = eventAggregator;
            _logService = logService;
            _eventAggregator.Subscribe(this);
        }

        public void Handle(Message<IUCLogPanelViewModel, ConsoleLog> message)
        {
            if (message.Value is null)
            {
                return;
            }

            var result = _logService.LogToText(message.Value);

            if (!result.IsFine)
            {
                _eventAggregator.PublishOnUIThread(new Message<IUCMessageBoxViewModel, Exception>(result.Exception));
                return;
            }

            Notes += result.Output;
        }

        public void Handle(Message<IUCMessageBoxViewModel, Command> message)
        {
            if (message is null)
            {
                return;
            }

            switch (message.Value)
            {
                case Command.Reset:
                    Notes = string.Empty;
                    Notes += _logService.CommandToText(message.Value);
                    break;
                default:
                    Notes += _logService.CommandToText(message.Value);
                    break;
            }
        }
    }
}
