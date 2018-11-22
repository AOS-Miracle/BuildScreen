using BuildScreen.ContinousIntegration.Entities;

namespace BuildScreen.Plugin
{
    public interface IPluginExecutor
    {
        void StartExecutePlugins(Build build);
    }
}