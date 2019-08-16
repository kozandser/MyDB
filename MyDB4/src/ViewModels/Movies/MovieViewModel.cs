using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyDB4.Models;
using PropertyChanged;
using System.ComponentModel;
using System.Xml.Linq;
using KLib;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class MovieViewModel : BaseViewModel
    {
        private static Dictionary<TypeEnum, string> _typesDict;
        private static Dictionary<StateEnum, string> _statesDict;
        public static Dictionary<TypeEnum, string> TypesDict
        {
            get
            {
                if (_typesDict == null)
                {
                    _typesDict = new Dictionary<TypeEnum, string>()
                    {
                        [TypeEnum.NA]        = "н/д",
                        [TypeEnum.ForMe]     = "Для меня",
                        [TypeEnum.ForParent] = "Для нас",
                        [TypeEnum.ForFamily] = "Семейный"
                    };
                }
                return _typesDict;
            }
        }
        public static Dictionary<StateEnum, string> StatesDict
        {
            get
            {
                if (_statesDict == null)
                {
                    _statesDict = new Dictionary<StateEnum, string>()
                    {
                        [StateEnum.Created]  = "Добавлен",
                        [StateEnum.InQueue]  = "В очереди",
                        [StateEnum.Finished] = "Просмотрен",
                        [StateEnum.Reviewed] = "Рецензия",
                        [StateEnum.Closed]   = "Закрыт",
                        [StateEnum.Archived] = "В архиве"
                    };
                }
                return _statesDict;
            }
        }      

        public int ID { get; set; }
        public string RTitle { get; set; }
        public string OTitle { get; set; }
        public int Year { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public TypeEnum Type { get; set; }
        public string FullTitle => RTitle + " " + OTitle;       

        public int? KinopoiskId { get; set; }

        public RatingWithKinopoiskViewModel Rating { get; set; }        
        public StateItemsViewModel States { get; set; }

        public bool IsFinishedWithDate => States.IsStateDoneWithDate(StateEnum.Finished);
        public DateTime? FinishDate => States.Items[StateEnum.Finished].Date;

        bool _isChanged;
        public override bool IsChanged
        {
            get
            {
                return _isChanged;
            }
            set
            {
                if(_isChanged == value) return;
                if (value == false)
                {
                    States.IsChanged = false;
                    Rating.IsChanged = false;
                }
                _isChanged = value;
                OnChanged(_isChanged);
            }
        }        

        public MovieViewModel() : base()
        {            
            Year = DateTime.Today.Year;
            Type = TypeEnum.NA;
            Rating = new RatingWithKinopoiskViewModel();
            States = new StateItemsViewModel(StatesDict);

            PrepareViewModel();
        }       

        public MovieViewModel(Movie movie) : base()
        {
            ID = movie.ID;
            RTitle = movie.RTitle;
            OTitle = movie.OTitle;
            Year = movie.Year;
            Note = movie.Note;
            Description = movie.Description;
            Link = movie.Link;
            Type = movie.Type;
            KinopoiskId = movie.KinopoiskId;
            Rating = new RatingWithKinopoiskViewModel(movie.Rating, movie.KinopoiskRating, movie.IMDBRating);
            States = new StateItemsViewModel(StatesDict);

            foreach(var s in States.Items)
            {
                var st = movie.States.FirstOrDefault(p => p.Name == s.Key);
                if (st != null)
                {
                    s.Value.IsDone = true;
                    s.Value.Date = st.Date;
                    s.Value.IsChanged = false;
                }
            }
            States.GetLastState();

            PrepareViewModel();
        }

        protected override void PrepareViewModel()
        {           
            Rating.Changed += (o, ea) => IsChanged = true;
            States.Changed += (o, ea) => IsChanged = true;
            IsChanged = false;
        }

        public Movie ToModel()
        {
            return new Movie()
            {
                ID = ID,
                RTitle = RTitle,
                OTitle = OTitle,
                Year = Year,
                Note = Note,
                Description = Description,
                Link = Link,
                Rating = Rating.Value,
                Type = Type,
                States = States.GetDoneStates().Select(p => p.ToModel()).ToList(),
                KinopoiskId = KinopoiskId,
                KinopoiskRating = Rating.KinopoiskRating,
                IMDBRating = Rating.IMDBRating
            };
        }
    }
}
