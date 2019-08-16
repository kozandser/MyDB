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
using System.Collections;
using KLib;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class GamesViewModel : BaseListViewModel<GameViewModel>
    {        
        public List<int> Hours { get; set; } = Enumerable.Range(0, 300).ToList();
        public List<int> Minutes { get; set; } = Enumerable.Range(0,60).ToList();

        public GamesViewModel() : base(@"Storage\Games.json.gz")
        {
            Items.ShapeView().OrderBy(p => p.Title).Apply();

            BuildFilters();
            LoadFromStorage();
            ResetNeedSave();
            LoadSettings();

            SelectedItem = View.Cast<GameViewModel>().FirstOrDefault(p => p.ID == LastSelectedID) ?? View.Cast<GameViewModel>().FirstOrDefault();
            if (SelectedItem == null) View.MoveCurrentToFirst();
            else View.MoveCurrentTo(SelectedItem);
        }

        protected override bool Filter(object item)
        {
            if (item == null) return false;
            var obj = item as GameViewModel;
            if (obj == null) return false;

            bool state = true;
            bool name = false;

            state = StateFilters[obj.States.LastState.Name].IsChecked;

            if (string.IsNullOrEmpty(TextFilter.Text)) name = true;
            if (!string.IsNullOrEmpty(obj.Title) && obj.Title.IndexOf(TextFilter.Text, StringComparison.OrdinalIgnoreCase) != -1)
            {
                name = true;
            }           

            return state && name;
        }

        public void BuildFilters()
        {
            StateFilters = new Dictionary<StateEnum, StateFilterViewModel>();
            foreach (var s in GameViewModel.StatesDict)
            {
                var s2 = new StateFilterViewModel(s.Key, s.Value);
                s2.Checked += (o, ea) => ApplyFilter();
                StateFilters.Add(s.Key, s2);
            }
        }

        public void LoadFromStorage()
        {
            var storage = Repository.LoadObjectFromStorage<Games>(STORAGE_FILE);
            foreach (var store in storage.Items)
            {
                var ovm = new GameViewModel(store);
                AddGame(ovm);
            }
        }

        public void LoadSettings()
        {
            var settings = MainViewModel.Default.AppSettings;
            foreach(var f in StateFilters)
            {
                var a = settings.GamesSettings.StateFilters.FirstOrDefault(p => p.Name == f.Value.Name);
                if (a == null) f.Value.IsChecked = true;
                else f.Value.IsChecked = a.IsChecked;
            }
            LastSelectedID = settings.GamesSettings.SelectedGameID;
        }

        public void SaveSettings()
        {
            var settings = MainViewModel.Default.AppSettings;
            settings.GamesSettings.StateFilters = StateFilters.Select(p => p.Value.ToModel()).ToList();
            settings.GamesSettings.SelectedGameID = SelectedItem?.ID ?? 0;
        }

        private void AddGame(GameViewModel game)
        {
            game.Changed += (o, ea) => NeedSave = true;
            Items.Add(game);            
        }
        private void DeleteGame(GameViewModel game)
        {
            Items.Remove(game);            
        }

        public override void Add()
        {            
            StateFilters[StateEnum.Created].IsChecked = true;
            TextFilter.ResetFilter();

            GameViewModel game = new GameViewModel()
            {
                ID = Items.Any() ? Items.Max(p => p.ID) + 1 : 1
            };

            AddGame(game);
            SelectedItem = game;
            ApplyFilter();

            NeedSave = true;
        }

        public override void AddNewFromClipboard()
        {
            var cb = System.Windows.Clipboard.GetText();
            if(String.IsNullOrEmpty(cb) == false)
            {
                Add();
                SelectedItem.Title = cb;
            }
        }

        public override void Delete()
        {
            if (SelectedItem == null) return;
            DeleteGame(SelectedItem);
            NeedSave = true;
        }

        public override void Save()
        {
            var storage = new Games();
            storage.Items = Items.Select(p => p.ToModel()).ToList();
            storage.SaveObjectToStorage(STORAGE_FILE);
            SetLastSaveTime();
            ResetNeedSave();
        }
        protected override void ResetNeedSave()
        {
            foreach (var i in Items)
            {
                i.IsChanged = false;
            }
            NeedSave = false;
        }

        public override void Backup()
        {
            string file = $"Games_{DateTime.Today:yyyyMMdd}.json.gz";
            file = System.IO.Path.Combine("Backups", file);            

            var storage = new Games();
            storage.Items = Items.Select(p => p.ToModel()).ToList();
            storage.SaveObjectToStorage(file);
        }

        public override void ApplyFilter()
        {
            View?.Refresh();
            if (SelectedItem == null) View.MoveCurrentToFirst();
            Refreshed?.Invoke();
        }

        public override void MultiClose(IList items)
        {
            if (items == null) return;
            foreach (GameViewModel i in items)
            {
                i.States.Items[StateEnum.Closed].IsDone = true;
            }            
        }

        public override void MultiArchive(IList items)
        {
            if (items == null) return;
            foreach (GameViewModel i in items)
            {
                i.States.Items[StateEnum.Archived].IsDone = true;
            }
        }

        public override void MultiDelete(IList items)
        {
            if (items == null) return;
            var r = ConfirmationShow?.Invoke("Удалить?", "Выделенные игры будут удалены.");
            if (r == true)
            {
                List<GameViewModel> list = new List<GameViewModel>();
                foreach (GameViewModel i in items)
                {
                    list.Add(i);
                }
                foreach (GameViewModel i in list)
                {
                    DeleteGame(i);
                }
                NeedSave = true;
            }
        }
    }
}
