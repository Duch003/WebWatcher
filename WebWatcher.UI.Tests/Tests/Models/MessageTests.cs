using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.Models;
using WebWatcher.UI.Tests.Classes;

namespace WebWatcher.UI.Tests.Tests.Models
{
    [TestFixture]
    public class MessageTests
    {
        private object _testObject;

        public MessageTests()
        {
            _testObject = new object();
        }

        [Test]
        public void Constructor_ValuePassedIntoConstrutor_AccessibleViaValueProp()
        {
            var message = new Message<IMockInterface1, object>(_testObject);

            Assert.IsTrue(_testObject == message.Value);
        }

        [Test]
        public void Constructor_NullPassedIntoConstrutor_AccessibleViaValueProp()
        {
            var message = new Message<IMockInterface1, object>(null);

            Assert.IsNull(message.Value);
        }
    }
}
