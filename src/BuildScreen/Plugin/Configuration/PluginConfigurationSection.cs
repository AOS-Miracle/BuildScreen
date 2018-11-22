using System.Xml.Serialization;

namespace BuildScreen.Plugin.Configuration
{
    [XmlRoot(ElementName = "pluginConfiguration")]
    public class PluginConfigurationSection
    {
        [XmlArray(ElementName = "plugins")]
        [XmlArrayItem(ElementName = "plugin", Type = typeof(PluginElement))]
        public PluginElement[] Plugins { get; set; }
    }
}
