using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebWatcher.UI.Interfaces;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.ViewModels
{
    public class UCMessageBoxViewModel : Screen, IHandle<Message<IUCMessageBoxViewModel, Exception>>,
        IHandle<Message<IUCMessageBoxViewModel, Response>>, IHandle<Message<IUCMessageBoxViewModel, Command>>, 
        IUCMessageBoxViewModel
    {
        private string _state;
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyOfPropertyChange();
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set 
            { 
                _message = value;
                NotifyOfPropertyChange();
            }
        }

        private readonly IEventAggregator _eventAggregator;

        public UCMessageBoxViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            Message = "Ready to go!";
            State = Models.State.Ok.ToString();
        }

        public void Handle(Message<IUCMessageBoxViewModel, Response> message)
        {
            if(message is null)
            {
                return;
            }

            State = message.Value.State.ToString();
            Message += HttpWorkerRequest.GetStatusDescription((int)message.Value.Status) + Environment.NewLine;
        }

        public void Handle(Message<IUCMessageBoxViewModel, Exception> message)
        {
            if (message is null)
            {
                return;
            }

            State = Models.State.Error.ToString();

            if (message.Value is null)
            {
                Message = "Unknown error.";
                return;
            }

            Message = MessageBuilder(message.Value);
        }

        protected string MessageBuilder(Exception exception, int tab = 0)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"MESSAGE:");
            builder.AppendLine($"{new string('\t', tab)}{exception.Message}");

            if(exception.InnerException != null)
            {
                builder.Append($"{new string('\t', ++tab)}INNER ");
                builder.AppendLine($"{new string('\t', tab)}{MessageBuilder(exception.InnerException, tab)}");
            }

            return builder.ToString();
        }

        public void Handle(Message<IUCMessageBoxViewModel, Command> message)
        {
            if(message is null)
            {
                return;
            }

            switch (message.Value)
            {
                case Command.Start:
                    State = Models.State.Warning.ToString();
                    Message = "Running...";
                    break;
                case Command.Stop:
                    State = Models.State.Warning.ToString();
                    Message = "Ready to resume.";
                    break;
                case Command.ValidUrl:
                case Command.Reset:
                    State = Models.State.Ok.ToString();
                    Message = "Ready to go!";
                    break;
                case Command.InvalidUrl:
                    State = Models.State.Error.ToString();
                    Message = "Some urls are invalid.";
                    break;
            }
        }
    }
}
