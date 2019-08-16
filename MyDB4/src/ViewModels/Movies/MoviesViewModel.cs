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
using System.Threading;
using System.Net;
using KLib;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class MoviesViewModel : BaseListViewModel<MovieViewModel>
    {
        public List<int> Years { get; set; } = Enumerable.Range(1850, 200).ToList();

        public ICommand LoadRatingsCommand { get; set; }
        public bool IsRatingsLoading { get; set; }

        public MoviesViewModel() : base(@"Storage\Movies.json.gz")
        {
            Items.ShapeView().OrderBy(p => p.RTitle).Apply();

            BuildFilters();
            LoadFromStorage();
            ResetNeedSave();
            LoadSettings();

            SelectedItem = View.Cast<MovieViewModel>().FirstOrDefault(p => p.ID == LastSelectedID) ?? View.Cast<MovieViewModel>().FirstOrDefault();
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
                    i.Rating.LoadRatingsCommand.Execute(null);
                }
                IsRatingsLoading = false;
            });
        }
        protected override bool Filter(object item)
        {
            if (item == null) return false;
            var obj = item as MovieViewModel;
            if (obj == null) return false;

            bool state = true;
            bool type = true;
            bool rname = false;
            bool oname = false;

            state = StateFilters[obj.States.LastState.Name].IsChecked;
            type = TypeFilters[obj.Type].IsChecked;

            if (string.IsNullOrEmpty(TextFilter.Text)) rname = true;
            if (!string.IsNullOrEmpty(obj.RTitle) && obj.RTitle.IndexOf(TextFilter.Text, StringComparison.OrdinalIgnoreCase) != -1)
            {
                rname = true;
            }
            if (!string.IsNullOrEmpty(obj.OTitle) && obj.OTitle.IndexOf(TextFilter.Text, StringComparison.OrdinalIgnoreCase) != -1)
            {
                oname = true;
            }

            return state && type && (rname || oname);
        }
        public void BuildFilters()
        {
            StateFilters = new Dictionary<StateEnum, StateFilterViewModel>();
            foreach (var s in MovieViewModel.StatesDict)
            {
                var s2 = new StateFilterViewModel(s.Key, s.Value);
                s2.Checked += (o, ea) => ApplyFilter();
                StateFilters.Add(s.Key, s2);
            }

            TypeFilters = new Dictionary<TypeEnum, TypeFilterViewModel>();
            foreach (var s in MovieViewModel.TypesDict)
            {
                var s2 = new TypeFilterViewModel(s.Key, s.Value);
                s2.Checked += (o, ea) => ApplyFilter();
                TypeFilters.Add(s.Key, s2);
            }
        }
        public void LoadFromStorage()
        {
            var storage = Repository.LoadObjectFromStorage<Movies>(STORAGE_FILE);
            foreach (var store in storage.Items)
            {
                var ovm = new MovieViewModel(store);
                AddMovie(ovm);
            }

            //try
            //{
            //    var storage = Repository.LoadObjectFromStorage<Movies>(STORAGE_FILE);
            //    foreach (var store in storage.Items)
            //    {
            //        var ovm = new MovieViewModel(store);
            //        AddMovie(ovm);
            //    }
            //}
            //catch
            //{
            //    var storage = Repository.GetObjectsFromStorage<Movie>(STORAGE_FILE);
            //    foreach (var store in storage)
            //    {
            //        var ovm = new MovieViewModel(store);
            //        AddMovie(ovm);
            //    }
            //}            
        }
        public void LoadSettings()
        {
            var settings = MainViewModel.Default.AppSettings;
            foreach(var f in StateFilters)
            {
                var a = settings.MoviesSettings.StateFilters.FirstOrDefault(p => p.Name == f.Value.Name);
                if (a == null) f.Value.IsChecked = true;
                else f.Value.IsChecked = a.IsChecked;
            }
            foreach (var f in TypeFilters)
            {
                var a = settings.MoviesSettings.TypeFilters.FirstOrDefault(p => p.Name == f.Value.Name);
                if (a == null) f.Value.IsChecked = true;
                else f.Value.IsChecked = a.IsChecked;
            }
            LastSelectedID = settings.MoviesSettings.SelectedMovieID;
        }
        public void SaveSettings()
        {
            var settings = MainViewModel.Default.AppSettings;
            settings.MoviesSettings.StateFilters = StateFilters.Select(p => p.Value.ToModel()).ToList();
            settings.MoviesSettings.TypeFilters = TypeFilters.Select(p => p.Value.ToModel()).ToList();
            settings.MoviesSettings.SelectedMovieID = SelectedItem?.ID ?? 0;
            //settings.SaveAppSettings();
        }
        private void AddMovie(MovieViewModel movie)
        {
            movie.Changed += (o, ea) => NeedSave = true;

            Items.Add(movie);            
        }
        private void DeleteMovie(MovieViewModel movie)
        {
            Items.Remove(movie);            
        }
        public override void Add()
        {
            TypeFilters[TypeEnum.NA].IsChecked = true;
            StateFilters[StateEnum.Created].IsChecked = true;
            TextFilter.ResetFilter();

            MovieViewModel movie = new MovieViewModel()
            {
                ID = Items.Any() ? Items.Max(p => p.ID) + 1 : 1
            };

            AddMovie(movie);
            SelectedItem = movie;
            ApplyFilter();

            NeedSave = true;
        }

        public override void AddNewFromClipboard()
        {
            var cb = System.Windows.Clipboard.GetText();
            if(String.IsNullOrEmpty(cb) == false)
            {
                Add();
                SelectedItem.RTitle = cb;
            }
        }

        public override void Delete()
        {
            if (SelectedItem == null) return;
            DeleteMovie(SelectedItem);
            NeedSave = true;
        }

        public override void Save()
        {
            var storage = new Movies();
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
            string file = $"Movies_{DateTime.Today:yyyyMMdd}.json.gz";
            file = System.IO.Path.Combine("Backups", file);            

            var storage = new Movies();
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
            foreach (MovieViewModel i in items)
            {
                i.States.Items[StateEnum.Closed].IsDone = true;
            }            
        }

        public override void MultiArchive(IList items)
        {
            if (items == null) return;
            foreach (MovieViewModel i in items)
            {
                i.States.Items[StateEnum.Archived].IsDone = true;
            }
        }

        public override void MultiDelete(IList items)
        {
            if (items == null) return;
            var r = ConfirmationShow?.Invoke("Удалить?", "Выделенные фильмы будут удалены.");
            if (r == true)
            {
                List<MovieViewModel> list = new List<MovieViewModel>();
                foreach (MovieViewModel i in items)
                {
                    list.Add(i);
                }
                foreach (MovieViewModel i in list)
                {
                    DeleteMovie(i);
                }
                NeedSave = true;
            }
        }
    }
}
