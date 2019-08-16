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

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class BooksViewModel : BaseListViewModel
    {
        public ObservableCollection<BookViewModel> Items { get; set; }
        public BookViewModel SelectedItem { get; set; }
        public List<int> Years { get; set; } = Enumerable.Range(1970, DateTime.Today.Year - 1969).OrderByDescending(p => p).ToList();
        public List<string> Authors { get; set; }

        public BooksViewModel() : base()
        {
            STORAGE_FILE = @"Storage\Books.json.gz";
            Items = new ObservableCollection<BookViewModel>();
            View = CollectionViewSource.GetDefaultView(Items);
            View.SortDescriptions.Add(new SortDescription("RTitle", ListSortDirection.Ascending));
            View.Filter = new Predicate<object>(Filter);

            SetLastSaveTime();

            BuildFilters();
            LoadFromStorage();
            LoadSettings();

            SelectedItem = View.Cast<BookViewModel>().FirstOrDefault(p => p.ID == LastSelectedID) ?? View.Cast<BookViewModel>().FirstOrDefault();
            if (SelectedItem == null) View.MoveCurrentToFirst();
            else View.MoveCurrentTo(SelectedItem);
        }

        public bool Filter(object item)
        {
            if (item == null) return false;
            var obj = item as BookViewModel;
            if (obj == null) return false;

            bool state = true;
            bool rname = false;
            bool oname = false;

            state = StateFilters[obj.LastState.Name].IsChecked;

            if (string.IsNullOrEmpty(TextFilter.Text)) rname = true;
            if (!string.IsNullOrEmpty(obj.RTitle) && obj.RTitle.IndexOf(TextFilter.Text, StringComparison.OrdinalIgnoreCase) != -1)
            {
                rname = true;
            }
            if (!string.IsNullOrEmpty(obj.OTitle) && obj.OTitle.IndexOf(TextFilter.Text, StringComparison.OrdinalIgnoreCase) != -1)
            {
                oname = true;
            }

            return state && (rname || oname);
        }

        public void BuildFilters()
        {
            StateFilters = new Dictionary<StateEnum, StateFilterViewModel>();
            foreach (var s in BookViewModel.StatesDict)
            {
                var s2 = new StateFilterViewModel(s.Key, s.Value);
                s2.Checked += (o, ea) => ApplyFilter();
                StateFilters.Add(s.Key, s2);
            }
        }

        public void LoadFromStorage()
        {
            try
            {
                var storage = Repository.LoadObjectFromStorage<Books>(STORAGE_FILE);
                foreach (var store in storage.Items)
                {
                    var ovm = new BookViewModel(store);
                    AddBook(ovm);
                }
            }
            catch
            {
                var storage = Repository.GetObjectsFromStorage<Book>(STORAGE_FILE);
                foreach (var store in storage)
                {
                    var ovm = new BookViewModel(store);
                    AddBook(ovm);
                }
            }        
        }

        public void LoadSettings()
        {
            var settings = MainViewModel.Default.AppSettings;
            foreach(var f in StateFilters)
            {
                var a = settings.BooksSettings.StateFilters.FirstOrDefault(p => p.Name == f.Value.Name);
                if (a == null) f.Value.IsChecked = true;
                else f.Value.IsChecked = a.IsChecked;
            }
            LastSelectedID = settings.BooksSettings.SelectedBookID;
        }

        public void SaveSettings()
        {
            var settings = MainViewModel.Default.AppSettings;
            settings.BooksSettings.StateFilters = StateFilters.Select(p => p.Value.ToModel()).ToList();
            settings.BooksSettings.SelectedBookID = SelectedItem?.ID ?? 0;
        }

        private void AddBook(BookViewModel book)
        {
            book.PropertyChanged += (o, ea) => NeedSave = true;
            Items.Add(book);            
        }
        private void DeleteBook(BookViewModel book)
        {
            Items.Remove(book);            
        }

        public override void Add()
        {            
            StateFilters[StateEnum.Created].IsChecked = true;
            TextFilter.ResetFilter();

            BookViewModel game = new BookViewModel()
            {
                ID = Items.Any() ? Items.Max(p => p.ID) + 1 : 1,
                Year = DateTime.Today.Year
            };

            AddBook(game);
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
                SelectedItem.RTitle = cb;
            }
        }

        public override void Delete()
        {
            if (SelectedItem == null) return;
            DeleteBook(SelectedItem);
            NeedSave = true;
        }

        public override void Save()
        {
            var storage = new Books();
            storage.Items = Items.Select(p => p.ToModel()).ToList();
            storage.SaveObjectToStorage(STORAGE_FILE);
            SetLastSaveTime();
            NeedSave = false;
        }

        public override void Backup()
        {
            string file = $"Books_{DateTime.Today:yyyyMMdd}.json.gz";
            file = System.IO.Path.Combine("Backups", file);            

            var storage = new Books();
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
            foreach (BookViewModel i in items)
            {
                i.States[StateEnum.Closed].IsDone = true;
            }            
        }

        public override void MultiArchive(IList items)
        {
            if (items == null) return;
            foreach (BookViewModel i in items)
            {
                i.States[StateEnum.Archived].IsDone = true;
            }
        }

        public override void MultiDelete(IList items)
        {
            if (items == null) return;
            var r = ConfirmationShow?.Invoke("Удалить?", "Выделенные книги будут удалены.");
            if (r == true)
            {
                List<BookViewModel> list = new List<BookViewModel>();
                foreach (BookViewModel i in items)
                {
                    list.Add(i as BookViewModel);
                }
                foreach (BookViewModel i in list)
                {
                    DeleteBook(i);
                }
                NeedSave = true;
            }
        }

        public void GetAuthors()
        {
            Authors = Items.Select(p => p.RAuthorName).Distinct().OrderBy(p => p).ToList();
        }
    }
}
