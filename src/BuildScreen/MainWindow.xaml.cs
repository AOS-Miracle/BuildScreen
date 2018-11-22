using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BuildScreen.ContinousIntegration;
using BuildScreen.ContinousIntegration.Entities;
using BuildScreen.ContinousIntegration.Exceptions;
using BuildScreen.ContinousIntegration.Persistance;
using BuildScreen.Core.Security;
using BuildScreen.Core.Utilities;
using BuildScreen.Plugin;
using BuildScreen.Properties;
using BuildScreen.Resources;

namespace BuildScreen
{
    public partial class MainWindow
    {
        readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private PluginHandler _pluginHandler;
        private IClientFactory _clientFactory;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShortcutsInfo.Text = InternalResources.ShortcutInfoEnterFullScreen;

            _clientFactory = new ClientFactory();
            _pluginHandler = new PluginHandler();

            ConnectAndRender();

            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, Settings.Default.RefreshInterval);
            _dispatcherTimer.Start();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                Topmost = true;

                ShortcutsInfo.Text = InternalResources.ShortcutInfoExitFullScreen;
            }

            if (e.Key == Key.Escape)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                Topmost = false;

                ShortcutsInfo.Text = InternalResources.ShortcutInfoEnterFullScreen;
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
            {
                _dispatcherTimer.Stop();

                foreach (Grid grid in UniformGridBuilds.Children.OfType<Grid>())
                {
                    grid.Background = Graphics.BrushNeutral;
                }

                OptionsWindow optionsDialog = new OptionsWindow();
                optionsDialog.Closing += OptionsWindow_Closing;
                optionsDialog.ShowDialog();
            }
        }

        private void OptionsWindow_Closing(object sender, CancelEventArgs e)
        {
            ConnectAndRender();

            _dispatcherTimer.Interval = new TimeSpan(0, 0, Settings.Default.RefreshInterval);
            _dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            ConnectAndRender();
        }

        private void ConnectAndRender()
        {
            UniformGridBuilds.Children.Clear();
            UniformGridBuilds.Columns = Settings.Default.NumberOfColumns;

            ClientConfiguration clientConfiguration = LoadClientConfiguration();

            if (!clientConfiguration.AllDataExists)
                return;
 
            try
            {
                using (IClient client = _clientFactory.CreateClient(clientConfiguration))
                {
                    if (client.IsConnected)
                    {
                        foreach (Build storedBuild in Settings.Default.Builds)
                        {
                            try
                            {
                                Build build = client.BuildByUniqueIdentifier(storedBuild.UniqueIdentifier);

                                _pluginHandler.TriggerPlugins(storedBuild, build);

                                int daysLastBuildHappened = DateTime.Now.Subtract(build.StartDate).Days;

                                if (!Settings.Default.HideInactive || Settings.Default.HideInactive && daysLastBuildHappened < Settings.Default.HideInactiveWeeks * 7)
                                {
                                    LoadGridWithBuilds(build);
                                }
                            }
                            catch (ClientLoadDocumentException)
                            {
                                // TODO: One build could not get loaded via typeId, maybe it got deleted but is still in the settings; Display error message
                            }
                        }
                    }
                }
            }
            catch (ClientConnectionException)
            {
                // TODO: Display error message
            }
        }

        private void LoadGridWithBuilds(Build build)
        {
            StackPanel stackPanel = new StackPanel();
 
            stackPanel.Children.Insert(0, new TextBlock
                                              {
                                                  Text = string.IsNullOrEmpty(build.ProjectName) ? build.TypeName : string.Format("{0}, {1}", build.ProjectName, build.TypeName),
                                                  FontSize = 24,
                                                  Foreground = new SolidColorBrush(Colors.White),
                                                  Padding = new Thickness(10, 0, 10, 0)
                                              });

            stackPanel.Children.Insert(1, new TextBlock
                                              {
                                                  Text = string.IsNullOrEmpty(build.ProjectName) ? string.Format("Build {0}", build.Number) : string.Format("Build {0}, {1}", build.Number, build.StatusText),
                                                  FontSize = 12,
                                                  Foreground = new SolidColorBrush(Colors.White),
                                                  Padding = new Thickness(10, 0, 10, 6)
                                              });

            Viewbox viewbox = new Viewbox();
            viewbox.HorizontalAlignment = HorizontalAlignment.Left;
            viewbox.Child = stackPanel;

            Grid grid = new Grid();
            grid.Background = build.Status == Status.Success ? Graphics.BrushSuccess : Graphics.BrushFailure;
            grid.Tag = build;

            if(build.Running)
                DoAnimationRectangle(grid);

            grid.Children.Insert(0, viewbox);

            UniformGridBuilds.Children.Insert(0, grid);
        }

        private static void DoAnimationRectangle(Grid g)
        {
            // Create a DoubleAnimation to animate the width of the button.
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 1;
            myDoubleAnimation.To = 0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(2000));
            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTarget(myDoubleAnimation, g);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Grid.OpacityProperty));

            Storyboard myAnimatedGridStoryboard = new Storyboard();
            myAnimatedGridStoryboard.Children.Add(myDoubleAnimation);

            myAnimatedGridStoryboard.Begin();
        }

        private static ClientConfiguration LoadClientConfiguration()
        {
            string baseUrl = string.Empty;

            switch (Settings.Default.ClientType)
            {
                case ClientType.TeamCity:
                    baseUrl = Settings.Default.BaseUrlTeamCity;
                    break;
                case ClientType.Hudson:
                    baseUrl = Settings.Default.BaseUrlHudson;
                    break;
                case ClientType.Bamboo:
                    baseUrl = Settings.Default.BaseUrlBamboo;
                    break;
            }

            return new ClientConfiguration
                {
                    Domain = Settings.Default.Domain,
                    Port = Settings.Default.Port,
                    UserName = Settings.Default.UserName,
                    Password = SecureData.Decrypt(Settings.Default.Password),
                    UseSsl = Settings.Default.UseSsl,
                    IgnoreInvalidCertificate = Settings.Default.IgnoreInvalidCertificate,
                    ClientType = Settings.Default.ClientType,
                    BaseUrl = baseUrl
                };
        }
    }
}
