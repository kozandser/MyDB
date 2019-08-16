using FirstFloor.ModernUI.Windows;
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
using FirstFloor.ModernUI.Windows.Navigation;
using FirstFloor.ModernUI.Windows.Controls;
using KLib;

namespace MyDB4.Pages
{
    /// <summary>
    /// Interaction logic for MoviesPage.xaml
    /// </summary>
    public partial class MoviesPage : UserControl, IContent
    {
        private MoviesViewModel VM;
        public MoviesPage()
        {
            InitializeComponent();
            NameScope.SetNameScope(dgcm, NameScope.GetNameScope(this));

            VM = MainViewModel.Default.MoviesViewModel;
            
            VM.ConfirmationShow = (s1, s2) =>
            {
                var dialog = new ModernDialog
                {
                    Title = s1,
                    Content = s2
                };

                MessageBoxResult result = MessageBoxResult.Cancel;
                var yesButton = new Button()
                {
                    Content = "Да",
                    Margin = new Thickness(2, 0, 2, 0)
                };
                yesButton.Click += (o, ea) =>
                {
                    result = MessageBoxResult.Yes;
                    dialog.Close();
                };
                var noButton = new Button()
                {
                    Content = "Нет",
                    Margin = new Thickness(2, 0, 2, 0),
                    FontWeight = FontWeights.Bold,
                    IsDefault = true
                };
                noButton.Click += (o, ea) =>
                {
                    result = MessageBoxResult.No;
                    dialog.Close();
                };
                dialog.Buttons = new Button[] { yesButton, noButton };


                dialog.ShowDialog();

                if (result == MessageBoxResult.Yes) return true;
                else return false;
            };
            VM.TextSearchStarted = () =>
            {
                tbFilter.Focus();
                Keyboard.Focus(tbFilter);
            };
            dg.Focus();
            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.ContextIdle,
                new Action(() => Keyboard.Focus(dg)));

            VM.Refreshed = () => dg.ScrollToCurrentItem();

            DataContext = VM;
        }

        

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }
        
    }
}
