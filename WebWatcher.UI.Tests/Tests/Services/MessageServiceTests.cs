using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.Models;
using WebWatcher.UI.Services;

namespace WebWatcher.UI.Tests.Tests.Services
{
    [TestFixture]
    public class MessageServiceTests
    {
        private MessageService _service;
        private Response _validResponse_Error;
        private Response _validResponse_Warning;
        private Response _validResponse_Ok;
        public MessageServiceTests()
        {
            _service = new MessageService();

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
        public void ResponseToText_ValidResponsePassed_WithErrorState_ReturnsErrorMessage()
        {
            var result = _service.ResponseToText(_validResponse_Error);
        
            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[02:20:00] " +
                    @"\cf1 [https://www.zalando.pl] " +
                    @"\cf3 \b [InternalServerError] \b0" +
                    @"\par}";

            Assert.AreEqual(expected, result.Output);
        }

        [Test]
        public void ResponseToText_ValidResponsePassed_WithWarningState_ReturnsWarningMessage()
        {
            var result = _service.ResponseToText(_validResponse_Warning);

            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[20:00:15] " +
                    @"\cf1 [https://www.youtube.com] " +
                    @"\cf4 \b [MultipleChoices] \b0" +
                    @"\par}";

            Assert.AreEqual(expected, result.Output);
        }

        [Test]
        public void ResponseToText_ValidResponsePassed_WithOkState_ReturnsOkMessage()
        {
            var result = _service.ResponseToText(_validResponse_Ok);

            var expected = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[06:06:06] " +
                    @"\cf1 [https://www.github.com] " +
                    @"\cf2 \b [OK] \b0" +
                    @"\par}";

            Assert.AreEqual(expected, result.Output);
        }
    }
}
