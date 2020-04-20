using Caliburn.Micro;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using WebWatcher.UI.Interfaces;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.ViewModels
{
    public class RootViewModel : Conductor<IScreen>.Collection.AllActive
    {
        protected IWebService _webService;
        protected IEventAggregator _eventAggregator;
        protected Timer _timer;

        private int _timeInMinutes;
        public int TimeInMinutes
        {
            get { return _timeInMinutes; }
            set 
            {
                _timeInMinutes = value;
                NotifyOfPropertyChange();
            }
        }

        private IUCLogPanelViewModel _uCLogPanelViewModel;
        public IUCLogPanelViewModel UCLogPanelViewModel
        {
            get { return _uCLogPanelViewModel; }
            set 
            {
                _uCLogPanelViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        private IUCMessageBoxViewModel _uCMessageBoxViewModel;
        public IUCMessageBoxViewModel UCMessageBoxViewModel
        {
            get { return _uCMessageBoxViewModel; }
            set 
            {
                _uCMessageBoxViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        private IUCControlPanelViewModel _uCControlPanelViewModel;
        public IUCControlPanelViewModel UCControlPanelViewModel
        {
            get { return _uCControlPanelViewModel; }
            set 
            { 
                _uCControlPanelViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        public RootViewModel(IWebService webService, IEventAggregator eventAggregator)
        {
            _webService = webService;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            UCMessageBoxViewModel = Startup.Resolve<IUCMessageBoxViewModel>();
            UCLogPanelViewModel = Startup.Resolve<IUCLogPanelViewModel>();
            UCControlPanelViewModel = Startup.Resolve<IUCControlPanelViewModel>();
        }

        public void Click()
        {
            var temp = new ConsoleLog();
            temp.DateTime = DateTime.Now;
            temp.State = State.Ok;
            temp.Status = HttpStatusCode.OK;
            temp.Url = "test";
            var message1 = new Message<IUCLogPanelViewModel, ConsoleLog>(temp);
            var message2 = new Message<IUCMessageBoxViewModel, ConsoleLog>(temp);
            _eventAggregator.PublishOnUIThread(message1);
            _eventAggregator.PublishOnUIThread(message2);
        }
    }
}
