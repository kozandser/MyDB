using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyDB4.Models;
using PropertyChanged;
using System.ComponentModel;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class GameViewModel : BaseViewModel
    {
    
        private static Dictionary<StateEnum, string> _statesDict;
        public static Dictionary<StateEnum, string> StatesDict
        {
            get
            {
                if (_statesDict == null)
                {
                    _statesDict = new Dictionary<StateEnum, string>()
                    {
                        [StateEnum.Created]  = "Добавлена",
                        [StateEnum.InQueue]  = "В очереди",
                        [StateEnum.Started]  = "Начата",
                        [StateEnum.Paused]   = "Приостановлена",
                        [StateEnum.Finished] = "Закончена",
                        [StateEnum.Archived] = "В архиве"
                    };
                }
                return _statesDict;
            }
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public TimeSpan? PlayTime { get; set; }
        public int PlayHours
        {
            set
            {
                if (PlayTime == null) PlayTime = new TimeSpan();
                PlayTime = new TimeSpan(value, PlayTime.Value.Minutes, 0);
            }
            get
            {
                return PlayTime == null ? 0 : (int)PlayTime.Value.TotalHours;
            }
        }
        public int PlayMinutes
        {
            set
            {
                if (PlayTime == null) PlayTime = new TimeSpan();
                PlayTime = new TimeSpan((int)PlayTime.Value.TotalHours, value, 0);
                if (value > 59) PlayMinutes = 0;
            }
            get
            {
                return PlayTime == null ? 0 : PlayTime.Value.Minutes;
            }
        }

        public RatingViewModel Rating { get; set; }
        public StateItemsViewModel States { get; set; }

        public bool IsStartedWithDate => States.IsStateDoneWithDate(StateEnum.Started);
        public DateTime? StartDate => States.Items[StateEnum.Started].Date;
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
                if (_isChanged == value) return;
                if (value == false)
                {
                    States.IsChanged = false;
                    Rating.IsChanged = false;
                }
                _isChanged = value;
                OnChanged(_isChanged);
            }
        }

        public GameViewModel() : base()
        {
            Rating = new RatingWithKinopoiskViewModel();
            States = new StateItemsViewModel(StatesDict);
            PrepareViewModel();
        }

        public GameViewModel(Game game) : base()
        {
            ID = game.ID;
            Title = game.Title;
            Note = game.Note;
            Description = game.Description;
            PlayTime = game.PlayTime;
            Rating = new RatingViewModel(game.Rating);
            States = new StateItemsViewModel(StatesDict);
            foreach (var s in States.Items)
            {
                var st = game.States.FirstOrDefault(p => p.Name == s.Key);
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
                
        public Game ToModel()
        {
            return new Game()
            {
                ID = this.ID,
                Title = this.Title,
                Note = this.Note,
                Description = this.Description,
                Rating = this.Rating.Value,
                PlayTime = this.PlayTime,
                States = States.GetDoneStates().Select(p => p.ToModel()).ToList()
            };
        }
    }
}
