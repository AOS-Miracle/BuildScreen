using BuildScreen.Core.Utilities;
using BuildScreen.Tests.Helpers.Extensions;
using NUnit.Framework;

namespace BuildScreen.Tests.Core.Utilities
{
    [TestFixture]
    public class ValidateTest
    {
        [Test]
        public void IsNumeric_StringIsNull_ShouldBeFalse()
        {
            Validate.IsNumeric(null).ShouldBeFalse();
        }

        [Test]
        public void IsNumeric_StringIsEmpty_ShouldBeFalse()
        {
            Validate.IsNumeric(string.Empty).ShouldBeFalse();
        }

        [Test]
        public void IsNumeric_InvalidNumbers_ShouldBeFalse()
        {
            Validate.IsNumeric("-1").ShouldBeFalse();
            Validate.IsNumeric("0,1").ShouldBeFalse();
            Validate.IsNumeric("0.1").ShouldBeFalse();
            Validate.IsNumeric("a").ShouldBeFalse();
        }

        [Test]
        public void IsNumeric_ValidNumbers_ShouldBeTrue()
        {
            Validate.IsNumeric("0").ShouldBeTrue();
            Validate.IsNumeric("0123456789012345789").ShouldBeTrue();
        }

        [Test]
        public void IsDomain_StringIsNull_ShouldBeFalse()
        {
            Validate.IsDomain(null).ShouldBeFalse();
        }

        [Test]
        public void IsDomain_StringIsEmpty_ShouldBeFalse()
        {
            Validate.IsDomain(string.Empty).ShouldBeFalse();
        }

        [Test]
        public void IsDomain_InvalidDomains_ShouldBeFalse()
        {
            Validate.IsDomain(" ").ShouldBeFalse();
            Validate.IsDomain("http://").ShouldBeFalse();
            Validate.IsDomain(".com").ShouldBeFalse();
            Validate.IsDomain("foo.1").ShouldBeFalse();
            Validate.IsDomain("www.foo.com/bar").ShouldBeFalse();
        }

        [Test]
        public void IsDomain_ValidDomains_ShouldBeTrue()
        {
            Validate.IsDomain("localhost").ShouldBeTrue();
            Validate.IsDomain("www.foo.com").ShouldBeTrue();
            Validate.IsDomain("bar.foo.com").ShouldBeTrue();
            Validate.IsDomain("foo.com").ShouldBeTrue();
            Validate.IsDomain("foo1.com").ShouldBeTrue();
            Validate.IsDomain("äüö.com").ShouldBeTrue();
        }

        [Test]
        public void IsIpAddress_StringIsNull_ShouldBeFalse()
        {
            Validate.IsIpAddress(null).ShouldBeFalse();
        }

        [Test]
        public void IsIpAddress_StringIsEmpty_ShouldBeFalse()
        {
            Validate.IsIpAddress(string.Empty).ShouldBeFalse();
        }

        [Test]
        public void IsIpAddress_InvalidIpAddresses_ShouldBeFalse()
        {
            Validate.IsIpAddress("65536").ShouldBeFalse();
            Validate.IsIpAddress("aaa.aaa.aaa.aaa").ShouldBeFalse();
            Validate.IsIpAddress("255.255.255.256").ShouldBeFalse();
            Validate.IsIpAddress("255.255.255").ShouldBeFalse();
            Validate.IsIpAddress("0.0.0.0").ShouldBeFalse();
        }

        [Test]
        public void IsIpAddress_ValidIpAddresses_ShouldBeTrue()
        {
            Validate.IsIpAddress("1.0.0.0").ShouldBeTrue();
            Validate.IsIpAddress("255.255.255.255").ShouldBeTrue();
        }
    }
}
