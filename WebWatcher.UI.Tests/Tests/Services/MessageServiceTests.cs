using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.Interfaces;
using WebWatcher.UI.Models;
using WebWatcher.UI.Services;

namespace WebWatcher.UI.Tests.Tests.Services
{
    [TestFixture]
    public class MessageServiceTests
    {
        private DateTime _now;
        private MessageService _service;
        private Response _validResponse_Error;
        private Response _validResponse_Warning;
        private Response _validResponse_Ok;
        public MessageServiceTests()
        {
            _now = new DateTime(2020, 4, 23, 21, 10, 19);

            var dateTimeServiceMock = new Mock<IDateTimeSerivce>();
            dateTimeServiceMock.Setup(x => x.GetNow()).Returns(_now);

            _service = new MessageService(dateTimeServiceMock.Object);

            _validResponse_Error = new Response
            {
                State = State.Error,
                Status = HttpStatusCode.InternalServerError,
                DateTime = new DateTime(1993, 7, 2, 2, 20, 0),
                Url = "https://www.zalando.pl"
            };

            _validResponse_Warning = new Response
            {
                State = State.Warning,
                Status = HttpStatusCode.Ambiguous,
                DateTime = new DateTime(1993, 11, 10, 20, 0, 15),
                Url = "https://www.youtube.com"
            };

            _validResponse_Ok = new Response
            {
                State = State.Ok,
                Status = HttpStatusCode.OK,
                DateTime = new DateTime(1991, 8, 23, 6, 6, 6),
                Url = "https://www.github.com"
            };
        }

        [Test]
        public void ResponseToText_ValidResponsePassed_WithErrorState_ReturnsResultWithErrorMessage()
        {
            //Arrange
            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[02:20:00] " +
                    @"\cf1 [https://www.zalando.pl] " +
                    @"\cf3 \b [InternalServerError] \b0" +
                    @"\par}";

            //Act
            var result = _service.ResponseToText(_validResponse_Error);

            //Assert
            Assert.AreEqual(expected, result.Output);
        }

        [Test]
        public void ResponseToText_ValidResponsePassed_WithWarningState_ReturnsResultWithWarningMessage()
        {
            //Arrange
            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[20:00:15] " +
                    @"\cf1 [https://www.youtube.com] " +
                    @"\cf4 \b [MultipleChoices] \b0" +
                    @"\par}";

            //Act
            var result = _service.ResponseToText(_validResponse_Warning);

            //Assert
            Assert.AreEqual(expected, result.Output);
        }

        [Test]
        public void ResponseToText_ValidResponsePassed_WithOkState_ReturnsResultWithOkMessage()
        {
            //Arrange
            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[06:06:06] " +
                    @"\cf1 [https://www.github.com] " +
                    @"\cf2 \b [OK] \b0" +
                    @"\par}";

            //Act
            var result = _service.ResponseToText(_validResponse_Ok);

            //Assert
            Assert.AreEqual(expected, result.Output);
        }

        [Test]
        public void ResponseToText_ResponseWithoutUrlPassed_WithOkState_ReturnsResultWithOkMessageWithoutUrl()
        {
            //Arrange
            var copy = _validResponse_Ok.Clone();
            copy.Url = string.Empty;

            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[06:06:06] " +
                    @"\cf1 [] " +
                    @"\cf2 \b [OK] \b0" +
                    @"\par}";

            //Act
            var result = _service.ResponseToText(copy);

            //Assert
            Assert.AreEqual(expected, result.Output);
        }

        [Test]
        public void ResponseToText_NullPassed_ReturnsResultWithNullWithException()
        {
            //Arrange
            //Act
            var result = _service.ResponseToText(null);

            //Assert
            Assert.IsFalse(result.IsFine);
            Assert.IsNull(result.Output);
            Assert.IsTrue(result.Exception is ArgumentNullException);
        }

        [Test]
        public void CommandToText_ResetCommandPassed_ReturnsResetMessage()
        {
            //Arrange
            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[21:10:19] " +
                    @"\cf1 [Timer Reset] " +
                    @"\cf1 \b [---Timer resetted by user---] \b0" +
                    @"\par}";

            //Act
            var result = _service.CommandToText(Command.Reset);

            //Assert
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CommandToText_StartCommandPassed_ReturnsStartMessage()
        {
            //Arrange
            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[21:10:19] " +
                    @"\cf1 [Timer Start] " +
                    @"\cf1 \b [---Timer on---] \b0" +
                    @"\par}";

            //Act
            var result = _service.CommandToText(Command.Start);

            //Assert
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CommandToText_StopCommandPassed_ReturnsResetMessage()
        {
            //Arrange
            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[21:10:19] " +
                    @"\cf1 [Timer Stop] " +
                    @"\cf1 \b [---Timer off---] \b0" +
                    @"\par}";

            //Act
            var result = _service.CommandToText(Command.Stop);

            //Assert
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CommandToText_InvalidUrlCommandPassed_ReturnsEmptyMessage()
        {
            //Arrange
            var expected = string.Empty;

            //Act
            var result = _service.CommandToText(Command.InvalidUrl);

            //Assert
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CommandToText_ValidUrlCommandPassed_ReturnsEmptyMessage()
        {
            //Arrange
            var expected = string.Empty;

            //Act
            var result = _service.CommandToText(Command.ValidUrl);

            //Assert
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void GetError_MessagePassed_ReturnsErrorMessage()
        {
            //Arrange
            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[21:10:19] " +
                    @"\cf1 [Internal error] " +
                    @"\cf1 \b [My test message] \b0" +
                    @"\par}";

            //Act
            var result = _service.GetError("My test message");

            //Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetError_NullPassed_ReturnsDefaultErrorMessage()
        {
            //Arrange
            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[21:10:19] " +
                    @"\cf1 [Internal error] " +
                    @"\cf1 \b [An internal error occured.] \b0" +
                    @"\par}";

            //Act
            var result = _service.GetError(null);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetError_EmptyStringPassed_ReturnsDefaultErrorMessage()
        {
            //Arrange
            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[21:10:19] " +
                    @"\cf1 [Internal error] " +
                    @"\cf1 \b [An internal error occured.] \b0" +
                    @"\par}";

            //Act
            var result = _service.GetError(string.Empty);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetError_NothingPassed_ReturnsDefaultErrorMessage()
        {
            //Arrange
            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[21:10:19] " +
                    @"\cf1 [Internal error] " +
                    @"\cf1 \b [An internal error occured.] \b0" +
                    @"\par}";

            //Act
            var result = _service.GetError();

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
