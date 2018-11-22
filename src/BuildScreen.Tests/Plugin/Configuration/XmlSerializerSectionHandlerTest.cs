using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using BuildScreen.Plugin.Configuration;
using NUnit.Framework;

namespace BuildScreen.Tests.Plugin.Configuration
{
    [TestFixture]
    public class XmlSerializerSectionHandlerTest
    {
        private XmlDocument _xDoc;

        [SetUp]
        public void Setup()
        {
            _xDoc = new XmlDocument();
        }

        [Test]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void Should_not_allow_section_type_to_be_invalid()
        {
            _xDoc.LoadXml("<root />");

            new XmlSerializerSectionHandler().Create(null, null, _xDoc.FirstChild);
        }

        [Test]
        public void Should_serialize_xml_into_specified_type()
        {
            string markup = string.Format("<root type='{0}' foo='Bar' />", (typeof(DummyConfigurationSection).AssemblyQualifiedName));
            _xDoc.LoadXml(markup);

            DummyConfigurationSection result = (DummyConfigurationSection)new XmlSerializerSectionHandler().Create(null, null, _xDoc.FirstChild);

            Assert.That(result.Foo, Is.EqualTo("Bar"));
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Should_not_throw_serlialization_exceptions_when_configuration_section_instance_throws()
        {
            string markup = string.Format("<root type='{0}' foo='Bar' />", (typeof(ThrowingConfigurationSection).AssemblyQualifiedName));
            _xDoc.LoadXml(markup);

            new XmlSerializerSectionHandler().Create(null, null, _xDoc.FirstChild);
        }
    }

    [XmlRoot(ElementName = "root")]
    public class DummyConfigurationSection
    {
        [XmlAttribute(AttributeName = "foo")]
        public string Foo { get; set; }
    }

    [XmlRoot(ElementName = "root")]
    public class ThrowingConfigurationSection
    {
        [XmlAttribute(AttributeName = "foo")]
        public string Foo { get { return null; } set { throw new NotImplementedException(); } }
    }    
}
