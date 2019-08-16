using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using PropertyChanged;
using System.Windows.Input;
using KLib;
using System.Xml.Linq;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class RatingViewModel : ChangedViewModel
    {
        public double Value { get; set; }
        public int HighRating => (int)Value;
        public int LowRating => (int)((Value - (int)Value) * 10);
        [DependsOn("Value")]
        public int RatingClass => GetRatingClass(Value);


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
                _isChanged = value;
                OnChanged(_isChanged);
            }
        }

        public RatingViewModel(double value = 0)
        {
            Value = value;
        }
        public static int GetRatingClass(double rating)
        {
            if (rating > 0 && rating < 3) return 1;
            else if (rating >= 3 && rating < 4) return 2;
            else if (rating >= 4) return 3;
            else return 0;
        }
    }
    [ImplementPropertyChanged]
    public class RatingWithKinopoiskViewModel : RatingViewModel
    {
        public double? KinopoiskRating { get; set; }
        public double? IMDBRating { get; set; }
        public int KinopiskRatingEquality =>
            Value == 0 ? 0 :
            Value * 2 > KinopoiskRating ? -1 :
            Value * 2 < KinopoiskRating ? 1 :
            0;
        public int IMDBRatingEquality =>
            Value == 0 ? 0 :
            Value * 2 > IMDBRating ? -1 :
            Value * 2 < IMDBRating ? 1 :
            0;

        public ICommand LoadRatingsCommand { get; set; }

        public RatingWithKinopoiskViewModel(double value = 0) : base(value)
        {
            LoadRatingsCommand = new RelayCommand<int?>(LoadRatings, (p) => p != null, "Загрузить оценки с Кинопоиска");
        }
        public RatingWithKinopoiskViewModel(double value, double? kinopoiskRating, double? iMDBRating) : this(value)
        {
            KinopoiskRating = kinopoiskRating;
            IMDBRating = iMDBRating;
        }
        private void LoadRatings(int? kinopoiskId)
        {
            if (kinopoiskId == null) return;

            string URLString = "https://rating.kinopoisk.ru/" + kinopoiskId + ".xml";
            try
            {
                XDocument doc = XDocument.Load(URLString);
                KinopoiskRating = (double)doc.Element("rating").Element("kp_rating");
                IMDBRating = (double)doc.Element("rating").Element("imdb_rating");
            }
            catch
            {

            }
        }

    }
}
