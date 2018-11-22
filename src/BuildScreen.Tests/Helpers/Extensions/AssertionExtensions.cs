using NUnit.Framework;

namespace BuildScreen.Tests.Helpers.Extensions
{
    public static class AssertionExtensions
    {
        public static void ShouldNotBeNull(this object actual)
        {
            Assert.IsNotNull(actual);
        }

        public static void ShouldBeEqual(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldNotBeEqual(this object actual, object expected)
        {
            Assert.AreNotEqual(expected, actual);
        }

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.AreSame(expected, actual);
        }

        public static void ShouldNotBeTheSameAs(this object actual, object expected)
        {
            Assert.AreNotSame(expected, actual);
        }

        public static void ShouldBeNull(this object actual)
        {
            Assert.IsNull(actual);
        }

        public static void ShouldBeNullOrEmpty(this string actual)
        {
            Assert.IsNullOrEmpty(actual);
        }

        public static void ShouldNotBeNullOrEmpty(this string actual)
        {
            Assert.IsNotNullOrEmpty(actual);
        }

        public static void ShouldBeEmpty(this string actual)
        {
            Assert.IsEmpty(actual);
        }

        public static void ShouldBeFalse(this bool value)
        {
            Assert.IsFalse(value);
        }

        public static void ShouldBeTrue(this bool value)
        {
            Assert.IsTrue(value);
        }

        public static void ShouldBe<T>(this object obj)
        {
            Assert.IsInstanceOf<T>(obj);
        }

        public static void ShouldBeAssignableFrom<T>(this object obj)
        {
            Assert.IsAssignableFrom(typeof(T), obj);
        }
    }
}
