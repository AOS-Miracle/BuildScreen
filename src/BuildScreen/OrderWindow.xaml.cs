using BuildScreen.ContinousIntegration.Entities;
using BuildScreen.Properties;
using BuildScreen.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BuildScreen
{
    /// <summary>
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        OrderWindowViewModel _viewModel;

        public OrderWindow(BuildList buildlist)
        {
            InitializeComponent();

            _viewModel = (DataContext as OrderWindowViewModel);

            _viewModel.Builds = buildlist;
        }

        private void ButtonOkay_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Builds = _viewModel.Builds;
            Settings.Default.Save();

            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
