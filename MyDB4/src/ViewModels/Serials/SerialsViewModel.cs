using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Data;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using PropertyChanged;
using MyDB4.Models;
using MyDB4;
using System.Collections;
using KLib;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class SerialsViewModel : BaseListViewModel<SerialViewModel>
    {
        public List<int> SeasonNumbers { get; set; } = Enumerable.Range(1, 20).ToList();
        public List<int> SeasonEpisodes { get; set; } = Enumerable.Range(1, 30).ToList();

        public ICommand LoadRatingsCommand { get; set; }
        public bool IsRatingsLoading { get; set; }

        public bool IsActive { get; set; }
        public bool HasLastSeason { get; set; }
        public bool IsClosed { get; set; }

        public SerialsViewModel() : base(@"Storage\Serials.json.gz")
        {
            Items.ShapeView().OrderBy(p => p.RTitle).Apply();

            //SetLastSaveTime(); 
            //BuildFilters();
            LoadFromStorage();
            ResetNeedSave();
            LoadSettings();

            SelectedItem = View.Cast<SerialViewModel>().FirstOrDefault(p => p.ID == LastSelectedID) ?? View.Cast<SerialViewModel>().FirstOrDefault();
            if (SelectedItem == null) View.MoveCurrentToFirst();
            else View.MoveCurrentTo(SelectedItem);

            LoadRatingsCommand = new RelayCommand(LoadRatings, () => IsRatingsLoading == false, "Загрузить оценки с Кинопоиска для всех");
        }
        private async void LoadRatings()
        {
            Task task = Task.Run(() =>
            {
                IsRatingsLoading = true;
                foreach (var i in Items)
                {
                    i.LoadRatingsCommand.Execute(null);
                }
                IsRatingsLoading = false;
            });
        }
        protected override bool Filter(object item)
        {
            bool state = false;
            bool rname = false;
            bool oname = false;
            var obj = item as SerialViewModel;
            if (obj == null) return false;

            if (IsActive)
                if (obj.HasLastSeason == false && obj.IsClosed == false) state = true;

            if (HasLastSeason)
                if (obj.HasLastSeason == true) state = true;

            if (IsClosed)
                if (obj.IsClosed) state = true;

            if (string.IsNullOrEmpty(TextFilter.Text)) rname = true;
            if (!string.IsNullOrEmpty(obj.RTitle) && obj.RTitle.IndexOf(TextFilter.Text, StringComparison.OrdinalIgnoreCase) != -1)
            {
                rname = true;
            }
            if (!string.IsNullOrEmpty(obj.OTitle) && obj.OTitle.IndexOf(TextFilter.Text, StringComparison.OrdinalIgnoreCase) != -1)
            {
                oname = true;
            }

            return (rname || oname);
        }

        public void LoadFromStorage()
        {
            try
            {
                var storage = Repository.LoadObjectFromStorage<Serials>(STORAGE_FILE);
                foreach (var store in storage.Items)
                {
                    var ovm = new SerialViewModel(store);
                    AddSerial(ovm);
                }
            }
            catch
            {
                var storage = Repository.GetObjectsFromStorage<Serial>(STORAGE_FILE);
                foreach (var store in storage)
                {
                    var ovm = new SerialViewModel(store);
                    AddSerial(ovm);
                }
            }
        }
        public void LoadSettings()
        {
            var settings = MainViewModel.Default.AppSettings;            
            LastSelectedID = settings.SerialsSettings.SelectedSerialID;
        }
        public void SaveSettings()
        {
            var settings = MainViewModel.Default.AppSettings;
            settings.SerialsSettings.SelectedSerialID = SelectedItem?.ID ?? 0;
            //settings.SaveAppSettings();
        }

        private void AddSerial(SerialViewModel serial)
        {
            serial.PropertyChanged += (o, ea) =>
            {
                if (ea != null)
                {
                    if (ea.PropertyName == "SelectedSeason") return;
                }
                NeedSave = true;
            };
            Items.Add(serial);
        }
        private void DeleteSerial(SerialViewModel serial)
        {
            Items.Remove(serial);
        }

        public override void Add()
        {
            IsActive = true;
            TextFilter.ResetFilter();

            SerialViewModel serial = new SerialViewModel()
            {
                ID = Items.Any() ? Items.Max(p => p.ID) + 1 : 1
            };
            AddSerial(serial);
            SelectedItem = serial;
            ApplyFilter();

            NeedSave = true;
        }
        public override void AddNewFromClipboard()
        {
            var cb = System.Windows.Clipboard.GetText();
            if (String.IsNullOrEmpty(cb) == false)
            {
                Add();
                SelectedItem.RTitle = cb;
            }
        }
        public override void Delete()
        {
            if (SelectedItem == null) return;
            DeleteSerial(SelectedItem);
            NeedSave = true;
        }
        
        public override void Save()
        {
            var storage = new Serials();
            storage.Items = Items.Select(p => p.ToModel()).ToList();
            storage.SaveObjectToStorage(STORAGE_FILE);
            SetLastSaveTime();
            SetLastSaveTime();
            ResetNeedSave();
        }
        public override void Backup()
        {
            string file = $"Serials_{DateTime.Today:yyyyMMdd}.json.gz";
            file = System.IO.Path.Combine("Backups", file);

            var storage = new Serials();
            storage.Items = Items.Select(p => p.ToModel()).ToList();
            storage.SaveObjectToStorage(file);
        }
        public override void ApplyFilter()
        {
            View?.Refresh();
            if (SelectedItem == null) View.MoveCurrentToFirst();
            Refreshed?.Invoke();
        }

        public override void MultiDelete(IList items)
        {
            if (items == null) return;
            var r = ConfirmationShow?.Invoke("Удалить?", "Выделенные сериалы будут удалены.");
            if (r == true)
            {
                List<SerialViewModel> list = new List<SerialViewModel>();
                foreach (SerialViewModel i in items)
                {
                    list.Add(i);
                }
                foreach (SerialViewModel i in list)
                {
                    DeleteSerial(i);
                }
                NeedSave = true;
            }
        }
        protected override void ResetNeedSave()
        {
            foreach (var i in Items)
            {
                foreach (var j in i.Seasons) j.IsChanged = false;
                i.IsChanged = false;                
            }
            NeedSave = false;
        }






    }
}
