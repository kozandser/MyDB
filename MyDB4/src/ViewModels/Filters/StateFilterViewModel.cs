using MyDB4.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class StateFilterViewModel
    {
        public bool IsCheckedSuppressed { get; set; }
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked == value) return;
                _isChecked = value;
                if(!IsCheckedSuppressed) Checked?.Invoke(this, EventArgs.Empty);
            }
        }

        public StateEnum Name { get; set; }
        public string Caption { get; set; }
        public EventHandler Checked { get; set; }

        public StateFilterViewModel(StateFilter sf, string caption)
        {
            Name = sf.Name;
            IsChecked = sf.IsChecked;
            Caption = caption;
        }

        public StateFilterViewModel(StateEnum name, string caption, bool isChecked = false)
        {
            Name = name;
            Caption = caption;
            IsChecked = isChecked;
        }

        public StateFilter ToModel()
        {
            return new StateFilter()
            {
                Name = Name,
                IsChecked = IsChecked
            };
        }
    }
}
