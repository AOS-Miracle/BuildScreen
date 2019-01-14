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
            catch (ClientConnectionException ex)
            {
                // Display error message
                MessageBox.Show("Der er sket en fejl: " + ex.Message);
            }
        }

        private void LoadGridWithBuilds(Build build)
        {
            StackPanel stackPanel = new StackPanel();

            Viewbox viewbox = new Viewbox();
            viewbox.HorizontalAlignment = HorizontalAlignment.Left;
            viewbox.Child = stackPanel;

            Grid grid = new Grid();
            grid.Tag = build;

            stackPanel.Children.Insert(0, new TextBlock
            {
                Text = string.IsNullOrEmpty(build.ProjectName) ? build.TypeName : string.Format("{0}, {1}", build.ProjectName, build.TypeName),
                FontSize = 24,
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(10, 0, 10, 0)
            });

            if (build.NextBuildRunning)
            {
                grid.Background = build.NextBuild.Status == Status.Success ? Graphics.BrushNeutral : Graphics.BrushFailure;

                stackPanel.Children.Insert(1, new TextBlock
                {
                    Text = string.Format("Build {0} running... ({1}% Complete - {2})", build.NextBuild.Number, build.NextBuild.percentageComplete, build.NextBuild.Status == Status.Success ? "Succeeding" : "Failing"),
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Colors.White),
                    Padding = new Thickness(10, 0, 10, 6)
                });

                DoAnimationRectangle(grid, build);
            }
            else
            {
                grid.Background = build.Status == Status.Success ? Graphics.BrushSuccess : Graphics.BrushFailure;

                stackPanel.Children.Insert(1, new TextBlock
                {
                    Text = string.IsNullOrEmpty(build.ProjectName) ? string.Format("Build {0}", build.Number) : string.Format("Build {0} ({2}), {1}", build.Number, build.StatusText, build.LastChangeBy),
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Colors.White),
                    Padding = new Thickness(10, 0, 10, 6)
                });
            }

            grid.Children.Insert(0, viewbox);

            UniformGridBuilds.Children.Insert(0, grid);
        }

        private static void DoAnimationRectangle(Grid g, Build build)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 1;
            opacityAnimation.To = 0;
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));
            opacityAnimation.AutoReverse = true;
            opacityAnimation.RepeatBehavior = RepeatBehavior.Forever;

            //DoubleAnimation widthAnimation = new DoubleAnimation();
            //widthAnimation.From = 1;
            //widthAnimation.To = 100;
            //widthAnimation.Duration = new Duration(TimeSpan.FromSeconds(5));


            Storyboard.SetTarget(opacityAnimation, g);
            //Storyboard.SetTarget(widthAnimation, g);

            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Grid.OpacityProperty));
            //Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Grid.WidthProperty));

            Storyboard myAnimatedGridStoryboard = new Storyboard();
            myAnimatedGridStoryboard.Children.Add(opacityAnimation);
            //myAnimatedGridStoryboard.Children.Add(widthAnimation);

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
