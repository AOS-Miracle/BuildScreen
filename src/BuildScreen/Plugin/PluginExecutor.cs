using System;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using BuildScreen.ContinousIntegration.Entities;
using BuildScreen.Plugin.Configuration;
using BuildScreen.Resources;
using log4net;

namespace BuildScreen.Plugin
{
    [Serializable]
    public class PluginExecutor : IDisposable, IPluginExecutor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PluginExecutor));
        private IBuildScreenPlugin _plugin;
        private bool _isDisposed;
        private AppDomain _pluginSandbox;

        public PluginExecutor(Type type)
        {
            ValidatePlugin(type);
            CreatePlugin(type);
        }

        public void StartExecutePlugins(Build build)
        {
            ParameterizedThreadStart threadStart = ExecAsync;
            threadStart.BeginInvoke(build, null, null);
        }

        private void CreatePlugin(Type type)
        {
            AppDomainSetup domaininfo = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
            };

            Evidence evidence = AppDomain.CurrentDomain.Evidence;
            _pluginSandbox = AppDomain.CreateDomain("PluginSandbox", evidence, domaininfo);
            _pluginSandbox.DomainUnload += ((sender, args) => _pluginSandbox = null);
            _plugin = (IBuildScreenPlugin)_pluginSandbox.CreateInstanceFromAndUnwrap(type.Assembly.Location, type.FullName);
        }

        private void ExecAsync(object build)
        {
            try
            {
                _plugin.Start(CreateExternalBuild(build as Build));
            }
            catch (Exception ex)
            {
                if (Log.IsWarnEnabled)
                    Log.Warn(string.Format("Exeption thrown when executing plugin {0}", _plugin.Name), ex);
            }

        }

        private static BuildScreenBuild CreateExternalBuild(Build build)
        {
            return new BuildScreenBuild
                {
                    FinishDate = build.FinishDate,
                    Number = build.Number,
                    ProjectName = build.ProjectName,
                    StartDate = build.StartDate,
                    Status = build.Status.ToString(),
                    StatusText = build.StatusText,
                    TypeId = build.UniqueIdentifier,
                    TypeName = build.TypeName
                };
        }

        public virtual void Dispose()
        {
            if (!_isDisposed)
            {
                DoCleanup();
                TearDownPluginSandbox();

                _isDisposed = true;
            }
        }

        private void TearDownPluginSandbox()
        {
            if (_pluginSandbox == null) return;

            try
            {
                AppDomain.Unload(_pluginSandbox);
            }
            catch (CannotUnloadAppDomainException ex) // Usually this means that the domain has alread been unloaded
            {
                if (Log.IsWarnEnabled)
                    Log.Warn(string.Format("CannotUnloadAppDomainException thrown when tearing down plugin {0}", _plugin.Name), ex);
            }

            _pluginSandbox = null;
        }

        private void DoCleanup()
        {
            try
            {
                _plugin.CleanUp();
            }
            catch (Exception ex)
            {
                if (Log.IsWarnEnabled)
                    Log.Warn(string.Format("Exception thrown when cleaning up in plugin {0}", _plugin.Name), ex);
            }
        }

        private static void ValidatePlugin(Type pluginType)
        {
            PluginConfigurationSection plugins = ConfigurationManager.GetSection("pluginConfiguration") as PluginConfigurationSection;

            if (plugins == null)
            {
                throw new ArgumentException(
                    string.Format(InternalResources.PluginCouldNotFindConfigurationSection, "pluginConfiguration")
                    );
            }

            PluginElement plugin = plugins.Plugins.FirstOrDefault(p => pluginType.FullName.Equals(p.TypeStr, StringComparison.OrdinalIgnoreCase));

            if (plugin == null)
            {
                throw new ArgumentException(
                    string.Format(InternalResources.PluginCouldNotFindPluginByName, pluginType.Name)
                    );
            }
        }
    }
}
