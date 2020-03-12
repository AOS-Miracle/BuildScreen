using BuildScreen.ContinousIntegration.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildScreen.ViewModels
{
    public class OrderWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private BuildList _buildlist;

        public OrderWindowViewModel()
        {

        }

        public BuildList Builds
        {
            get { return _buildlist; }
            set
            {
                if (value != _buildlist)
                {
                    _buildlist = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Builds"));
                }
            }
        }
    }
}
