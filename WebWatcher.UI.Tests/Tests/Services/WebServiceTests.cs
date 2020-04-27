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
    public class WebServiceTests
    {
        private string _exceptionUrl = "exception";
        private string _validUrl = "valid";
        private string _invalidUrl = "invalid";
        private Response _response;
        private WebService _service;

        public WebServiceTests()
        {
            _response = new Response
            {
                Url = _validUrl,
                DateTime = new DateTime(2020, 4, 23, 21, 32, 19),
                State = State.Ok,
                Status = HttpStatusCode.OK
            };

            var mockValidator = new Mock<IUrlValidator>();
            mockValidator.Setup(x => x.IsUrlValid(_validUrl)).Returns(true);
            mockValidator.Setup(x => x.IsUrlValid(_invalidUrl)).Returns(false);
            mockValidator.Setup(x => x.IsUrlValid(_exceptionUrl)).Returns(true);

            var mockReader = new Mock<IWebReader>();
            mockReader.Setup(x => x.Get(_validUrl)).Returns(_response);
            mockReader.Setup(x => x.Get(_exceptionUrl)).Throws(new Exception("Test"));

            _service = new WebService(mockReader.Object, mockValidator.Object);
        }

        [Test]
        public void CheckPage_ValidUrlPassed_ServerResponded_ReturnsResultWithResponse()
        {
            //Arrange
            //Act
            var result = _service.CheckPage(_validUrl);

            //Assert
            Assert.IsTrue(result.IsFine);
            Assert.NotNull(result.Output);
            Assert.Null(result.Exception);
            Assert.AreEqual(result.Output, _response);
        }

        [Test]
        public void CheckPage_InvalidUrlPassed_ReturnsResultWithException()
        {
            //Arrange
            //Act
            var result = _service.CheckPage(_invalidUrl);

            //Assert
            Assert.IsFalse(result.IsFine);
            Assert.Null(result.Output);
            Assert.NotNull(result.Exception);
            Assert.IsTrue(result.Exception is ArgumentException);
        }

        [Test]
        public void CheckPage_ValidUrlPassed_ExceptionOnResponseThrown_ReturnsResultWithException()
        {
            //Arrange
            //Act
            var result = _service.CheckPage(_exceptionUrl);

            //Assert
            Assert.IsFalse(result.IsFine);
            Assert.Null(result.Output);
            Assert.NotNull(result.Exception);
            Assert.IsTrue(result.Exception is Exception);
            Assert.IsTrue(result.Exception.Message == "Test");
        }

        [Test]
        public void CheckPage_EmptyUrlPassed_ReturnsResultWithException()
        {
            //Arrange
            //Act
            var result = _service.CheckPage(string.Empty);

            //Assert
            Assert.IsFalse(result.IsFine);
            Assert.Null(result.Output);
            Assert.NotNull(result.Exception);
            Assert.IsTrue(result.Exception is ArgumentException);
        }

        [Test]
        public void CheckPage_NullUrlPassed_ReturnsResultWithException()
        {
            //Arrange
            //Act
            var result = _service.CheckPage(null);

            //Assert
            Assert.IsFalse(result.IsFine);
            Assert.Null(result.Output);
            Assert.NotNull(result.Exception);
            Assert.IsTrue(result.Exception is ArgumentException);
        }
    }
}
