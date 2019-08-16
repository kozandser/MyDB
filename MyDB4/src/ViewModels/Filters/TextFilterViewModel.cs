using KLib;
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
    public class TextFilterViewModel
    {
        public EventHandler TextChanged { get; set; }
        public ICommand ResetFilterCommand { get; set; }

        private string _text = "";
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value) return;
                _text = value;
                TextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public TextFilterViewModel()
        {
            ResetFilterCommand = new RelayCommand(ResetFilter, "Сбросить фильтр");
        }

        public void ResetFilter()
        {
            Text = String.Empty;
        }
    }
}
