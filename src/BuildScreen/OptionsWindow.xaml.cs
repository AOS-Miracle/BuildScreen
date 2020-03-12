using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using BuildScreen.ContinousIntegration;
using BuildScreen.ContinousIntegration.Entities;
using BuildScreen.ContinousIntegration.Persistance;
using BuildScreen.Core.Security;
using BuildScreen.Core.Utilities;
using BuildScreen.Properties;
using BuildScreen.Resources;

namespace BuildScreen
{
    public partial class OptionsWindow
    {
        private readonly BackgroundWorker _backgroundWorker;
        private readonly IClientFactory _clientFactory;

        public OptionsWindow()
        {
            InitializeComponent();

            PictureBoxProgress.Image = InternalResources.Image_Load_16x16;
            PictureBoxProgress.Visible = false;
            WindowsFormHost.Visibility = Visibility.Hidden;

            _clientFactory = new ClientFactory();

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += WorkerFetching;
            _backgroundWorker.RunWorkerCompleted += WorkerCompleted;
        }

        private void OptionsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
            ConnectAndRender();
        }

        private void ButtonTestConnection_Click(object sender, RoutedEventArgs e)
        {
            ConnectAndRender();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_backgroundWorker.IsBusy)
                _backgroundWorker.CancelAsync();

            Close();
        }

        private void ButtonOkay_Click(object sender, RoutedEventArgs e)
        {
            if (_backgroundWorker.IsBusy)
                _backgroundWorker.CancelAsync();

            BuildList storedBuilds = new BuildList();

            foreach (object item in ListBoxBuilds.Items.Cast<object>().Where(item => ((CheckBox)item).IsChecked == true))
            {
                storedBuilds.Add(((CheckBox)item).Tag as Build);
            }

            SaveSettings(storedBuilds);

            Close();
        }

        private void OrderWindowButton_Click(object sender, RoutedEventArgs e)
        {
            BuildList storedBuilds = new BuildList();
            foreach (object item in ListBoxBuilds.Items.Cast<object>().Where(item => ((CheckBox)item).IsChecked == true))
            {
                storedBuilds.Add(((CheckBox)item).Tag as Build);
            }

            SaveSettings(storedBuilds);

            OrderWindow orderDialog = new OrderWindow(storedBuilds);
            orderDialog.ShowDialog();

        }

        private void TextBoxDomain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Validate.IsDomain(TextBoxDomain.Text) && !Validate.IsIpAddress(TextBoxDomain.Text))
                TextBoxPreview.Text = InternalResources.ValidateInvalidDomain;
            else
                TextBoxPreview.Text = CreatePreview();
        }

        private void TextBoxPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBoxPort.Text) && !Validate.IsNumeric(TextBoxPort.Text))
                TextBoxPreview.Text = InternalResources.ValidateInvalidePortNumber;
            else
                TextBoxPreview.Text = CreatePreview();
        }

        protected void RadioButtonProtocolHttp_Checked(object sender, RoutedEventArgs e)
        {
            TextBoxPreview.Text = CreatePreview();
        }

        protected void RadioButtonProtocolHttp_Unchecked(object sender, RoutedEventArgs e)
        {
            TextBoxPreview.Text = CreatePreview();
        }

        protected void RadioButtonProtocolHttps_Checked(object sender, RoutedEventArgs e)
        {
            TextBoxPreview.Text = CreatePreview();
        }

        protected void RadioButtonProtocolHttps_Unchecked(object sender, RoutedEventArgs e)
        {
            TextBoxPreview.Text = CreatePreview();
        }

        public void ConnectAndRender()
        {
            ClientConfiguration clientConfiguration = CreateClientConfiguration();

            if (clientConfiguration.AllDataExists)
            {
                PictureBoxProgress.Visible = true;
                WindowsFormHost.Visibility = Visibility.Visible;
                ButtonTestConnection.IsEnabled = false;
                ListBoxBuilds.Items.Clear();
                TextBoxErrorMessage.Visibility = Visibility.Hidden;
                TextBlockConnectionStatus.Visibility = Visibility.Visible;
                GroupBoxBuilds.IsEnabled = false;
                OrderWindowButton.IsEnabled = false;

                RunWorker(clientConfiguration);
            }
        }

        private void RunWorker(ClientConfiguration clientConfiguration)
        {
            _backgroundWorker.RunWorkerAsync(clientConfiguration);
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                string status = string.Empty;

                if (e.Error.InnerException.GetType() == typeof(WebException))
                    status = string.Concat(((WebException)e.Error.InnerException).Status.ToString(), ": ");

                TextBoxErrorMessage.Text = string.Concat(status, e.Error.Message);
                TextBoxErrorMessage.Visibility = Visibility.Visible;
            }
            else
            {
                TextBlockConnectionStatus.Visibility = Visibility.Hidden;
                GroupBoxBuilds.IsEnabled = true;
                OrderWindowButton.IsEnabled = true;

                FillListBoxBuilds(e.Result as ReadOnlyCollection<Build>);
            }

            ButtonTestConnection.IsEnabled = true;
            PictureBoxProgress.Visible = false;
            WindowsFormHost.Visibility = Visibility.Hidden;
        }

        private void WorkerFetching(object sender, DoWorkEventArgs e)
        {
            using (IClient client = _clientFactory.CreateClient(e.Argument as ClientConfiguration))
            {
                if (client.IsConnected)
                {
                    ReadOnlyCollection<Build> builds = client.Builds();
                    e.Result = builds;
                }
            }
        }

        private void FillListBoxBuilds(IEnumerable<Build> builds)
        {
            foreach (Build build in builds)
            {
                CheckBox checkBox = new CheckBox();

                if (Settings.Default.Builds != null)
                {
                    foreach (Build storedBuild in Settings.Default.Builds)
                    {
                        if (storedBuild.UniqueIdentifier == build.UniqueIdentifier)
                        {
                            checkBox.IsChecked = true;
                            build.DisplayOrder = storedBuild.DisplayOrder;
                        }         
                    }
                }

                checkBox.Content = string.IsNullOrEmpty(build.ProjectName) ? build.TypeName : string.Format("{0}, {1}", build.ProjectName, build.TypeName);
                checkBox.Margin = new Thickness(6);
                checkBox.Padding = new Thickness(9, 0, 0, 0);
                checkBox.Tag = build;

                ListBoxBuilds.Items.Add(checkBox);
            }
        }

        private ClientConfiguration CreateClientConfiguration()
        {
            ClientType clientType = (ClientType)Enum.Parse(typeof(ClientType), ComboBoxCIType.Text);
            string baseUrl = string.Empty;

            switch (clientType)
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
                    Domain = TextBoxDomain.Text,
                    Port = string.IsNullOrEmpty(TextBoxPort.Text) ? 80 : Convert.ToInt32(TextBoxPort.Text),
                    UserName = TextBoxUserName.Text,
                    Password = PasswordBoxPassword.Password,
                    UseSsl = Convert.ToBoolean(RadioButtonProtocolHttps.IsChecked),
                    IgnoreInvalidCertificate = Settings.Default.IgnoreInvalidCertificate,
                    ClientType = clientType,
                    BaseUrl = baseUrl
                };
        }

        private string CreatePreview()
        {
            string preview = Convert.ToBoolean(RadioButtonProtocolHttps.IsChecked) ? "https://" : "http://";

            preview += TextBoxDomain.Text;

            if (!string.IsNullOrEmpty(TextBoxPort.Text))
                preview += ":" + TextBoxPort.Text;

            return preview + "/";
        }

        private void UnCheckAll(object sender, RoutedEventArgs e)
        {
            SetCheckBoxesInListBox(false);
        }

        private void CheckAll(object sender, RoutedEventArgs e)
        {
            SetCheckBoxesInListBox(true);
        }

        private void SetCheckBoxesInListBox(bool shouldBeChecked)
        {
            foreach (CheckBox checkBox in ListBoxBuilds.Items)
            {
                checkBox.IsChecked = shouldBeChecked;
            }
        }

        private void ListBoxBuilds_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // ignore selecting a row only when context menu should show up
            e.Handled = true;
        }

        private void CheckBoxHideInactive_Checked(object sender, RoutedEventArgs e)
        {
            LabelHideInactiveSince.IsEnabled = true;
            LabelHideInactiveWeeks.IsEnabled = true;
            ComboBoxHideInactive.IsEnabled = true;
        }

        private void CheckBoxHideInactive_Unchecked(object sender, RoutedEventArgs e)
        {
            LabelHideInactiveSince.IsEnabled = false;
            LabelHideInactiveWeeks.IsEnabled = false;
            ComboBoxHideInactive.IsEnabled = false;
        }

        private void TextBoxPort_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxPort.SelectAll();
        }

        private void TextBoxUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxUserName.SelectAll();
        }

        private void PasswordBoxPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBoxPassword.SelectAll();
        }

        private void SaveSettings(BuildList storedBuilds)
        {
            Settings.Default.Domain = TextBoxDomain.Text;
            Settings.Default.Port = string.IsNullOrEmpty(TextBoxPort.Text) ? 80 : Convert.ToInt32(TextBoxPort.Text);
            Settings.Default.UserName = TextBoxUserName.Text;
            Settings.Default.Password = SecureData.Encrypt(PasswordBoxPassword.Password);
            Settings.Default.RefreshInterval = Convert.ToInt32(ComboBoxRefreshInterval.Text);
            Settings.Default.UseSsl = Convert.ToBoolean(RadioButtonProtocolHttps.IsChecked);
            Settings.Default.Builds = storedBuilds;
            Settings.Default.HideInactive = Convert.ToBoolean(CheckBoxHideInactive.IsChecked);
            Settings.Default.HideInactiveWeeks = Convert.ToInt32(ComboBoxHideInactive.Text);
            Settings.Default.ClientType = (ClientType)Enum.Parse(typeof(ClientType), ComboBoxCIType.Text);
            Settings.Default.NumberOfColumns = Convert.ToInt32(ComboBoxColumnsNumber.Text);

            Settings.Default.Save();
        }

        private void LoadSettings()
        {
            TextBoxDomain.Text = Settings.Default.Domain;
            TextBoxPort.Text = Settings.Default.Port == 80 ? string.Empty : Settings.Default.Port.ToString();
            TextBoxUserName.Text = Settings.Default.UserName;

            PasswordBoxPassword.Password = SecureData.Decrypt(Settings.Default.Password);

            CheckBoxHideInactive.IsChecked = Settings.Default.HideInactive;

            RadioButtonProtocolHttp.IsChecked = !Settings.Default.UseSsl;
            RadioButtonProtocolHttps.IsChecked = Settings.Default.UseSsl;

            ComboBoxRefreshInterval.Items.Cast<ComboBoxItem>()
                .Where(comboBoxItem => (string)comboBoxItem.Content == Settings.Default.RefreshInterval.ToString())
                .Select(comboBoxItem => comboBoxItem).Single().IsSelected = true;

            ComboBoxColumnsNumber.Items.Cast<ComboBoxItem>()
                .Where(comboBoxItem => (string)comboBoxItem.Content == Settings.Default.NumberOfColumns.ToString())
                .Select(comboBoxItem => comboBoxItem).Single().IsSelected = true;

            ComboBoxHideInactive.Items.Cast<ComboBoxItem>()
                .Where(comboBoxItem => (string)comboBoxItem.Content == Settings.Default.HideInactiveWeeks.ToString())
                .Select(comboBoxItem => comboBoxItem).Single().IsSelected = true;

            ComboBoxCIType.Items.Cast<ComboBoxItem>()
                .Where(comboBoxItem => (string)comboBoxItem.Content == Settings.Default.ClientType.ToString())
                .Select(comboBoxItem => comboBoxItem).Single().IsSelected = true;
        }
    }
}
