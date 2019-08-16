using MyDB4.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections;
using System.Windows.Data;
using System.IO;
using KLib;
using KLib.Classes;
using System.Collections.ObjectModel;

namespace MyDB4.ViewModels
{
    [ImplementPropertyChanged]
    public class BaseListViewModel
    {
        #region PROPERTIES=================================
        public string STORAGE_FILE = "";
        public Dictionary<StateEnum, StateFilterViewModel> StateFilters { get; set; }
        public Dictionary<TypeEnum, TypeFilterViewModel> TypeFilters { get; set; }
        public DateTime LastSaveTime { get; set; }
        public TextFilter TextFilter { get; set; }
        public int LastSelectedID;
        public ICollectionView View { get; set; }
        public bool NeedSave { get; set; }
        #endregion=========================================
        #region COMMANDS===================================
        public ICommand AddCommand { get; set; }
        public ICommand AddNewFromClipboardCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ApplyFilterCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand BackupCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand StateFiltersCheckOnCommand { get; set; }
        public ICommand StateFiltersCheckOffCommand { get; set; }
        public ICommand TypeFiltersCheckOnCommand { get; set; }
        public ICommand TypeFiltersCheckOffCommand { get; set; }
        public ICommand MultiCloseCommand { get; set; }
        public ICommand MultiArchiveCommand { get; set; }
        public ICommand MultiDeleteCommand { get; set; }
        public ICommand StartTextSearchCommand { get; set; }
        #endregion=========================================
        #region ACTIONS====================================
        public Func<string, string, bool> ConfirmationShow { get; set; }
        public Action Loaded { get; set; }
        public Action Refreshed { get; set; }
        public Action TextSearchStarted { get; set; }
        #endregion=========================================

        public BaseListViewModel()
        {
            AddCommand = new RelayCommand(Add, "Добавить");
            AddNewFromClipboardCommand = new RelayCommand(AddNewFromClipboard, "Добавить из буфера обмена");
            DeleteCommand = new RelayCommand(Delete, "Удалить");
            ApplyFilterCommand = new RelayCommand(ApplyFilter, "Обновить");
            LoadCommand = new RelayCommand(ApplyFilter, "Загрузить");
            SaveCommand = new RelayCommand(Save, "Сохранить");
            BackupCommand = new RelayCommand(Backup, "Резервная копия");
            StateFiltersCheckOnCommand = new RelayCommand(
                () =>
                {
                    foreach (var f in StateFilters)
                    {
                        f.Value.IsCheckedSuppressed = true;
                        f.Value.IsChecked = true;
                        f.Value.IsCheckedSuppressed = false;
                    }
                    ApplyFilter();
                },
                "Выбать все"
                );
            StateFiltersCheckOffCommand = new RelayCommand(
                () =>
                {
                    foreach (var f in StateFilters)
                    {
                        f.Value.IsCheckedSuppressed = true;
                        f.Value.IsChecked = false;
                        f.Value.IsCheckedSuppressed = false;
                    }
                    ApplyFilter();
                },
                "Снять выбор"
                );
            TypeFiltersCheckOnCommand = new RelayCommand(
                () =>
                {
                    foreach (var f in TypeFilters)
                    {
                        f.Value.IsCheckedSuppressed = true;
                        f.Value.IsChecked = true;
                        f.Value.IsCheckedSuppressed = false;
                    }
                    ApplyFilter();
                },
                "Выбать все"
                );
            TypeFiltersCheckOffCommand = new RelayCommand(
                () =>
                {
                    foreach (var f in TypeFilters)
                    {
                        f.Value.IsCheckedSuppressed = true;
                        f.Value.IsChecked = false;
                        f.Value.IsCheckedSuppressed = false;
                    }
                    ApplyFilter();
                },
                "Снять выбор"
                );
            MultiCloseCommand = new RelayCommand<IList>(MultiClose, "Закрыть выделенные");
            MultiArchiveCommand = new RelayCommand<IList>(MultiArchive, "В архив выделенные");
            MultiDeleteCommand = new RelayCommand<IList>(MultiDelete, "Удалить выделенные");
            StartTextSearchCommand = new RelayCommand(() => TextSearchStarted?.Invoke(), "Начать поиск");

            TextFilter = new TextFilter();
            TextFilter.TextChanged += (o, ea) => ApplyFilter();

            
        }

        public virtual void MultiDelete(IList obj)
        {
            throw new NotImplementedException();
        }

        public virtual void MultiArchive(IList obj)
        {
            throw new NotImplementedException();
        }

        public virtual void MultiClose(IList obj)
        {
            throw new NotImplementedException();
        }

        public void SetLastSaveTime()
        {
            FileInfo file1 = new FileInfo(STORAGE_FILE);
            LastSaveTime = file1.LastWriteTime;
            //LastSaveTime = file1?.LastWriteTime ?? new DateTime();
        }

        public virtual void Add()
        {
            throw new NotImplementedException();
        }

        public virtual void AddNewFromClipboard()
        {
            throw new NotImplementedException();
        }

        public virtual void Delete()
        {
            throw new NotImplementedException();
        }

        public virtual void ApplyFilter()
        {
            throw new NotImplementedException();
        }

        public virtual void Save()
        {
            throw new NotImplementedException();
        }

        public virtual void Backup()
        {
            throw new NotImplementedException();
        }
    }

    [ImplementPropertyChanged]
    public abstract class BaseListViewModel<T> where T : BaseViewModel
    {
        #region PROPERTIES=================================
        public string STORAGE_FILE = "";
        public Dictionary<StateEnum, StateFilterViewModel> StateFilters { get; set; }
        public Dictionary<TypeEnum, TypeFilterViewModel> TypeFilters { get; set; }
        public DateTime LastSaveTime { get; set; }
        public TextFilter TextFilter { get; set; }
        public int LastSelectedID;
        public ObservableCollection<T> Items { get; set; }
        public T SelectedItem { get; set; }
        public ICollectionView View { get; set; }
        public bool NeedSave { get; set; }
        #endregion=========================================
        #region COMMANDS===================================
        public ICommand AddCommand { get; set; }
        public ICommand AddNewFromClipboardCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ApplyFilterCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand BackupCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand StateFiltersCheckOnCommand { get; set; }
        public ICommand StateFiltersCheckOffCommand { get; set; }
        public ICommand TypeFiltersCheckOnCommand { get; set; }
        public ICommand TypeFiltersCheckOffCommand { get; set; }
        public ICommand MultiCloseCommand { get; set; }
        public ICommand MultiArchiveCommand { get; set; }
        public ICommand MultiDeleteCommand { get; set; }
        public ICommand StartTextSearchCommand { get; set; }
        #endregion=========================================
        #region ACTIONS====================================
        public Func<string, string, bool> ConfirmationShow { get; set; }
        public Action Loaded { get; set; }
        public Action Refreshed { get; set; }
        public Action TextSearchStarted { get; set; }
        #endregion=========================================

        public BaseListViewModel(string storagefile)
        {
            STORAGE_FILE = storagefile;
            Items = new ObservableCollection<T>();
            View = CollectionViewSource.GetDefaultView(Items);
            Items.ShapeView().Where(Filter).Apply();

            SetLastSaveTime();

            AddCommand = new RelayCommand(Add, "Добавить");
            AddNewFromClipboardCommand = new RelayCommand(AddNewFromClipboard, "Добавить из буфера обмена");
            DeleteCommand = new RelayCommand(Delete, "Удалить");
            ApplyFilterCommand = new RelayCommand(ApplyFilter, "Обновить");
            LoadCommand = new RelayCommand(ApplyFilter, "Загрузить");
            SaveCommand = new RelayCommand(Save, "Сохранить");
            BackupCommand = new RelayCommand(Backup, "Резервная копия");
            StateFiltersCheckOnCommand = new RelayCommand(
                () =>
                {
                    foreach (var f in StateFilters)
                    {
                        f.Value.IsCheckedSuppressed = true;
                        f.Value.IsChecked = true;
                        f.Value.IsCheckedSuppressed = false;
                    }
                    ApplyFilter();
                },
                "Выбать все"
                );
            StateFiltersCheckOffCommand = new RelayCommand(
                () =>
                {
                    foreach (var f in StateFilters)
                    {
                        f.Value.IsCheckedSuppressed = true;
                        f.Value.IsChecked = false;
                        f.Value.IsCheckedSuppressed = false;
                    }
                    ApplyFilter();
                },
                "Снять выбор"
                );
            TypeFiltersCheckOnCommand = new RelayCommand(
                () =>
                {
                    foreach (var f in TypeFilters)
                    {
                        f.Value.IsCheckedSuppressed = true;
                        f.Value.IsChecked = true;
                        f.Value.IsCheckedSuppressed = false;
                    }
                    ApplyFilter();
                },
                "Выбать все"
                );
            TypeFiltersCheckOffCommand = new RelayCommand(
                () =>
                {
                    foreach (var f in TypeFilters)
                    {
                        f.Value.IsCheckedSuppressed = true;
                        f.Value.IsChecked = false;
                        f.Value.IsCheckedSuppressed = false;
                    }
                    ApplyFilter();
                },
                "Снять выбор"
                );
            MultiCloseCommand = new RelayCommand<IList>(MultiClose, "Закрыть выделенные");
            MultiArchiveCommand = new RelayCommand<IList>(MultiArchive, "В архив выделенные");
            MultiDeleteCommand = new RelayCommand<IList>(MultiDelete, "Удалить выделенные");
            StartTextSearchCommand = new RelayCommand(() => TextSearchStarted?.Invoke(), "Начать поиск");

            TextFilter = new TextFilter();
            TextFilter.TextChanged += (o, ea) => ApplyFilter();
        }

        protected abstract bool Filter(object item);
        protected virtual void ResetNeedSave()
        {
            foreach (var i in Items)
            {
                i.IsChanged = false;
            }
            NeedSave = false;
        }
        

        public virtual void MultiDelete(IList obj)
        {
            throw new NotImplementedException();
        }

        public virtual void MultiArchive(IList obj)
        {
            throw new NotImplementedException();
        }

        public virtual void MultiClose(IList obj)
        {
            throw new NotImplementedException();
        }

        public void SetLastSaveTime()
        {
            FileInfo file1 = new FileInfo(STORAGE_FILE);
            LastSaveTime = file1.LastWriteTime;
            //LastSaveTime = file1?.LastWriteTime ?? new DateTime();
        }

        public virtual void Add()
        {
            throw new NotImplementedException();
        }

        public virtual void AddNewFromClipboard()
        {
            throw new NotImplementedException();
        }

        public virtual void Delete()
        {
            throw new NotImplementedException();
        }

        public virtual void ApplyFilter()
        {
            if (View == null) return;
            View.DeferRefresh();
            if (SelectedItem == null) View.MoveCurrentToFirst();
            Refreshed?.Invoke();
        }

        public virtual void Save()
        {
            throw new NotImplementedException();
        }

        public virtual void Backup()
        {
            throw new NotImplementedException();
        }
    }



}
