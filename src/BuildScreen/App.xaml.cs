using System;
using System.Collections.Generic;
using System.Windows;
using BuildScreen.Core.Shell;

namespace BuildScreen
{
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "F46D2B3FFBCB4c5aA1C746B37A7AB6E8";

        [STAThread]
        public static void Main()
        {
            try
            {
                if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
                {
                    App application = new App();

                    application.InitializeComponent();
                    application.Run();

                    // Allow single instance code to perform cleanup operations
                    SingleInstance<App>.Cleanup();
                }
            }
            catch(Exception ie)
            {
                MessageBox.Show(ie.Message);
            }
            
        }

        #region Implementation of ISingleInstanceApp

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }

        #endregion
    }
}
