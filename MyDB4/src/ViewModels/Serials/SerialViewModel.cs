using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using PropertyChanged;
using MyDB4.Models;
using System.ComponentModel;
using KLib;
using System.Xml.Linq;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class SerialViewModel : BaseViewModel
    {
        public int ID { get; set; }
        public string RTitle { get; set; }
        public string OTitle { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public bool HasLastSeason { get; set; }
        public DateTime? CreateDate { get; set; }
        public string FullTitle => RTitle + " " + OTitle;
        public int? KinopoiskId { get; set; }
        public RatingWithKinopoiskViewModel Rating { get; set; }
        bool _isChanged;
        public override bool IsChanged
        {
            get
            {
                return _isChanged;
            }
            set
            {
                if (_isChanged == value) return;
                if (value == false)
                {
                    foreach (var i in Seasons) i.IsChanged = false;
                    Rating.IsChanged = false;
                }                
                _isChanged = value;
                OnChanged(_isChanged);
            }
        }

        public ObservableCollection<SeasonViewModel> Seasons { get; set; }
        [DoNotSetChanged]
        public SeasonViewModel SelectedSeason { get; set; }

        public ICommand AddSeasonCommand { get; set; }
        public ICommand DeleteSeasonCommand { get; set; }
        public ICommand LoadRatingsCommand { get; set; }

        [DependsOn("HasLastSeason")]
        public bool IsClosed 
        {
            get
            {
                var last = Seasons.LastOrDefault();
                if(last == null) return false;

                if (HasLastSeason && last.States.IsStateDone(StateEnum.Finished)) return true;
                return false;
            }
        }

        public SerialViewModel() : base()
        {
            PrepareViewModel();

            CreateDate = DateTime.Today;
            Rating = new RatingWithKinopoiskViewModel(0, null, null);
            Rating.Changed += (o, ea) => IsChanged = true;
            IsChanged = false;
        }
        public SerialViewModel(Serial serial) : base()
        {
            PrepareViewModel();

            ID            = serial.ID;
            RTitle        = serial.RTitle;
            OTitle        = serial.OTitle;
            Note          = serial.Note;
            HasLastSeason = serial.HasLastSeason;
            CreateDate    = serial.CreateDate;            
            Description   = serial.Description;
            KinopoiskId = serial.KinopoiskId;
            Rating = new RatingWithKinopoiskViewModel(0, serial.KinopoiskRating, serial.IMDBRating);
            foreach (var i in serial.Seasons)
            {
                var st = new SeasonViewModel(i);
                st.Changed += (o, ea) => IsChanged = true;
                Seasons.Add(st);
            }
            Rating.Changed += (o, ea) => IsChanged = true;
            IsChanged = false;
        }
        protected override void PrepareViewModel()
        {
            Seasons = new ObservableCollection<SeasonViewModel>();
            AddSeasonCommand = new RelayCommand(AddSeason, () => HasLastSeason == false, "Добавить сезон");
            DeleteSeasonCommand = new RelayCommand(DeleteSeason, () => SelectedSeason != null, "Удалить сезон");
            LoadRatingsCommand = new RelayCommand(LoadRatings, () => KinopoiskId != null, "Загрузить оценки с Кинопоиска");
        }
        public Serial ToModel()
        {
            var serial = new Serial()
            {
                ID            = this.ID,
                RTitle        = this.RTitle,
                OTitle        = this.OTitle,
                Note          = this.Note,
                Description   = this.Description,
                HasLastSeason = this.HasLastSeason,
                CreateDate    = this.CreateDate,
                Seasons       = this.Seasons.Select(p => p.ToModel()).ToList(),
                KinopoiskId = KinopoiskId,
                KinopoiskRating = Rating.KinopoiskRating,
                IMDBRating = Rating.IMDBRating
            };            
            return serial;
        }
        void AddSeason()
        {
            SeasonViewModel season = new SeasonViewModel();
            season.Changed += (o, ea) => IsChanged = true;
            season.Number = Seasons.Any() ? Seasons.Max(p => p.Number) + 1 : 1;
            
            Seasons.Add(season);
            SelectedSeason = season;
        }
        void DeleteSeason()
        {
            if (SelectedSeason == null) return;
            Seasons.Remove(SelectedSeason);
            IsChanged = true;
        }
        private void LoadRatings()
        {
            if (KinopoiskId == null) return;
            string URLString = "https://rating.kinopoisk.ru/" + KinopoiskId + ".xml";
            try
            {
                XDocument doc = XDocument.Load(URLString);
                Rating.KinopoiskRating = (double)doc.Element("rating").Element("kp_rating");
                Rating.IMDBRating = (double)doc.Element("rating").Element("imdb_rating");
                var x = Rating.IsChanged;
            }
            catch
            {

            }
        }
    }
}
