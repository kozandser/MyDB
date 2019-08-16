using FirstFloor.ModernUI.Windows;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using MyDB4.Models;
using MyDB4.Resources;
using MyDB4.ViewModels;
using PropertyChanged;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Navigation;
using LiveCharts.Defaults;
using FirstFloor.ModernUI.Windows.Controls;

namespace MyDB4.Pages
{

    [ImplementPropertyChanged]
    public partial class BooksStatsPage : UserControl, IContent
    {
        BooksStatisticsViewModel Statistics { get; set; } = new BooksStatisticsViewModel();

        public BooksStatsPage()
        {
            InitializeComponent();
            DataContext = Statistics;
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {

        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {

        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            Statistics.Reload();
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {

        }
        
    }
}
