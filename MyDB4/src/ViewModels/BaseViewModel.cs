using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public abstract class ChangedViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<bool> Changed;
        public virtual bool IsChanged { get; set; }
        public void OnChanged(bool changed)
        {
            Changed?.Invoke(this, changed);
        }
    }

    [ImplementPropertyChanged]
    public abstract class BaseViewModel : ChangedViewModel
    {
        public ICommand CopyInfoCommand { get; set; }
        public BaseViewModel()
        {
            CopyInfoCommand = new RelayCommand<string>(
                (o) =>
                {
                    if (o == null) return;
                    System.Windows.Clipboard.SetDataObject(o);
                },
                "К");
        }

        protected virtual void PrepareViewModel()
        {
            CopyInfoCommand = new RelayCommand<string>(
                (o) =>
                {
                    if (o == null) return;
                    System.Windows.Clipboard.SetDataObject(o);
                },
                "К");
        }
    }


}
