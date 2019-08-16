using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PropertyChanged;
using MyDB4.Models;
using System.ComponentModel;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class SeasonViewModel : BaseViewModel
    {
        static Dictionary<StateEnum, string> _statesDict;
        public static Dictionary<StateEnum, string> StatesDict
        {
            get
            {
                if (_statesDict == null)
                {
                    _statesDict = new Dictionary<StateEnum, string>()
                    {
                        [StateEnum.Created] = "Добавлен",
                        [StateEnum.InQueue] = "В очереди",
                        [StateEnum.Started] = "Начат",
                        [StateEnum.Paused]  = "Приостановлен",
                        [StateEnum.Finished] = "Закончен"
                    };
                }
                return _statesDict;
            }
        }

        public int Number { get; set; }
        public int Episodes { get; set; }
        public string Note { get; set; }

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
                }
                _isChanged = value;
                OnChanged(_isChanged);
            }
        }

        public SeasonViewModel()
        {
            States = new StateItemsViewModel(StatesDict);
            States.Changed += (o, ea) => IsChanged = true;
            IsChanged = false;
        }
        public SeasonViewModel(Season season)
        {
            Number   = season.Number;
            Episodes = season.Episodes;
            Note     = season.Note;
            States = new StateItemsViewModel(StatesDict);

            foreach (var s in States.Items)
            {
                var st = season.States.FirstOrDefault(p => p.Name == s.Key);
                if (st != null)
                {
                    s.Value.IsDone = true;
                    s.Value.Date = st.Date;
                    s.Value.IsChanged = false;
                }
            }
            States.GetLastState();

            States.Changed += (o, ea) => IsChanged = true;
            IsChanged = false;
        }

        public Season ToModel()
        {
            var season = new Season()
            {
                Number   = Number,
                Episodes = Episodes,
                Note     = Note,
                States   = States.GetDoneStates().Select(p => p.ToModel()).ToList()
            };            
            return season;
        }        
    }
}
