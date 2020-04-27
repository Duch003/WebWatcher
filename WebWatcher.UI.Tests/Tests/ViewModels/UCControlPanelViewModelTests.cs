using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.ViewModels;
using Caliburn.Micro;
using WebWatcher.UI.Models;
using WebWatcher.UI.Interfaces;
using System.Net;
using WebWatcher.UI.Tests.Layers;
using WebWatcher.UI.Tests.Mocks;
using System.Diagnostics;

namespace WebWatcher.UI.Tests.Tests.ViewModels
{
    [TestFixture]
    public class UCControlPanelViewModelTests
    {
        private string _validUrl = "valid";
        private string _invalidUrl = "invalid";
        private string _exceptionUrl = "exception";
        private Response _response;
        private Result<Response> _okResult;
        private Result<Response> _errorResult_InvalidUrl;
        private Result<Response> _errorResult_ReaderError;
        private Dictionary<Command, int> _messageBoxCallList;
        private Dictionary<Command, int> _logPanelCallList;
        private UCControlPanelViewModelLayer _viewModel;

        private TestContext _testContext;

        public TestContext TestContext
        {
            get { return _testContext; }
            set { _testContext = value; }
        }

        public UCControlPanelViewModelTests()
        {
            ZeroMessageBoxCallList();
            ZeroLogPanelCallList();
            _response = new Response
            {
                DateTime = new DateTime(2020, 4, 24, 21, 18, 19),
                State = State.Ok,
                Status = HttpStatusCode.OK,
                Url = "test"
            };
            _okResult = new Result<Response>(_response);
            _errorResult_InvalidUrl = new Result<Response>(null, new Exception("Invalid url"));
            _errorResult_ReaderError = new Result<Response>(null, new Exception("Reader error"));

            InstantializeViewModel();
        }

        private void InstantializeViewModel()
        {
            var mockEventAggregator = new MockEventAggregator();
            mockEventAggregator.AddCallback(typeof(Message<IUCMessageBoxViewModel, Command>), message =>
            {
                _messageBoxCallList[( (Message<IUCMessageBoxViewModel, Command>)message ).Value] += 1;
            });
            mockEventAggregator.AddCallback(typeof(Message<IUCLogPanelViewModel, Command>), message =>
            {
                _logPanelCallList[( (Message<IUCLogPanelViewModel, Command>)message ).Value] += 1;
            });

            var mockWebService = new Mock<IWebService>();
            mockWebService.Setup(x => x.CheckPage(_validUrl)).Returns(_okResult);
            mockWebService.Setup(x => x.CheckPage(_invalidUrl)).Returns(_errorResult_InvalidUrl);
            mockWebService.Setup(x => x.CheckPage(_exceptionUrl)).Returns(_errorResult_ReaderError);

            var mockUrlValidator = new Mock<IUrlValidator>();
            mockUrlValidator.Setup(x => x.IsUrlValid(_validUrl)).Returns(true);
            mockUrlValidator.Setup(x => x.IsUrlValid(_invalidUrl)).Returns(false);
            mockUrlValidator.Setup(x => x.IsUrlValid(_exceptionUrl)).Returns(true);

            _viewModel = new UCControlPanelViewModelLayer(mockEventAggregator, mockWebService.Object, mockUrlValidator.Object);
        }

        [TearDown]
        public void TearDown()
        {
            ZeroLogPanelCallList();
            ZeroMessageBoxCallList();
            InstantializeViewModel();
        }

        [Test]
        public void InnerState_ProvidedUrlIsInvalid_IsInBlockedState()
        {
            /*Blocked state:
             *  WHILE 
             *      vm.Url = _invalidUrl
             *  STATE IS:
             *      vm.State = "Error"
             *      vm._timer.Enabled = false
             *      vm.CanStart = false
             *      vm.CanStop = false
             *      vm.CanReset = true
             *      vm.CanChangeAddress = true
             */

            //Arrage
            _viewModel.Url = _invalidUrl;

            //Act
            //Assert
            Assert.IsTrue(_viewModel.State == State.Error.ToString());
            Assert.IsFalse(_viewModel.Timer.Enabled);
            Assert.IsFalse(_viewModel.CanStart);
            Assert.IsFalse(_viewModel.CanStop);
            Assert.IsTrue(_viewModel.CanReset);
            Assert.IsTrue(_viewModel.CanChangeAddress);
            Assert.IsTrue(_messageBoxCallList[Command.InvalidUrl] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Start] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Stop] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Reset] == 0);
            Assert.IsTrue(_logPanelCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Start] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Stop] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Reset] == 0);
        }

        [Test]
        public void InnerState_ProvidedUrlIsInvalid_Resetted_IsInBlockedState()
        {
            /*Blocked state:
             *  WHILE 
             *      vm.Url = _invalidUrl
             *  STATE IS:
             *      vm.State = "Error"
             *      vm._timer.Enabled = false
             *      vm.CanStart = false
             *      vm.CanStop = false
             *      vm.CanReset = true
             *      vm.CanChangeAddress = true
             */

            //Arrage
            _viewModel.Url = _invalidUrl;

            //Act
            _viewModel.Reset();

            //Assert
            Assert.IsTrue(_viewModel.State == State.Error.ToString());
            Assert.IsFalse(_viewModel.Timer.Enabled);
            Assert.IsFalse(_viewModel.CanStart);
            Assert.IsFalse(_viewModel.CanStop);
            Assert.IsTrue(_viewModel.CanReset);
            Assert.IsTrue(_viewModel.CanChangeAddress);
            Assert.IsTrue(_messageBoxCallList[Command.InvalidUrl] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Start] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Stop] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Reset] == 1);
            Assert.IsTrue(_logPanelCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Start] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Stop] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Reset] == 1);
        }

        [Test]
        public void InnerState_ProvidedUrlIsInvalid_Resetted_ThenResetted_IsInBlockedState()
        {
            /*Blocked state:
             *  WHILE 
             *      vm.Url = _invalidUrl
             *      vm.Reset()
             *      vm.Reset()
             *  STATE IS:
             *      vm.State = "Error"
             *      vm._timer.Enabled = false
             *      vm.CanStart = false
             *      vm.CanStop = false
             *      vm.CanReset = true
             *      vm.CanChangeAddress = true
             */

            //Arrage
            _viewModel.Url = _invalidUrl;

            //Act
            _viewModel.Reset();
            _viewModel.Reset();

            //Assert
            Assert.IsTrue(_viewModel.State == State.Error.ToString());
            Assert.IsFalse(_viewModel.Timer.Enabled);
            Assert.IsFalse(_viewModel.CanStart);
            Assert.IsFalse(_viewModel.CanStop);
            Assert.IsTrue(_viewModel.CanReset);
            Assert.IsTrue(_viewModel.CanChangeAddress);
            Assert.IsTrue(_messageBoxCallList[Command.InvalidUrl] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Start] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Stop] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Reset] == 2);
            Assert.IsTrue(_logPanelCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Start] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Stop] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Reset] == 2);
        }

        [Test]
        public void InnerState_ProvidedUrlIsValid_IsInStandbyState()
        {
            /*Standby state:
             *  WHILE 
             *      vm.Url = _validUrl
             *  STATE IS:
             *      vm.State = "Ok"
             *      vm._timer.Enabled = false
             *      vm.CanStart = true
             *      vm.CanStop = false
             *      vm.CanReset = true
             *      vm.CanChangeAddress = true
             */

            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            //Assert
            Assert.IsTrue(_viewModel.State == State.Ok.ToString());
            Assert.IsFalse(_viewModel.Timer.Enabled);
            Assert.IsTrue(_viewModel.CanStart);
            Assert.IsFalse(_viewModel.CanStop);
            Assert.IsTrue(_viewModel.CanReset);
            Assert.IsTrue(_viewModel.CanChangeAddress);
            Assert.IsTrue(_messageBoxCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.ValidUrl] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Start] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Stop] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Reset] == 0);
            Assert.IsTrue(_logPanelCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Start] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Stop] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Reset] == 0);
        }

        [Test]
        public void InnerState_ProvidedUrlIsValid_Started_IsInRunningState()
        {
            /*Running state:
             *  WHILE 
             *      vm.Url = _validUrl
             *      vm.Start()
             *  STATE IS:
             *      vm.State = "Ok"
             *      vm._timer.Enabled = true
             *      vm.CanStart = false
             *      vm.CanStop = true
             *      vm.CanReset = false
             *      vm.CanChangeAddress = false
             */

            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            _viewModel.Start();

            //Assert
            Assert.IsTrue(_viewModel.State == State.Ok.ToString());
            Assert.IsTrue(_viewModel.Timer.Enabled);
            Assert.IsFalse(_viewModel.CanStart);
            Assert.IsTrue(_viewModel.CanStop);
            Assert.IsFalse(_viewModel.CanReset);
            Assert.IsFalse(_viewModel.CanChangeAddress);
            Assert.IsTrue(_messageBoxCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.ValidUrl] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Start] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Stop] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Reset] == 0);
            Assert.IsTrue(_logPanelCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Start] == 1);
            Assert.IsTrue(_logPanelCallList[Command.Stop] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Reset] == 0);
        }

        [Test]
        public void InnerState_ProvidedUrlIsValid_Started_ThenStopped_IsInStandbyState()
        {
            /*Standby state:
             *  WHILE 
             *      1. vm.Url = _validUrl
             *      2. vm.Start()
             *      3. vm.Stop()
             *  STATE IS:
             *      vm.State = "Ok"
             *      vm._timer.Enabled = false
             *      vm.CanStart = true
             *      vm.CanStop = false
             *      vm.CanReset = true
             *      vm.CanChangeAddress = true
             */

            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            _viewModel.Start();
            _viewModel.Stop();

            //Assert
            Assert.IsTrue(_viewModel.State == State.Ok.ToString());
            Assert.IsFalse(_viewModel.Timer.Enabled);
            Assert.IsTrue(_viewModel.CanStart);
            Assert.IsFalse(_viewModel.CanStop);
            Assert.IsTrue(_viewModel.CanReset);
            Assert.IsTrue(_viewModel.CanChangeAddress);
            Assert.IsTrue(_messageBoxCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.ValidUrl] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Start] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Stop] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Reset] == 0);
            Assert.IsTrue(_logPanelCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Start] == 1);
            Assert.IsTrue(_logPanelCallList[Command.Stop] == 1);
            Assert.IsTrue(_logPanelCallList[Command.Reset] == 0);
        }

        [Test]
        public void InnerState_ProvidedUrlIsValid_Started_ThenStopped_ThenStarted_IsInRunningState()
        {
            /*Running state:
             *  WHILE 
             *      1. vm.Url = _validUrl
             *      2. vm.Start()
             *      3. vm.Stop()
             *      4. vm.Start()
             *  STATE IS:
             *      vm.State = "Ok"
             *      vm._timer.Enabled = true
             *      vm.CanStart = false
             *      vm.CanStop = true
             *      vm.CanReset = false
             *      vm.CanChangeAddress = false
             */

            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            _viewModel.Start();
            _viewModel.Stop();
            _viewModel.Start();

            //Assert
            Assert.IsTrue(_viewModel.State == State.Ok.ToString());
            Assert.IsTrue(_viewModel.Timer.Enabled);
            Assert.IsFalse(_viewModel.CanStart);
            Assert.IsTrue(_viewModel.CanStop);
            Assert.IsFalse(_viewModel.CanReset);
            Assert.IsFalse(_viewModel.CanChangeAddress);
            Assert.IsTrue(_messageBoxCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.ValidUrl] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Start] == 2);
            Assert.IsTrue(_messageBoxCallList[Command.Stop] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Reset] == 0);
            Assert.IsTrue(_logPanelCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Start] == 2);
            Assert.IsTrue(_logPanelCallList[Command.Stop] == 1);
            Assert.IsTrue(_logPanelCallList[Command.Reset] == 0);
        }

        [Test]
        public void InnerState_ProvidedUrlIsValid_Started_ThenStopped_ThenResetted_IsInStandbyState()
        {
            /*Standby state:
             *  WHILE 
             *      1. vm.Url = _validUrl
             *      2. vm.Start()
             *      3. vm.Stop()
             *      4. vm.Reset()
             *  STATE IS:
             *      vm.State = "Ok"
             *      vm._timer.Enabled = false
             *      vm.CanStart = true
             *      vm.CanStop = false
             *      vm.CanReset = true
             *      vm.CanChangeAddress = true
             */

            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            _viewModel.Start();
            _viewModel.Stop();
            _viewModel.Reset();

            //Assert
            Assert.IsTrue(_viewModel.State == State.Ok.ToString());
            Assert.IsFalse(_viewModel.Timer.Enabled);
            Assert.IsTrue(_viewModel.CanStart);
            Assert.IsFalse(_viewModel.CanStop);
            Assert.IsTrue(_viewModel.CanReset);
            Assert.IsTrue(_viewModel.CanChangeAddress);
            Assert.IsTrue(_messageBoxCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.ValidUrl] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Start] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Stop] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Reset] == 1);
            Assert.IsTrue(_logPanelCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Start] == 1);
            Assert.IsTrue(_logPanelCallList[Command.Stop] == 1);
            Assert.IsTrue(_logPanelCallList[Command.Reset] == 1);
        }

        [Test]
        public void InnerState_ProvidedUrlIsValid_Started_ThenStopped_ThenResetted_ThenResetted_IsInStandbyState()
        {
            /*Standby state:
             *  WHILE 
             *      1. vm.Url = _validUrl
             *      2. vm.Start()
             *      3. vm.Stop()
             *      4. vm.Reset()
             *      5. vm.Reset()
             *  STATE IS:
             *      vm.State = "Ok"
             *      vm._timer.Enabled = false
             *      vm.CanStart = true
             *      vm.CanStop = false
             *      vm.CanReset = true
             *      vm.CanChangeAddress = true
             */

            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            _viewModel.Start();
            _viewModel.Stop();
            _viewModel.Reset();
            _viewModel.Reset();

            //Assert
            Assert.IsTrue(_viewModel.State == State.Ok.ToString());
            Assert.IsFalse(_viewModel.Timer.Enabled);
            Assert.IsTrue(_viewModel.CanStart);
            Assert.IsFalse(_viewModel.CanStop);
            Assert.IsTrue(_viewModel.CanReset);
            Assert.IsTrue(_viewModel.CanChangeAddress);
            Assert.IsTrue(_messageBoxCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.ValidUrl] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Start] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Stop] == 1);
            Assert.IsTrue(_messageBoxCallList[Command.Reset] == 2);
            Assert.IsTrue(_logPanelCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Start] == 1);
            Assert.IsTrue(_logPanelCallList[Command.Stop] == 1);
            Assert.IsTrue(_logPanelCallList[Command.Reset] == 2);
        }

        [Test]
        public void InnerState_StartingPoint_IsInBlockedState()
        {
            /*Blocked state:
             *  WHILE 
             *      --Just initialized--
             *  STATE IS:
             *      vm.State = null
             *      vm.Url = null
             *      vm.State = "Error"
             *      vm._timer.Enabled = false
             *      vm.CanStart = false
             *      vm.CanStop = false
             *      vm.CanReset = true
             *      vm.CanChangeAddress = true
             */

            //Arrage
            

            //Act
            //Assert
            Assert.IsTrue(_viewModel.State == null);
            Assert.IsTrue(_viewModel.Url == null);
            Assert.IsFalse(_viewModel.Timer.Enabled);
            Assert.IsFalse(_viewModel.CanStart);
            Assert.IsFalse(_viewModel.CanStop);
            Assert.IsTrue(_viewModel.CanReset);
            Assert.IsTrue(_viewModel.CanChangeAddress);
            Assert.IsTrue(_messageBoxCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Start] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Stop] == 0);
            Assert.IsTrue(_messageBoxCallList[Command.Reset] == 0);
            Assert.IsTrue(_logPanelCallList[Command.InvalidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.ValidUrl] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Start] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Stop] == 0);
            Assert.IsTrue(_logPanelCallList[Command.Reset] == 0);
        }

        [Test]
        public void CanStart_UrlIsValid_ReturnsTrue()
        {
            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            //Assert
            Assert.IsTrue(_viewModel.CanStart);
        }

        [Test]
        public void CanStart_UrlIsInvalid_ReturnsFalse()
        {
            //Arrage
            _viewModel.Url = _invalidUrl;

            //Act
            //Assert
            Assert.IsFalse(_viewModel.CanStart);
        }

        [Test]
        public void CanStart_UrlIsValid_Started_ThenStopped_ReturnsTrue()
        {
            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            _viewModel.Start();
            _viewModel.Stop();

            //Assert
            Assert.IsTrue(_viewModel.CanStart);
        }

        [Test]
        public void CanStart_UrlIsValid_Started_ReturnsFalse()
        {
            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            _viewModel.Start();

            //Assert
            Assert.IsFalse(_viewModel.CanStart);
        }

        [Test]
        public void CanStop_IsInRunningMode_ReturnsTrue()
        {
            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            _viewModel.Start();

            //Assert
            Assert.IsTrue(_viewModel.CanStop);
        }

        [Test]
        public void CanStop_IsInStandbyMode_ReturnsFalse()
        {
            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            //Assert
            Assert.IsFalse(_viewModel.CanStop);
        }

        [Test]
        public void CanStop_IsInStandbyMode_Started_ThenStopped_ReturnsFalse()
        {
            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            _viewModel.Start();
            _viewModel.Stop();

            //Assert
            Assert.IsFalse(_viewModel.CanStop);
        }

        [Test]
        public void CanReset_IsInStandbyMode_UrlIsValid_ReturnsTrue()
        {
            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            //Assert
            Assert.IsTrue(_viewModel.CanReset);
        }

        [Test]
        public void CanReset_IsInStandbyMode_UrlIsInvalid_ReturnsTrue()
        {
            //Arrage
            _viewModel.Url = _invalidUrl;

            //Act
            //Assert
            Assert.IsTrue(_viewModel.CanReset);
        }

        [Test]
        public void CanReset_IsInRunningMode_ReturnsFlase()
        {
            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            _viewModel.Start();

            //Assert
            Assert.IsFalse(_viewModel.CanReset);
        }

        [Test]
        public void CanReset_IsInRunningMode_Stopped_ReturnsTrue()
        {
            //Arrage
            _viewModel.Url = _validUrl;

            //Act
            _viewModel.Start();
            _viewModel.Stop();

            //Assert
            Assert.IsTrue(_viewModel.CanReset);
        }

        [Test]
        public void Start_GetTimeFromMultipliedTimePropAndStartsTimer_StartsTimer()
        {
            //Arrage
            _viewModel.Url = _validUrl;
            _viewModel.Time = 6;

            //Act
            _viewModel.Start();

            //Assert
            Assert.IsTrue(_viewModel.Timer.Interval == TimeFromSeconds(_viewModel.Time));
            Assert.IsTrue(_viewModel.Timer.Enabled);
        }

        private double TimeFromSeconds(double seconds)
        {
            return seconds * 1000 * 60;
        } 

        private void ZeroMessageBoxCallList()
        {
            _messageBoxCallList = new Dictionary<Command, int>
            {
                {Command.Start, 0 },
                {Command.Stop, 0 },
                {Command.Reset, 0 },
                {Command.InvalidUrl, 0 },
                {Command.ValidUrl, 0 },
            };
        }

        private void ZeroLogPanelCallList()
        {
            _logPanelCallList = new Dictionary<Command, int>
            {
                {Command.Start, 0 },
                {Command.Stop, 0 },
                {Command.Reset, 0 },
                {Command.InvalidUrl, 0 },
                {Command.ValidUrl, 0 },
            };
        }
    }
}
