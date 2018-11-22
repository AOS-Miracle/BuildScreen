using System;
using BuildScreen.Core.Extensions;
using BuildScreen.Tests.Helpers.Extensions;
using NUnit.Framework;

namespace BuildScreen.Tests.Core.Extensions
{
    [TestFixture]
    public class DoubleExtensionsTest
    {
        [Test]
        public void ToDateTimeFromUnixTimestamp_DoubleIsZero_ShouldMatchExpectedDateTime()
        {
            const double zero = 0;
            zero.ToDateTimeFromUnixTimestamp().ShouldBeEqual(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        }

        [Test]
        public void ToDateTimeFromUnixTimestamp_DoubleIsTwoThousand_ShouldMatchExpectedDateTime()
        {
            const double zero = 2000;
            zero.ToDateTimeFromUnixTimestamp().ShouldBeEqual(new DateTime(1970, 1, 1, 0, 0, 2, 0));
        }
    }
}
