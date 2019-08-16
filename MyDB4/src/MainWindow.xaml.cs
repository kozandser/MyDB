using FirstFloor.ModernUI.Windows.Controls;
using MyDB4.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyDB4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        MainViewModel VM;
        public MainWindow()
        {
            InitializeComponent();
            VM = new MainViewModel();
            this.DataContext = VM;
        }

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (VM.NeedSave)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Сохранить изменения перед выходом?").Append("Требуется сохранить: ");
                if (VM.MoviesViewModel.NeedSave) sb.Append("[b]Фильмы[/b], ");
                if (VM.SerialsViewModel.NeedSave) sb.Append("[b]Сериалы[/b], ");
                if (VM.GamesViewModel.NeedSave) sb.Append("[b]Игры[/b], ");
                if (VM.BooksViewModel.NeedSave) sb.Append("[b]Книги[/b], ");
                sb.Length = sb.Length - 2;


                var dialog = new ModernDialog()
                {
                    Title = "Выход",
                    Content = new BBCodeBlock()
                    {
                        BBCode = sb.ToString()
                    }
                };

                MessageBoxResult result = MessageBoxResult.None;

                var saveAndExitButton = new Button()
                {
                    Content = "Сохранить и выйти",
                    Margin = new Thickness(2, 0, 2, 0),
                    FontWeight = FontWeights.Bold,
                    IsDefault = true
                };
                saveAndExitButton.Click += (o, ea) =>
                {
                    result = MessageBoxResult.Yes;
                    dialog.Close();
                };
                var dontSaveAndExitButton = new Button()
                {
                    Content = "Не сохранять и выйти",
                    Margin = new Thickness(2, 0, 2, 0)
                };
                dontSaveAndExitButton.Click += (o, ea) =>
                {
                    result = MessageBoxResult.No;
                    dialog.Close();
                };
                var cancelExitButton = new Button()
                {
                    Content = "Отмена",
                    Margin = new Thickness(2, 0, 2, 0),
                    IsCancel = true
                };
                cancelExitButton.Click += (o, ea) =>
                {
                    result = MessageBoxResult.Cancel;
                    dialog.Close();
                };

                dialog.Buttons = new Button[] { saveAndExitButton, dontSaveAndExitButton, cancelExitButton };


                dialog.ShowDialog();

                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (result == MessageBoxResult.Yes)
                {
                    VM.SaveUnsaved();
                }                
            }
            VM.SaveSettings();
        }
    }
}
