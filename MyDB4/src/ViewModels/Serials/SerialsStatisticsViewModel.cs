using KLib;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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
    public class SerialsStatisticsViewModel
    {
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

            private List<SerialViewModel> items = new List<SerialViewModel>();

            public ICommand PrevPeriodCommand { get; set; }
            public ICommand NextPeriodCommand { get; set; }
            public ICommand ResetPeriodCommand { get; set; }
            public ICommand ResetZoomCommand { get; set; }
            public EventHandler Reloaded { get; set; }

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
                items = MainViewModel.Default.SerialsViewModel.Items.ToList();

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
                var serials = new List<IntervalStatItem>();
                foreach (var serial in items)
                {
                    foreach (var season in serial.Seasons.Where(p => p.IsStartedWithDate))
                    {
                        if (ShowPaused == false &&
                            season.States.IsStateDone(Models.StateEnum.Paused) == true &&
                            season.States.IsStateDone(Models.StateEnum.Finished) == false) continue; 

                        var item = new IntervalStatItem()
                        {
                            Title = serial.RTitle + " " + season.Number.ToString(),
                            Start = season.StartDate.Value.Ticks
                        };
                        if (season.IsFinishedWithDate)
                        {
                            item.End = season.FinishDate.Value.Ticks;
                            item.IsFinished = true;
                            item.Fill = Resources.XamlResources.FinishedState_ColorBrush;
                        }
                        else
                        {
                            if(ShowPaused == true && season.States.IsStateDone(Models.StateEnum.Paused) == true)
                            {
                                item.End = DateTime.Today.Ticks;
                                item.Fill = Resources.XamlResources.PausedState_ColorBrush;
                            }
                            if (season.States.IsStateDone(Models.StateEnum.Paused) == false)
                            {
                                item.End = DateTime.Today.Ticks;
                                item.Fill = Resources.XamlResources.InQueueState_ColorBrush;
                            }                            
                        }
                        serials.Add(item);
                    }
                }
                Labels = new List<string>();
                var temp = serials.ToList();
                From = new DateTime(Year, 01, 01).Ticks;// items.First().Start;
                To = new DateTime(Year, 12, 31).Ticks; //items.Last().End;
                foreach (var item in temp)
                {
                    if ( (item.Start < From && item.End < From) ||
                         (item.Start > To && item.End > To) )
                        serials.Remove(item);
                }
                serials = serials.OrderByDescending(p => p.Start).ToList();
                foreach(var item in serials)
                {
                    Labels.Add(item.Title);
                }

                Series.Add(
                        new RowSeries
                        {
                            Values = new ChartValues<IntervalStatItem>(serials),
                            DataLabels = false,
                            Title = "",
                            LabelPoint = p => new DateTime((long)p.XStart).ToString("dd.MM.yyyy") + " - " + new DateTime((long)p.X).ToString("dd.MM.yyyy") +
                            " - " + new TimeSpan((long)(p.X - p.XStart)).TotalDays
                        });
                Reloaded?.Invoke(this, EventArgs.Empty);
            }
      }
    
        

        public YearStatisticsViewModel YearStatistics { get; set; } = new YearStatisticsViewModel();

        public SerialsStatisticsViewModel()
        {
            
        }

        public void Reload()
        {
            YearStatistics.ReloadSeries();
        }
    }
}
