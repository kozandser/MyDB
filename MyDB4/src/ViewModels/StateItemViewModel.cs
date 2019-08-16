using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using System.Windows.Input;
using System.ComponentModel;
using MyDB4.Models;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class StateItemViewModel : ChangedViewModel
    {
        public event EventHandler StateChanged;
        private bool _isDone;
        public bool IsDone
        {
            get { return _isDone; }
            set
            {
                if (_isDone == value) return;
                _isDone = value;
                SetDone(value);
                OnStateChanged();
            }
        }
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
        void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public StateEnum Name { get; set; }
        public string Caption { get; set; }
        public DateTime? Date { get; set; }

        public StateItemViewModel(StateEnum name, string caption, DateTime? date, bool isDone = false)
        {
            Name = name;
            Caption = caption;
            IsDone = isDone;
            Date = date;
            IsChanged = false;
        }

        public StateItem ToModel()
        {
            var state = new StateItem()
            {
                Name = this.Name,
                Date = this.Date
            };
            return state;
        }

        public void SetDone(bool flag)
        {          
            IsDone = flag;
            SetDate();
        }

        public void SetDate()
        {
            if (IsDone) Date = DateTime.Today;
            else Date = null;
        }
    }

    public class StateItemsViewModel : ChangedViewModel
    {
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
                    foreach (var i in Items)
                    {
                        i.Value.IsChanged = false;
                    }
                }
                _isChanged = value;
                OnChanged(_isChanged);
            }
        }
        public Dictionary<StateEnum, StateItemViewModel> Items { get; set; } = new Dictionary<StateEnum, StateItemViewModel>();
        public StateItemViewModel LastState { get; set; }

        public StateItemsViewModel(Dictionary<StateEnum, string> dict)
        {
            foreach (var state in dict)
            {
                StateItemViewModel newstate = new StateItemViewModel(state.Key, state.Value, null, false);
                AddState(newstate);
            }
            Items[StateEnum.Created].SetDone(true);
            IsChanged = false;
        }
        private void AddState(StateItemViewModel state)
        {
            state.Changed += (o, ea) => IsChanged = true;
            state.StateChanged += (o, ea) => LastState = Items.LastOrDefault(p => p.Value.IsDone).Value;
            Items.Add(state.Name, state);
        }
        public StateItemViewModel GetLastState()
        {
            LastState = Items.LastOrDefault(p => p.Value.IsDone).Value;
            return LastState;
        }
        public StateItemViewModel[] GetDoneStates()
        {
            return Items.Where(p => p.Value.IsDone).Select(p => p.Value).ToArray();
        }
        public bool IsStateDone(StateEnum state)
        {
            return Items[state].IsDone;
        }
        public bool IsStateDoneWithDate(StateEnum state)
        {
            return Items[state].IsDone && Items[state].Date != null;
        }
    }
}
