using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MyDB4.Models;
using MyDB4.Resources;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class BooksStatisticsViewModel
    {        
        [ImplementPropertyChanged]
        public class StateStatisticsViewModel
        {
            private List<BookViewModel> items = new List<BookViewModel>();
            public SeriesCollection Series { get; set; }

            public StateStatisticsViewModel()
            {
                ReloadSeries();
            }

            public void ReloadSeries()
            {
                Series = new SeriesCollection();
                items = MainViewModel.Default.BooksViewModel.Items.ToList();
                foreach (var x in BookViewModel.StatesDict)
                {
                    var ps = new PieSeries()
                    {
                        Values = new ChartValues<int>() { items.Count(p => p.LastState.Name == x.Key) },
                        Title = x.Value,
                        DataLabels = true,
                        Fill = ConvertFuncs.StateToBrushFunc(x.Key)
                    };
                    Series.Add(ps);
                }
            }
        }
        [ImplementPropertyChanged]
        public class RatingStatisticsViewModel
        {
            private class DataModel
            {
                public double Rating { get; set; }
                public int Count { get; set; }
            }
            private List<GameViewModel> items = new List<GameViewModel>();
            public SeriesCollection Series { get; set; }

            public Func<double, string> XFormatter { get; set; }
            public Func<double, string> YFormatter { get; set; }
            private CartesianMapper<DataModel> Config = Mappers.Xy<DataModel>()
                .X(model => model.Rating)
                .Y(model => model.Count);

            public RatingStatisticsViewModel()
            {
                XFormatter = value => value.ToString();
                YFormatter = value => value.ToString();
                ReloadSeries();
            }

            public void ReloadSeries()
            {
                Series = new SeriesCollection(Config);
                items = MainViewModel.Default.GamesViewModel.Items.Where(p => p.Rating.Value > 0).ToList();
                var points = new List<DataModel>();
                for (double i = 0.5; i <= 5; i = i + 0.5)
                {
                    points.Add(
                            new DataModel()
                            {
                                Rating = i,
                                Count = items.Count(p => p.Rating.Value == i)
                            });
                }
                Series.Add(
                        new ColumnSeries()
                        {
                            Values = new ChartValues<DataModel>(points),
                            Title = "Кол-во оценок",
                            ColumnPadding = 5,
                            DataLabels = true
                        });
            }
        }       

        public int AllCount { get; set; }
        public StateStatisticsViewModel StateStatistics { get; set; } = new StateStatisticsViewModel();
        public RatingStatisticsViewModel RatingStatistics { get; set; } = new RatingStatisticsViewModel();        

        public BooksStatisticsViewModel()
        {
            Reload();
        }

        public void Reload()
        {
            AllCount = MainViewModel.Default.BooksViewModel.Items.Count;

            StateStatistics.ReloadSeries();
            RatingStatistics.ReloadSeries();
        }
    }

    
}
