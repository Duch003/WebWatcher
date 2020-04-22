using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.Validators;

namespace WebWatcher.UI.Tests.Tests.Validators
{
    [TestFixture]
    public class UrlValidatorTests
    {
        private UrlValidator _validator;
        private string _validUrl_https;
        private string _validUrl_http;
        private string _validUrl_noProtocol;
        private string _invalidUrl;
        
        public UrlValidatorTests()
        {
            
            _validator = new UrlValidator();
            _validUrl_https = "https://www.wp.pl";
            _validUrl_http = "http://www.http2demo.io/";
            _validUrl_noProtocol = "www.onet.pl";
            _invalidUrl = "nonexistingpage";
        }

        [Test]
        public void IsUrlValid_ValidStringPassed_ReturnsTrue()
        {
            Assert.IsTrue(_validator.IsUrlValid(_validUrl_http));
            Assert.IsTrue(_validator.IsUrlValid(_validUrl_https));
        }

        [Test]
        public void IsUrlValid_InvalidStringPassed_ReturnsFalse()
        {
            Assert.IsFalse(_validator.IsUrlValid(_invalidUrl));
        }

        [Test]
        public void IsUrlValid_StringWithoutProtocolPassed_ReturnsFalse()
        {
            Assert.IsFalse(_validator.IsUrlValid(_validUrl_noProtocol));
        }

        [Test]
        public void IsUrlValid_EmptyStringPassed_ReturnsFalse()
        {
            Assert.IsFalse(_validator.IsUrlValid(string.Empty));
        }

        [Test]
        public void IsUrlValid_NullPassed_ReturnsFalse()
        {
            Assert.IsFalse(_validator.IsUrlValid(null));
        }
    }
}
