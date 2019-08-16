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
    public class TypeFilterViewModel
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

        public TypeEnum Name { get; set; }
        public string Caption { get; set; }
        public EventHandler Checked { get; set; }

        public TypeFilterViewModel(TypeEnum name, string caption, bool isChecked = false)
        {
            Name = name;
            Caption = caption;
            IsChecked = isChecked;
        }

        public TypeFilter ToModel()
        {
            return new TypeFilter()
            {
                Name = Name,
                IsChecked = IsChecked
            };
        }
    }
}
