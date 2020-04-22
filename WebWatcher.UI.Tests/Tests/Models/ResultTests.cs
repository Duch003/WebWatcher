using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.Tests.Tests.Models
{
    [TestFixture]
    public class ResultTests
    {
        private object _testObject;

        public ResultTests()
        {
            _testObject = new object();
        }

        [Test]
        public void Constructor_ExceptionIsNull_IsFinePropIsTrue()
        {
            var result = new Result<object>(_testObject);

            Assert.IsTrue(result.IsFine);
        }

        [Test]
        public void Constructor_ExceptionIsNotNull_IsFinePropIsFalse()
        {
            var result = new Result<object>(null, new Exception("Test message"));

            Assert.IsFalse(result.IsFine);
        }

        [Test]
        public void Constructor_ObjectPassedIntoConstructor_AccessibleViaOutputProp()
        {
            var result = new Result<object>(_testObject);

            Assert.IsNotNull(result.Output);
            Assert.IsTrue(result.Output == _testObject);
        }

        [Test]
        public void Constructor_ExceptionPassedIntoConstructor_AccessibleViaExceptionProp()
        {
            var result = new Result<object>(null, new Exception("Test message"));

            Assert.IsNotNull(result.Exception);
            Assert.IsTrue(result.Exception.Message == "Test message");
        }

        [Test]
        public void Constructor_NullResultPassedIntoConstructor_OutputPropIsNull()
        {
            var result = new Result<object>(null);

            Assert.IsNull(result.Output);
        }

        [Test]
        public void Constructor_NullExceptionPassedIntoConstructor_ExceptionPropIsNull()
        {
            var result = new Result<object>(_testObject, null);

            Assert.IsNull(result.Exception);
        }
    }
}
