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
    public partial class MoviesStatsPage : UserControl, IContent
    {
        MoviesStatisticsViewModel Statistics { get; set; } = new MoviesStatisticsViewModel();

        public MoviesStatsPage()
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

        private void EveryDay_chart_DataClick(object sender, ChartPoint chartPoint)
        {
            var date = new DateTime((long)(chartPoint.X * TimeSpan.FromDays(1).Ticks));
            var lst = Statistics.EveryDayStatistics.GetMovies(date);
            var dlg = new SimpleMoviesList(lst);
            dlg.Title = date.ToString("dd MMMM yyyy");
            dlg.ShowDialog();
        }

        private void EveryMonth_chart_DataClick(object sender, ChartPoint chartPoint)
        {
            var lst = Statistics.EveryMonthStatistics.GetMovies(Statistics.EveryMonthStatistics.Year, (int)chartPoint.X);
            var dlg = new SimpleMoviesList(lst);
            dlg.Title = new DateTime(Statistics.EveryMonthStatistics.Year, (int)chartPoint.X, 1).ToString("MMMM, yyyy");
            dlg.ShowDialog();
        }

        private void EveryYear_chart_DataClick(object sender, ChartPoint chartPoint)
        {
            var lst = Statistics.EveryYearStatistics.GetMovies((int)chartPoint.X);
            var dlg = new SimpleMoviesList(lst);
            dlg.Title = new DateTime((int)chartPoint.X,1, 1).ToString("yyyy");
            dlg.ShowDialog();
        }
    }
}
