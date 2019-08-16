using KLib;
using MyDB4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyDB4.ViewModels
{
    public class MainViewModel
    {
        public static MainViewModel Default { get; private set; }

        public AppSettings AppSettings { get; set; }
        public MoviesViewModel MoviesViewModel { get; set; }
        public SerialsViewModel SerialsViewModel { get; set; }
        public GamesViewModel GamesViewModel { get; set; }
        public BooksViewModel BooksViewModel { get; set; }

        public ICommand SaveSettingsCommand { get; set; }
        public ICommand SaveAllCommand { get; set; }
        public ICommand BackupAllCommand { get; set; }

        public bool NeedSave => MoviesViewModel.NeedSave || SerialsViewModel.NeedSave || GamesViewModel.NeedSave || BooksViewModel.NeedSave;

        public MainViewModel()
        {
            Default = this;
            AppSettings = Repository.LoadAppSettings();
            MoviesViewModel = new MoviesViewModel();
            SerialsViewModel = new SerialsViewModel();
            GamesViewModel = new GamesViewModel();
            BooksViewModel = new BooksViewModel();

            SaveSettingsCommand = new RelayCommand(SaveSettings, "Сохранить настройки");
            SaveAllCommand = new RelayCommand(SaveAll, "Сохранить все");
            BackupAllCommand = new RelayCommand(BackupAll, "Сделать резервые копии для всех");
        }

        public void SaveSettings()
        {
            MoviesViewModel.SaveSettings();
            SerialsViewModel.SaveSettings();
            GamesViewModel.SaveSettings();
            BooksViewModel.SaveSettings();
            MainViewModel.Default.AppSettings.SaveAppSettings();
        }
        public void SaveAll()
        {
            MoviesViewModel.Save();
            SerialsViewModel.Save();
            GamesViewModel.Save();
            BooksViewModel.Save();
        }
        public void SaveUnsaved()
        {
            if (MoviesViewModel.NeedSave) MoviesViewModel.Save();
            if (SerialsViewModel.NeedSave) SerialsViewModel.Save();
            if (BooksViewModel.NeedSave) BooksViewModel.Save();
            if (GamesViewModel.NeedSave) GamesViewModel.Save();
        }
        public void BackupAll()
        {
            MoviesViewModel.Backup();
            SerialsViewModel.Backup();
            GamesViewModel.Backup();
            BooksViewModel.Backup();
        }       
    }
}
