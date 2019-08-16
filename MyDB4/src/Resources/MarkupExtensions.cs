using MyDB4.Models;
using MyDB4.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace MyDB4.Resources
{
    public static class XamlResources
    {
        public static SolidColorBrush CreatedState_ColorBrush { get; private set; }  = Application.Current.Resources["CreatedState_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush InQueueState_ColorBrush { get; private set; }  = Application.Current.Resources["InQueueState_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush StartedState_ColorBrush { get; private set; }  = Application.Current.Resources["StartedState_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush PausedState_ColorBrush { get; private set; }   = Application.Current.Resources["PausedState_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush FinishedState_ColorBrush { get; private set; } = Application.Current.Resources["FinishedState_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush ReviewedState_ColorBrush { get; private set; } = Application.Current.Resources["ReviewedState_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush ClosedState_ColorBrush { get; private set; }   = Application.Current.Resources["ClosedState_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush ArchivedState_ColorBrush { get; private set; } = Application.Current.Resources["ArchivedState_ColorBrush"] as SolidColorBrush;

        public static SolidColorBrush TypeNA_ColorBrush { get; private set; }        = Application.Current.Resources["TypeNA_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush TypeForMe_ColorBrush { get; private set; }     = Application.Current.Resources["TypeForMe_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush TypeForParent_ColorBrush { get; private set; } = Application.Current.Resources["TypeForParent_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush TypeForFamily_ColorBrush { get; private set; } = Application.Current.Resources["TypeForFamily_ColorBrush"] as SolidColorBrush;

        public static SolidColorBrush Rating0_ColorBrush { get; private set; } = Application.Current.Resources["Rating0_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush Rating1_ColorBrush { get; private set; } = Application.Current.Resources["Rating1_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush Rating2_ColorBrush { get; private set; } = Application.Current.Resources["Rating2_ColorBrush"] as SolidColorBrush;
        public static SolidColorBrush Rating3_ColorBrush { get; private set; } = Application.Current.Resources["Rating3_ColorBrush"] as SolidColorBrush;

        public static SolidColorBrush Black_ColorBrush { get; private set; }         = new SolidColorBrush(Colors.Black);
        public static SolidColorBrush PeterRiverBrush_ColorBrush { get; private set; } = Application.Current.Resources["PeterRiverBrush"] as SolidColorBrush;

        public static Canvas Plus_box_icon { get; private set; } = Application.Current.Resources["plus_box_icon"] as Canvas;
        public static Canvas Download_icon { get; private set; } = Application.Current.Resources["download_icon"] as Canvas;
        public static Canvas Filmstrip_icon { get; private set; } = Application.Current.Resources["filmstrip_icon"] as Canvas;
        public static Canvas Message_text_icon { get; private set; } = Application.Current.Resources["message_text_icon"] as Canvas;
        public static Canvas Lock_icon { get; private set; } = Application.Current.Resources["lock_icon"] as Canvas;
        public static Canvas Archive_icon { get; private set; } = Application.Current.Resources["archive_icon"] as Canvas;
        public static Canvas Play_icon_icon { get; private set; } = Application.Current.Resources["play_icon"] as Canvas;
        public static Canvas Finish_icon { get; private set; } = Application.Current.Resources["finish_icon"] as Canvas;
        public static Canvas Pause_icon { get; private set; } = Application.Current.Resources["pause_icon"] as Canvas;
    }

    public static class ConvertFuncs
    {
        public static SolidColorBrush TypeToBrushFunc(this TypeEnum val)
        {
            switch (val)
            {
                case TypeEnum.NA:
                    return XamlResources.TypeNA_ColorBrush;
                case TypeEnum.ForMe:
                    return XamlResources.TypeForMe_ColorBrush;
                case TypeEnum.ForParent:
                    return XamlResources.TypeForParent_ColorBrush;
                case TypeEnum.ForFamily:
                    return XamlResources.TypeForFamily_ColorBrush;
            }
            return XamlResources.Black_ColorBrush;
        }

        public static SolidColorBrush StateToBrushFunc(this StateEnum val)
        {
            switch (val)
            {
                case StateEnum.Created:
                    return XamlResources.CreatedState_ColorBrush;
                case StateEnum.InQueue:
                    return XamlResources.InQueueState_ColorBrush;
                case StateEnum.Started:
                    return XamlResources.StartedState_ColorBrush;
                case StateEnum.Paused:
                    return XamlResources.PausedState_ColorBrush;
                case StateEnum.Finished:
                    return XamlResources.FinishedState_ColorBrush;
                case StateEnum.Reviewed:
                    return XamlResources.ReviewedState_ColorBrush;
                case StateEnum.Closed:
                    return XamlResources.ClosedState_ColorBrush;
                case StateEnum.Archived:
                    return XamlResources.ArchivedState_ColorBrush;
            }
            return XamlResources.Black_ColorBrush;
        }

        public static SolidColorBrush RatingClassToBrushFunc(int val)
        {
            switch (val)
            {
                case 0:
                    return XamlResources.Rating0_ColorBrush;
                case 1:
                    return XamlResources.Rating1_ColorBrush;
                case 2:
                    return XamlResources.Rating2_ColorBrush;
                case 3:
                    return XamlResources.Rating3_ColorBrush;
                default:
                    break;
            }
            return XamlResources.Black_ColorBrush;
        }

        public static SolidColorBrush RatingToBrushFunc(double val)
        {
            int val1 = RatingViewModel.GetRatingClass(val);
            switch (val1)
            {
                case 0:
                    return XamlResources.Rating0_ColorBrush;
                case 1:
                    return XamlResources.Rating1_ColorBrush;
                case 2:
                    return XamlResources.Rating2_ColorBrush;
                case 3:
                    return XamlResources.Rating3_ColorBrush;
                default:
                    break;
            }
            return XamlResources.Black_ColorBrush;
        }
    }

    internal class StateToBrushConverter : MarkupExtension, IValueConverter
    {
        private static StateToBrushConverter _converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new StateToBrushConverter());
        }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value is StateEnum)
            {
                StateEnum val = (StateEnum)value;
                return val.StateToBrushFunc();
            }
            return XamlResources.Black_ColorBrush;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class TypeToBrushConverter : MarkupExtension, IValueConverter
    {
        private static TypeToBrushConverter _converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new TypeToBrushConverter());
        }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value is TypeEnum)
            {
                TypeEnum val = (TypeEnum)value;
                return val.TypeToBrushFunc();                
            }
            return XamlResources.Black_ColorBrush;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class RatingToBrushConverter : MarkupExtension, IValueConverter
    {
        private static RatingToBrushConverter _converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new RatingToBrushConverter());
        }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int val = (int)value;
                switch (val)
                {
                    case 0:
                        return XamlResources.Rating0_ColorBrush;
                    case 1:
                        return XamlResources.Rating1_ColorBrush;
                    case 2:
                        return XamlResources.Rating2_ColorBrush;
                    case 3:
                        return XamlResources.Rating3_ColorBrush;
                    default:
                        break;
                }
            }
            return XamlResources.Black_ColorBrush;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class MovieStateToIconConverter : MarkupExtension, IValueConverter
    {
        private static MovieStateToIconConverter _converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new MovieStateToIconConverter());
        }

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value is StateEnum)
            {
                StateEnum val = (StateEnum)value;
                switch (val)
                {
                    case StateEnum.Created:
                        return XamlResources.Plus_box_icon;
                    case StateEnum.InQueue:
                        return XamlResources.Download_icon;
                    case StateEnum.Started:
                        return XamlResources.Download_icon;
                    case StateEnum.Finished:
                        return XamlResources.Filmstrip_icon;
                    case StateEnum.Reviewed:
                        return XamlResources.Message_text_icon;
                    case StateEnum.Closed:
                        return XamlResources.Lock_icon;
                    case StateEnum.Archived:
                        return XamlResources.Archive_icon;
                    default:
                        break;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class SeasonStateToIconConverter : MarkupExtension, IValueConverter
    {
        private static SeasonStateToIconConverter _converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new SeasonStateToIconConverter());
        }
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value is StateEnum)
            {
                StateEnum val = (StateEnum)value;
                switch (val)
                {
                    case StateEnum.Created:
                        return XamlResources.Plus_box_icon;
                    case StateEnum.InQueue:
                        return XamlResources.Download_icon;
                    case StateEnum.Started:
                        return XamlResources.Play_icon_icon;
                    case StateEnum.Paused:
                        return XamlResources.Pause_icon;
                    case StateEnum.Finished:
                        return XamlResources.Finish_icon;
                    default:
                        return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class GameStateToIconConverter : MarkupExtension, IValueConverter
    {
        private static GameStateToIconConverter _converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new GameStateToIconConverter());
        }
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value is StateEnum)
            {
                StateEnum val = (StateEnum)value;
                switch (val)
                {
                    case StateEnum.Created:
                        return XamlResources.Plus_box_icon;
                    case StateEnum.InQueue:
                        return XamlResources.Download_icon;
                    case StateEnum.Started:
                        return XamlResources.Play_icon_icon;
                    case StateEnum.Paused:
                        return XamlResources.Pause_icon;
                    case StateEnum.Finished:
                        return XamlResources.Finish_icon;
                    case StateEnum.Archived:
                        return XamlResources.Archive_icon;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class BookStateToIconConverter : MarkupExtension, IValueConverter
    {
        private static BookStateToIconConverter _converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new BookStateToIconConverter());
        }
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value is StateEnum)
            {
                StateEnum val = (StateEnum)value;
                switch (val)
                {
                    case StateEnum.Created:
                        return XamlResources.Plus_box_icon;
                    case StateEnum.Started:
                        return XamlResources.Play_icon_icon;
                    case StateEnum.Paused:
                        return XamlResources.Pause_icon;
                    case StateEnum.Finished:
                        return XamlResources.Finish_icon;
                    case StateEnum.Archived:
                        return XamlResources.Archive_icon;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
