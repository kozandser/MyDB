using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Newtonsoft.Json;

namespace MyDB4.Models
{
    public class StateFilter
    {
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public StateEnum Name { get; set; }
        public bool IsChecked { get; set; }
    }

    public class TypeFilter
    {
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public TypeEnum Name { get; set; }
        public bool IsChecked { get; set; }
    }

    public class MoviesSettings
    {
        public List<StateFilter> StateFilters { get; set; }
        public List<TypeFilter> TypeFilters { get; set; }
        public int SelectedMovieID { get; set; }
        public MoviesSettings()
        {
            StateFilters = new List<StateFilter>();
            TypeFilters = new List<TypeFilter>();
        }
    }
    public class SerialsSettings
    {
        public int SelectedSerialID { get; set; }        
    }
    public class GamesSettings
    {
        public List<StateFilter> StateFilters { get; set; }
        public int SelectedGameID { get; set; }
        public GamesSettings()
        {
            StateFilters = new List<StateFilter>();
        }
    }
    public class BooksSettings
    {
        public List<StateFilter> StateFilters { get; set; }
        public int SelectedBookID { get; set; }
        public BooksSettings()
        {
            StateFilters = new List<StateFilter>();
        }
    }

    public class AppSettings
    {
        public MoviesSettings MoviesSettings { get; set; }
        public SerialsSettings SerialsSettings { get; set; }
        public GamesSettings GamesSettings { get; set; }
        public BooksSettings BooksSettings { get; set; }
        
        public AppSettings()
        {
            MoviesSettings = new MoviesSettings();
            SerialsSettings = new SerialsSettings();
            GamesSettings = new GamesSettings();
            BooksSettings = new BooksSettings();
        }
    }
}
