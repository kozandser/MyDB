using FirstFloor.ModernUI.Windows.Controls;
using MyDB4.Models;
using MyDB4.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static MyDB4.ViewModels.MoviesStatisticsViewModel;

namespace MyDB4.Pages
{
    /// <summary>
    /// Interaction logic for SimpleMoviesList.xaml
    /// </summary>
    public partial class SimpleMoviesList : ModernDialog
    {
        public List<MovieViewModel> Items { get; set; }
        public ObservableCollection<CountClass> Counts { get; set; } = new ObservableCollection<CountClass>();
        public int Totals { get; set; }

        public SimpleMoviesList(List<MovieViewModel> lst)
        {
            InitializeComponent();
            Items = lst.OrderBy(p => p.States.Items[StateEnum.Finished].Date).ToList();

            foreach (var t in MovieViewModel.TypesDict)
            {
                Counts.Add(
                    new CountClass(Items.Count(p => p.Type == t.Key),t.Key, t.Value)
                    );
            }
            Totals = Counts.Sum(p => p.Count);


            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton };

            DataContext = this;
        }
    }
}
