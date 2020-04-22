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
                ValidateUrls();
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanStart);
            }
        }

        protected Timer _timer;
        protected IEventAggregator _eventAggregator;
        protected IWebService _webService;
        protected IUrlValidator _urlValidator;
        private readonly IMessageService _logService;

        public UCControlPanelViewModel(IEventAggregator eventAggregator, IWebService webService, IUrlValidator urlValidator,
            IMessageService logService)
        {
            Time = 1;
            InitializeTimer();
            _eventAggregator = eventAggregator;
            _webService = webService;
            _urlValidator = urlValidator;
            _logService = logService;
            _eventAggregator.Subscribe(this);
        }

        public bool CanStart => !_timer.Enabled && ValidateUrls();
        public void Start()
        {
            if (_timer.Interval != MultipliedTime)
            {
                Reset();
            }
            _timer.Interval = MultipliedTime;
            _timer.Start();
            _eventAggregator.PublishOnUIThread(new Message<IUCMessageBoxViewModel, Command>(Command.Start));
            _eventAggregator.PublishOnUIThread(new Message<IUCLogPanelViewModel, Command>(Command.Start));
            NotifyAll();
        }

        public bool CanStop => _timer.Enabled;
        public void Stop()
        {
            _timer.Stop();
            _eventAggregator.PublishOnUIThread(new Message<IUCMessageBoxViewModel, Command>(Command.Stop));
            _eventAggregator.PublishOnUIThread(new Message<IUCLogPanelViewModel, Command>(Command.Stop));
            NotifyAll();
        }

        public bool CanReset => !_timer.Enabled;
        public void Reset()
        {
            _timer.Stop();
            _timer.Dispose();
            InitializeTimer();
            _eventAggregator.PublishOnUIThread(new Message<IUCMessageBoxViewModel, Command>(Command.Reset));
            _eventAggregator.PublishOnUIThread(new Message<IUCLogPanelViewModel, Command>(Command.Reset));
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
            if (!result)
            {
                State = Models.State.Error.ToString();
                _eventAggregator.PublishOnUIThread(new Message<IUCMessageBoxViewModel, Command>(Command.InvalidUrl));
                return false;
            }

            State = Models.State.Ok.ToString();
            _eventAggregator.PublishOnUIThread(new Message<IUCMessageBoxViewModel, Command>(Command.ValidUrl));
            return true;
        }

        protected void NotifyAll()
        {
            NotifyOfPropertyChange(() => CanStart);
            NotifyOfPropertyChange(() => CanStop);
            NotifyOfPropertyChange(() => CanReset);
        }
    }
}
