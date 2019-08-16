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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class GamesStatisticsViewModel
    {        
        [ImplementPropertyChanged]
        public class StateStatisticsViewModel
        {
            private List<GameViewModel> items = new List<GameViewModel>();
            public SeriesCollection Series { get; set; }

            public StateStatisticsViewModel()
            {
                ReloadSeries();
            }

            public void ReloadSeries()
            {
                Series = new SeriesCollection();
                items = MainViewModel.Default.GamesViewModel.Items.ToList();
                foreach (var x in GameViewModel.StatesDict)
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
        [ImplementPropertyChanged]
        public class YearStatisticsViewModel
        {
            public class IntervalStatItem
            {
                public string Title { get; set; }
                public double Start { get; set; }
                public double End { get; set; }
                public bool IsFinished { get; set; }
                public SolidColorBrush Fill { get; set; }

            }

            private List<GameViewModel> items = new List<GameViewModel>();
            private bool _showPaused;
            public bool ShowPaused
            {
                get
                {
                    return _showPaused;
                }
                set
                {
                    _showPaused = value;
                    ReloadSeries();
                }
            }

            public ICommand PrevPeriodCommand { get; set; }
            public ICommand NextPeriodCommand { get; set; }
            public ICommand ResetPeriodCommand { get; set; }
            public ICommand ResetZoomCommand { get; set; }
            public EventHandler Reloaded { get; set; }

            public int Year { get; set; }
            public double Step { get; set; } = TimeSpan.FromDays(1).Ticks * 30.44;

            public SeriesCollection Series { get; set; }
            private GanttMapper<IntervalStatItem> Config = Mappers.Gantt<IntervalStatItem>()
                .XStart(p => p.Start)
                .X(p => p.End)
                .Fill(p => p.Fill);
            public Func<double, string> XFormatter { get; set; }
            public List<string> Labels { get; set; } = new List<string>();
            public double From { get; set; }
            public double To { get; set; }
            public double YearTicks => To - From;

            public YearStatisticsViewModel()
            {
                items = MainViewModel.Default.GamesViewModel.Items.ToList();
                XFormatter = value => new DateTime((long)value).ToString("dd.MM.yyyy");

                PrevPeriodCommand = new RelayCommand(PrevPeriod, "Назад");
                NextPeriodCommand = new RelayCommand(NextPeriod, "Вперед");
                ResetPeriodCommand = new RelayCommand(ResetPeriod, "Текущий год");
                ResetZoomCommand = new RelayCommand(ResetZoom, "Сброс зума");

                Year = DateTime.Today.Year;
                Series = new SeriesCollection(Config);
            }

            public void PrevPeriod()
            {
                Year -= 1;
                ReloadSeries();
            }
            public void NextPeriod()
            {
                Year = Year + 1;
                ReloadSeries();
            }
            public void ResetPeriod()
            {
                Year = DateTime.Today.Year;
                ReloadSeries();
            }
            public void ResetZoom()
            {
                From = new DateTime(Year, 01, 01).Ticks;// items.First().Start;
                To = new DateTime(Year, 12, 31).Ticks; //items.Last().End;
            }

            public void ReloadSeries()
            {
                Series.Clear();
                var games = new List<IntervalStatItem>();
                foreach (var game in items.Where(p => p.IsStartedWithDate))
                {
                    if (ShowPaused == false &&
                        game.States.IsStateDone(Models.StateEnum.Paused) == true &&
                        game.States.IsStateDone(Models.StateEnum.Finished) == false) continue;

                    var item = new IntervalStatItem()
                    {
                        Title = game.Title,
                        Start = game.StartDate.Value.Ticks
                    };
                    if (game.IsFinishedWithDate)
                    {
                        item.End = game.FinishDate.Value.Ticks;
                        item.IsFinished = true;
                        item.Fill = Resources.XamlResources.FinishedState_ColorBrush;
                    }
                    else
                    {
                        if (ShowPaused == true && game.States.IsStateDone(Models.StateEnum.Paused) == true)
                        {
                            item.End = DateTime.Today.Ticks;
                            item.Fill = Resources.XamlResources.PausedState_ColorBrush;
                        }
                        if (game.States.IsStateDone(Models.StateEnum.Paused) == false)
                        {
                            item.End = DateTime.Today.Ticks;
                            item.Fill = Resources.XamlResources.InQueueState_ColorBrush;
                        }
                    }
                    games.Add(item);                    
                }
                Labels = new List<string>();
                var temp = games.ToList();
                From = new DateTime(Year, 01, 01).Ticks;// items.First().Start;
                To = new DateTime(Year, 12, 31).Ticks; //items.Last().End;
                foreach (var item in temp)
                {
                    if ((item.Start < From && item.End < From) ||
                         (item.Start > To && item.End > To))
                        games.Remove(item);
                }
                games = games.OrderByDescending(p => p.Start).ToList();
                foreach (var item in games)
                {
                    Labels.Add(item.Title);
                }

                Series.Add(
                        new RowSeries
                        {
                            Values = new ChartValues<IntervalStatItem>(games),
                            DataLabels = false,
                            Title = "",
                            LabelPoint = p => new DateTime((long)p.XStart).ToString("dd.MM.yyyy") + " - " + new DateTime((long)p.X).ToString("dd.MM.yyyy") +
                            " - " + new TimeSpan((long)(p.X - p.XStart)).TotalDays
                        });
                Reloaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public int AllCount { get; set; }
        public StateStatisticsViewModel StateStatistics { get; set; } = new StateStatisticsViewModel();
        public RatingStatisticsViewModel RatingStatistics { get; set; } = new RatingStatisticsViewModel();
        public YearStatisticsViewModel YearStatistics { get; set; } = new YearStatisticsViewModel();

        public GamesStatisticsViewModel()
        {
            
        }

        public void Reload()
        {
            AllCount = MainViewModel.Default.GamesViewModel.Items.Count;

            StateStatistics.ReloadSeries();
            RatingStatistics.ReloadSeries();
            YearStatistics.ReloadSeries();
        }
    }

    
}
