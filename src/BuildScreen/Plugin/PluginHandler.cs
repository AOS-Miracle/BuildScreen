using System.Configuration;
using System.Linq;
using BuildScreen.ContinousIntegration.Entities;
using BuildScreen.Plugin.Configuration;

namespace BuildScreen.Plugin
{
    public class PluginHandler
    {
        public void TriggerPlugins(Build oldBuild, Build newBuild)
        {
            if (oldBuild.Status.Equals(newBuild.Status)) return;

            PluginConfigurationSection plugins = ConfigurationManager.GetSection("pluginConfiguration") as PluginConfigurationSection;

            foreach (PluginExecutor pluginExecutor in
                plugins.Plugins.Select(pluginElement => new PluginExecutor(pluginElement.PluginType)))
            {
                pluginExecutor.StartExecutePlugins(newBuild);
            }
        }
    }
}
