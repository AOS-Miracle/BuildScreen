using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;

#if DEBUG
[assembly: AssemblyTitle("BuildScreen (DEBUG)")]
#else // RELEASE
[assembly: AssemblyTitle("BuildScreen")]
#endif

[assembly: AssemblyDescription("")]
[assembly: AssemblyCulture("")]

[assembly: NeutralResourcesLanguage("en-US")]

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly
)]

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
