using System;
using BuildScreen.Core.Security;
using BuildScreen.Tests.Helpers.Extensions;
using NUnit.Framework;

namespace BuildScreen.Tests.Core.Security
{
    [TestFixture]
    public class SecureDataTest
    {
        [Test]
        public void Encrypt_DecryptedDataIsNull_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SecureData.Encrypt(null));
        }

        [Test]
        public void Decrypt_EncryptedDataIsNull_ShouldBeNull()
        {
            SecureData.Decrypt(null).ShouldBeNull();
        }

        [Test]
        public void Decrypt_EncryptedString_ShouldMatchExpectedString()
        {
            string encryptedString = SecureData.Encrypt("Password");
            SecureData.Decrypt(encryptedString).ShouldBeEqual("Password");
        }
    }
}
