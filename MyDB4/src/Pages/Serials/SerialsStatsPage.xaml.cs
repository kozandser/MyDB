using FirstFloor.ModernUI.Windows;
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
using MyDB4.ViewModels;
using PropertyChanged;

namespace MyDB4.Pages
{
    /// <summary>
    /// Interaction logic for SerialsStatsPage.xaml
    /// </summary>
    /// 
    [ImplementPropertyChanged]
    public partial class SerialsStatsPage : UserControl, IContent
    {
        SerialsStatisticsViewModel Statistics { get; set; } = new SerialsStatisticsViewModel();

        public SerialsStatsPage()
        {
            InitializeComponent();
            Statistics.YearStatistics.Reloaded += (o, e) =>
            {
                XAxis.MinValue = Statistics.YearStatistics.From;
                XAxis.MaxValue = Statistics.YearStatistics.To;
            };

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            XAxis.MinValue = Statistics.YearStatistics.From;
            XAxis.MaxValue = Statistics.YearStatistics.To;
        }
    }
}
