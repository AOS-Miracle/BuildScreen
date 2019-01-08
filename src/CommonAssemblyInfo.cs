using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

[assembly: AssemblyProduct("BuildScreen")]
// this name is used in the folder "C:\Users\YourUserName\AppData\Local\[->BuildScreen<-]" to store the settings
[assembly: AssemblyCompany("BuildScreen")]

[assembly: AssemblyCopyright("Copyright (c) 2018")]
[assembly: AssemblyTrademark("")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else // RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif

// the following PublicKey is the one from the BuildScreen.Tests project
[assembly: InternalsVisibleTo("BuildScreen.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100658EC8E61F118140A90BBE21A425D6525DB5ED9D1CAC7FADA2AEAC45E55DE2A4536A07191D998FA476DFB09358783B8C3B31C551C894AC4B0BB545279BCA7800E58FB273171B0D898ACD053C90E156BE188EA85C81316634F52FC9CBD8C4DEDFA73CACC31463324E82E4B46BB9B0B21504B06AEF8BC34A61827434525C1B1BB6")]

[assembly: AssemblyVersion("0.2.4.0")]
[assembly: AssemblyFileVersion("0.2.4.0")]
