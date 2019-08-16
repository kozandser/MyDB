using KLib;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MyDB4.Models;
using MyDB4.Resources;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class MoviesStatisticsViewModel
    {
        public class CountClass
        {
            public int Count { get; private set; }
            public TypeEnum Type { get; private set; }
            public string Text { get; private set; }

            public CountClass(int count, TypeEnum type, string text)
            {
                Count = count;
                Type = type;
                Text = text;
            }
        }

        [ImplementPropertyChanged]
        public class TypeStatisticsViewModel
        {
            private List<MovieViewModel> items = new List<MovieViewModel>();
            public SeriesCollection Series { get; set; }

            public TypeStatisticsViewModel()
            {
                Series = new SeriesCollection();
            }

            public void ReloadSeries()
            {                
                items = MainViewModel.Default.MoviesViewModel.Items.ToList();
                Series.Clear();

                foreach (var x in MovieViewModel.TypesDict)
                {
                    var ps = new PieSeries()
                    {
                        Values = new ChartValues<int>() { items.Where(p => p.Type == x.Key).Count() },
                        Title = x.Value,
                        DataLabels = true,
                        Fill = ConvertFuncs.TypeToBrushFunc(x.Key)
                    };
                    Series.Add(ps);
                }               
            }
        }
        [ImplementPropertyChanged]
        public class StateStatisticsViewModel
        {
            private List<MovieViewModel> items = new List<MovieViewModel>();
            public SeriesCollection Series { get; set; }

            public StateStatisticsViewModel()
            {
                Series = new SeriesCollection();
            }

            public void ReloadSeries()
            {
                items = MainViewModel.Default.MoviesViewModel.Items.ToList();
                Series.Clear();
                foreach (var x in MovieViewModel.StatesDict)
                {
                    var ps = new PieSeries()
                    {
                        Values = new ChartValues<int>() { items.Count(p => p.States.LastState.Name == x.Key) },
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
            private List<MovieViewModel> items = new List<MovieViewModel>();
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
                Series = new SeriesCollection(Config);
            }

            public void ReloadSeries()
            {                
                items = MainViewModel.Default.MoviesViewModel.Items.Where(p => p.Rating.Value > 0).ToList();
                Series.Clear();
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
        [ImplementPropertyChanged]
        public class EveryDayStatisticsViewModel
        {
            public ICommand PrevPeriodCommand { get; set; }
            public ICommand NextPeriodCommand { get; set; }
            public ICommand ResetPeriodCommand { get; set; }
            public EventHandler Reloaded { get; set; }

            private List<MovieViewModel> items = new List<MovieViewModel>();
            public DateTime FirstDay { get; set; }
            public DateTime LastDay { get; set; }
            public string PeriodString => FirstDay.ToString("MMMM, yyyy");
            public void SetPeriod(int year, int month)
            {
                FirstDay = new DateTime(year, month, 1);
                LastDay = new DateTime(year, month, DateTime.DaysInMonth(year, FirstDay.Month));
            }
            public void PrevPeriod()
            {
                SetPeriod(FirstDay.AddMonths(-1).Year, FirstDay.AddMonths(-1).Month);
                ReloadSeries();
            }
            public void NextPeriod()
            {
                SetPeriod(FirstDay.AddMonths(1).Year, FirstDay.AddMonths(1).Month);
                ReloadSeries();
            }
            public void ResetPeriod()
            {
                SetPeriod(DateTime.Today.Year, DateTime.Today.Month);
                ReloadSeries();
            }

            public ObservableCollection<CountClass> Counts { get; set; } = new ObservableCollection<CountClass>();
            public int Totals { get; set; }

            public SeriesCollection Series { get; set; }
            public Func<double, string> XFormatter { get; set; }
            public CartesianMapper<DateTimePoint> Config = Mappers.Xy<DateTimePoint>()
                .X(dateModel => dateModel.DateTime.Ticks / TimeSpan.FromDays(1).Ticks)
                .Y(dateModel => dateModel.Value);

            public EveryDayStatisticsViewModel()
            {
                XFormatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("d");

                PrevPeriodCommand = new RelayCommand(PrevPeriod, "Назад");
                NextPeriodCommand = new RelayCommand(NextPeriod, "Назад");
                ResetPeriodCommand = new RelayCommand(ResetPeriod, "Назад");

                items = MainViewModel.Default.MoviesViewModel.Items.ToList();

                SetPeriod(DateTime.Today.Year, DateTime.Today.Month);

                Series = new SeriesCollection(Config);
            }
            
            public void ReloadSeries()
            {
                var views = items.Where(p => p.States.IsStateDoneWithDate(StateEnum.Finished)).ToList();
                Series.Clear();                
                Counts.Clear();
                foreach (var t in MovieViewModel.TypesDict)
                {
                    int c = 0;
                    var points = new List<DateTimePoint>();
                    for (var dt = FirstDay; dt <= LastDay; dt = dt.AddDays(1))
                    {
                        var count = views.Count(p => p.FinishDate == dt && p.Type == t.Key);
                        points.Add(
                            new DateTimePoint()
                            {
                                DateTime = dt,
                                Value = count
                            });
                        c = c + count;
                    }

                    var a = new StackedColumnSeries()
                    {
                        Values = new ChartValues<DateTimePoint>(points),
                        Title = t.Value,
                        Fill = ConvertFuncs.TypeToBrushFunc(t.Key)
                    };
                    Series.Add(a);
                    Counts.Add(new CountClass(c, t.Key, t.Value));
                }
                Totals = Counts.Sum(p => p.Count);
                Reloaded?.Invoke(null, EventArgs.Empty);
            }
            public List<MovieViewModel> GetMovies(DateTime date)
            {
                return items.Where(p => p.FinishDate == date).ToList();
            }
        }
        [ImplementPropertyChanged]
        public class EveryMonthStatisticsViewModel
        {
            private class DataModel
            {
                public int Month { get; set; }
                public int Count { get; set; }
            }

            public ICommand PrevPeriodCommand { get; set; }
            public ICommand NextPeriodCommand { get; set; }
            public ICommand ResetPeriodCommand { get; set; }
            public EventHandler Reloaded { get; set; }

            private List<MovieViewModel> items;
            public int Year { get; set; }

            public ObservableCollection<CountClass> Counts { get; set; } = new ObservableCollection<CountClass>();
            public int Totals { get; set; }

            public void PrevPeriod()
            {
                Year -= 1;
                ReloadSeries();
            }
            public void NextPeriod()
            {
                Year++;
                ReloadSeries();
            }
            public void ResetPeriod()
            {
                Year = DateTime.Today.Year;
                ReloadSeries();
            }

            public SeriesCollection Series { get; set; }
            public Func<double, string> XFormatter { get; set; }
            private CartesianMapper<DataModel> Config = Mappers.Xy<DataModel>()
                .X(model => model.Month)
                .Y(model => model.Count);

            public EveryMonthStatisticsViewModel()
            {
                XFormatter = value => new DateTime(Year, (int)value, 1).ToString("MMMM, yyyy");

                PrevPeriodCommand = new RelayCommand(PrevPeriod, "Назад");
                NextPeriodCommand = new RelayCommand(NextPeriod, "Вперед");
                ResetPeriodCommand = new RelayCommand(ResetPeriod, "Сброс");

                Year = DateTime.Today.Year;
                items = MainViewModel.Default.MoviesViewModel.Items.ToList();
                Series = new SeriesCollection(Config);
            }            
            public void ReloadSeries()
            {
                var views = items.Where(p => p.IsFinishedWithDate && p.FinishDate.Value.Year == Year).ToList();
                Series.Clear();
                Counts.Clear();
                
                foreach (var t in MovieViewModel.TypesDict)//.Where(p => p.Key != TypeEnum.NA))
                {
                    int c = 0;
                    var points = new List<DataModel>();
                    for (var dt = 1; dt <= 12; dt = dt + 1)
                    {
                        var count = views.Count(p => p.FinishDate.Value.Month == dt && p.Type == t.Key);
                        points.Add(
                            new DataModel()
                            {
                                Month = dt,
                                Count = count
                            });
                        c = c + count;
                    }
                    var a = new StackedColumnSeries()
                    {
                        Values = new ChartValues<DataModel>(points),
                        Title = t.Value,
                        Fill = ConvertFuncs.TypeToBrushFunc(t.Key)
                    };
                    Series.Add(a);
                    Counts.Add(new CountClass(c, t.Key, t.Value));
                }
                Totals = Counts.Sum(p => p.Count);
                Reloaded?.Invoke(null, EventArgs.Empty);
            }
            public List<MovieViewModel> GetMovies(int year, int month)
            {
                return items.Where(p => p.IsFinishedWithDate && p.FinishDate.Value.Year == Year && p.FinishDate.Value.Month == month).ToList();
            }
        }
        [ImplementPropertyChanged]
        public class EveryYearStatisticsViewModel
        {
            private class DataModel
            {
                public int Year { get; set; }
                public int Count { get; set; }
            }

            public EventHandler Reloaded { get; set; }

            private List<MovieViewModel> items;
            public ObservableCollection<CountClass> Counts { get; set; } = new ObservableCollection<CountClass>();
            public int Totals { get; set; }

            public SeriesCollection Series { get; set; }
            public Func<double, string> XFormatter { get; set; }
            private CartesianMapper<DataModel> Config = Mappers.Xy<DataModel>()
                .X(model => model.Year)
                .Y(model => model.Count);

            public EveryYearStatisticsViewModel()
            {
                XFormatter = value => value.ToString();
                items = MainViewModel.Default.MoviesViewModel.Items.ToList();
                Series = new SeriesCollection(Config);
            }
            public void ReloadSeries()
            {
                var views = items.Where(p => p.IsFinishedWithDate).ToList();
                Series.Clear();
                Counts.Clear();             
                foreach (var t in MovieViewModel.TypesDict)
                {
                    int c = 0;
                    var points = new List<DataModel>();
                    for (var dt = 2012; dt <= DateTime.Today.Year; dt = dt + 1)
                    {
                        var count = views.Count(p => p.FinishDate.Value.Year == dt && p.Type == t.Key);
                        points.Add(
                            new DataModel()
                            {
                                Year = dt,
                                Count = count
                            });
                        c = c + count;
                    }
                    var a = new StackedColumnSeries()
                    {
                        Values = new ChartValues<DataModel>(points),
                        Title = t.Value,
                        Fill = ConvertFuncs.TypeToBrushFunc(t.Key)
                    };
                    Series.Add(a);
                    Counts.Add(new CountClass(c, t.Key, t.Value));                    
                }
                Totals = Counts.Sum(p => p.Count);
                Reloaded?.Invoke(null, EventArgs.Empty);
            }
            public List<MovieViewModel> GetMovies(int year)
            {
                return items.Where(p => p.IsFinishedWithDate && p.FinishDate.Value.Year == year).ToList();
            }
        }

        public int AllCount { get; set; }
        public TypeStatisticsViewModel TypeStatistics { get; set; } = new TypeStatisticsViewModel();
        public StateStatisticsViewModel StateStatistics { get; set; } = new StateStatisticsViewModel();
        public RatingStatisticsViewModel RatingStatistics { get; set; } = new RatingStatisticsViewModel();
        public EveryDayStatisticsViewModel EveryDayStatistics { get; set; } = new EveryDayStatisticsViewModel();
        public EveryMonthStatisticsViewModel EveryMonthStatistics { get; set; } = new EveryMonthStatisticsViewModel();
        public EveryYearStatisticsViewModel EveryYearStatistics { get; set; } = new EveryYearStatisticsViewModel();

        public MoviesStatisticsViewModel()
        {
            //Reload();
        }

        public void Reload()
        {
            AllCount = MainViewModel.Default.MoviesViewModel.Items.Count;

            TypeStatistics.ReloadSeries();
            StateStatistics.ReloadSeries();
            RatingStatistics.ReloadSeries();
            EveryDayStatistics.ReloadSeries();
            EveryMonthStatistics.ReloadSeries();
            EveryYearStatistics.ReloadSeries();
        }
    }

    
}
