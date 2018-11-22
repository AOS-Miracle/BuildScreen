using System;
using BuildScreen.Plugin;
using BuildScreen.Plugin.Configuration;
using NUnit.Framework;

namespace BuildScreen.Tests.Plugin.Configuration
{
    [TestFixture]
    public class PluginElementTest
    {
        [Test]
        [Ignore]
        public void Should_return_correct_type()
        {
            PluginElement pluginElement = new PluginElement();
            pluginElement.TypeStr = "BuildScreen.Tests.Core.Plugin.Configuration.Dummyplugin, BuildScreen.Tests";

            Assert.That(pluginElement.PluginType, Is.EqualTo(new Dummyplugin().GetType()));
        }

        [Test]
        [Ignore]
        [ExpectedException(typeof(PluginTypeInitializationException), ExpectedMessage = "'BuildScreen.Tests.Core.Plugin.Configuration.DummypluginNotExistingType, BuildScreen.Tests' could not be found. Check that the type has been typed correctly and the type really exists.")]
        public void Should_not_be_able_to_create_type_from_invalid_type_string()
        {
            PluginElement pluginElement = new PluginElement();
            pluginElement.TypeStr = "BuildScreen.Tests.Core.Plugin.Configuration.DummypluginNotExistingType, BuildScreen.Tests";
            pluginElement.Name = "DummyName";

            Assert.That(pluginElement.PluginType, Is.EqualTo(new Dummyplugin().GetType()));

        }

        [Test]
        [Ignore]
        [ExpectedException(typeof(PluginNotSupportedException), ExpectedMessage = "Only plugins that implement IBuildScreenPlugin can be added under the plugin section.")]
        public void Should_not_be_able_to_create_type_which_dont_implement_IBuildScreenPlugin()
        {
            PluginElement pluginElement = new PluginElement();
            pluginElement.TypeStr = "BuildScreen.Tests.Core.Plugin.Configuration.Dummyplugin";
            pluginElement.Name = "BuildScreen.Tests.dll";

            //pluginElement.TypeStr = "BuildScreen.Tests.Core.Plugin.Configuration.PluginElementTests, BuildScreen.Tests";

            Assert.That(pluginElement.PluginType, Is.EqualTo(new Dummyplugin().GetType()));

        }
    }


    public class Dummyplugin : IBuildScreenPlugin
    {
        public bool Start(IBuildScreenBuild build)
        {
            throw new NotImplementedException();
        }

        public bool Kill(IBuildScreenBuild build)
        {
            throw new NotImplementedException();
        }

        public void CleanUp()
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
