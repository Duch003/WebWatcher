using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WebWatcher.UI.Interfaces;
using WebWatcher.UI.ViewModels;

namespace WebWatcher.UI.Tests.Layers
{
    public class UCControlPanelViewModelLayer : UCControlPanelViewModel
    {
        public UCControlPanelViewModelLayer(IEventAggregator eventAggregator, IWebService webService, IUrlValidator urlValidator)
            : base(eventAggregator, webService, urlValidator)
        {

        }

        public Timer Timer
        {
            get { return _timer; }
            set { _timer = value; }
        }

    }
}
