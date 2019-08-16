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
    public class BookViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;        
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
                        [StateEnum.Started]  = "Начата",
                        [StateEnum.Finished] = "Закончена",
                        [StateEnum.Archived] = "В архиве"
                    };
                }
                return _statesDict;
            }
        }

        public ICommand CopyInfoCommand { get; set; }

        public int ID { get; set; }
        public string OTitle { get; set; }
        public string RTitle { get; set; }
        public string OAuthorName { get; set; }
        public string RAuthorName { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public RatingViewModel Rating { get; set; } = new RatingViewModel();
        public Dictionary<StateEnum, StateItemViewModel> States { get; set; } = new Dictionary<StateEnum, StateItemViewModel>();
        public StateItemViewModel LastState { get; set; }
        public bool IsStartedWithDate => States[StateEnum.Started].IsDone && States[StateEnum.Started].Date != null;
        public DateTime? StartDate => States[StateEnum.Started].Date;
        public bool IsFinishedWithDate => States[StateEnum.Finished].IsDone && States[StateEnum.Finished].Date != null;
        public DateTime? FinishDate => States[StateEnum.Finished].Date;        

        public BookViewModel()
        {            
            foreach (var state in StatesDict)
            {
                StateItemViewModel newstate = new StateItemViewModel(state.Key, state.Value, null, false);
                AddState(newstate);
            }
            States[StateEnum.Created].SetDone(true);
            //GetLastState();
            CopyInfoCommand = new RelayCommand<string>(
                (o) =>
                {
                    if (o is string) System.Windows.Clipboard.SetDataObject(o as string);
                },
                "К");
            Rating.PropertyChanged += (o, ea) => PropertyChanged?.Invoke(this, ea);
        }

        public BookViewModel(Book book)
        {
            ID = book.ID;
            OTitle = book.OTitle;
            RTitle = book.RTitle;
            OAuthorName = book.OAuthorName;
            RAuthorName = book.RAuthorName;
            Year = book.Year;
            Note = book.Note;
            Description = book.Description;
            Rating.Value = book.Rating;

            foreach (var state in StatesDict)
            {
                StateItemViewModel newstate = new StateItemViewModel(state.Key, state.Value, null);
                var st = book.States.FirstOrDefault(p => p.Name == state.Key);
                if(st != null)
                {
                    newstate.IsDone = true;
                    newstate.Date = st.Date;
                }
                AddState(newstate);
            }
            GetLastState();

            CopyInfoCommand = new RelayCommand<string>(
                (o) =>
                {
                    if (o is string) System.Windows.Clipboard.SetDataObject(o as string);
                },
                "К");
            Rating.PropertyChanged += (o, ea) => PropertyChanged?.Invoke(this, ea);
        }

        private void AddState(StateItemViewModel state)
        {
            state.PropertyChanged += (o, ea) => PropertyChanged?.Invoke(this, ea);
            state.StateChanged += (o, ea) => LastState = States.LastOrDefault(p => p.Value.IsDone).Value;
            States.Add(state.Name, state);
        }

        public StateItemViewModel GetLastState()
        {
            LastState = States.LastOrDefault(p => p.Value.IsDone).Value;
            return LastState;
        }

        public Book ToModel()
        {
            return new Book()
            {
                ID = this.ID,
                RTitle = this.RTitle,
                OTitle = this.OTitle,
                RAuthorName = this.RAuthorName,
                OAuthorName = this.OAuthorName,
                Year = this.Year,
                Note = this.Note,
                Description = this.Description,
                Rating = this.Rating.Value,
                States = States.Where(p => p.Value.IsDone).Select(p => p.Value.ToModel()).ToList()
            };
        }
    }
}
