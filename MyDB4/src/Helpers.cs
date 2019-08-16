using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyDB4
{
    public static class Helpers
    {
        //public static void SyncSortWithView(this DataGrid dg)
        //{
        //    SortDescriptionCollection tmp = dg.Items.SortDescriptions;

        //    foreach (SortDescription sd in tmp)
        //    {
        //        var col = dg.Columns.FirstOrDefault(x => x.SortMemberPath == sd.PropertyName);
        //        if (col != null)
        //        {
        //            col.SortDirection = sd.Direction;
        //        }
        //    }
        //}

        //public static void ScrollToCurrentItem(this DataGrid dg)
        //{
        //    var view = (CollectionView)CollectionViewSource.GetDefaultView(dg.ItemsSource);
        //    if (view == null) return;
        //    if (view.CurrentItem == null) return;
        //    dg.ScrollIntoView(view.CurrentItem);
        //}
    }
}
