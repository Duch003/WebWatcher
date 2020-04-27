using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WebWatcher.UI.Interfaces;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.ViewModels
{
    public class UCControlPanelViewModel : Screen, IUCControlPanelViewModel
    {
        private bool _canChangeAddress;
        public bool CanChangeAddress
        {
            get { return _canChangeAddress; }
            private set 
            { 
                _canChangeAddress = value;
                NotifyOfPropertyChange();
            }
        }

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

        private double _time;
        public double Time
        {
            get { return _time; }
            set
            {
                _time = value;
                NotifyOfPropertyChange();
            }
        }
        public double MultipliedTime => Time * 1000 * 60;

        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                _canStart = ValidateUrls();
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanStart);
            }
        }

        protected Timer _timer;
        protected IEventAggregator _eventAggregator;
        protected IWebService _webService;
        protected IUrlValidator _urlValidator;
        private bool _canStart = false;

        public UCControlPanelViewModel(IEventAggregator eventAggregator, IWebService webService, IUrlValidator urlValidator)
        {
            Time = 1;
            InitializeTimer();
            _eventAggregator = eventAggregator;
            _webService = webService;
            _urlValidator = urlValidator;
            _eventAggregator.Subscribe(this);
            CanChangeAddress = true;
        }

        public bool CanStart => !_timer.Enabled && _canStart;
        public void Start()
        {
            if (_timer.Interval != MultipliedTime)
            {
                Reset();
            }
            CanChangeAddress = false;
            _timer.Interval = MultipliedTime;
            _timer.Start();
            Publish(Command.Start);
            NotifyAll();
        }

        public bool CanStop => _timer.Enabled;
        public void Stop()
        {
            _timer.Stop();
            CanChangeAddress = true;
            Publish(Command.Stop);
            NotifyAll();
        }

        public bool CanReset => !_timer.Enabled;
        public void Reset()
        {
            _timer.Stop();
            _timer.Dispose();
            CanChangeAddress = true;
            InitializeTimer();
            Publish(Command.Reset);
            NotifyAll();
        }

        protected void OnElapsedEventHandler(object sender, ElapsedEventArgs e)
        {
            var result = _webService.CheckPage(Url);
            if (!result.IsFine)
            {
                _eventAggregator.PublishOnUIThread(new Message<IUCMessageBoxViewModel, Exception>(result.Exception));
                Reset();
                return;
            }

            _eventAggregator.PublishOnUIThread(new Message<IUCLogPanelViewModel, Response>(result.Output));
        }

        protected void InitializeTimer()
        {
            _timer = new Timer();
            _timer.Elapsed += OnElapsedEventHandler;
            _timer.AutoReset = true;
            _timer.Interval = MultipliedTime;
        }

        protected bool ValidateUrls()
        {
            var result = _urlValidator.IsUrlValid(Url);
            var newState = result ? Models.State.Ok.ToString() : Models.State.Error.ToString();

            if(newState == State)
            {
                return result;
            }
            else if (!result)
            {
                State = Models.State.Error.ToString();
                _eventAggregator.PublishOnUIThread(new Message<IUCMessageBoxViewModel, Command>(Command.InvalidUrl));
            }
            else
            {
                State = Models.State.Ok.ToString();
                _eventAggregator.PublishOnUIThread(new Message<IUCMessageBoxViewModel, Command>(Command.ValidUrl));
            }

            
            return result;
        }

        protected void NotifyAll()
        {
            NotifyOfPropertyChange(() => CanStart);
            NotifyOfPropertyChange(() => CanStop);
            NotifyOfPropertyChange(() => CanReset);
        }
        protected void Publish(Command command)
        {
            _eventAggregator.PublishOnUIThread(new Message<IUCMessageBoxViewModel, Command>(command));
            _eventAggregator.PublishOnUIThread(new Message<IUCLogPanelViewModel, Command>(command));
        }
    }
}
