using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyDB4.Models
{
    public class StateItem
    {
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public StateEnum Name { get; set; }
        [JsonConverter(typeof(MyDateTimeConvertor))]
        public DateTime? Date { get; set; }
    }
    #region MOVIES
    public class Movies
    {
        public List<Movie> Items { get; set; }
        public Movies()
        {
            Items = new List<Movie>();
        }
    }

    public class Movie
    {
        public int ID { get; set; }
        public string RTitle { get; set; }
        public string OTitle { get; set; }
        public int Year { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public double Rating { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public TypeEnum Type { get; set; }
        public List<StateItem> States { get; set; }

        public int? KinopoiskId { get; set; }
        public double? KinopoiskRating { get; set; }
        public double? IMDBRating { get; set; }
    }    
    #endregion
    #region SERIALS
    public class Serials
    {
        public List<Serial> Items { get; set; }
        public Serials()
        {
            Items = new List<Serial>();
        }
    }

    public class Serial
    {
        public int ID { get; set; }
        public string RTitle { get; set; }
        public string OTitle { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(MyDateTimeConvertor))]
        public DateTime? CreateDate { get; set; }
        public bool HasLastSeason { get; set; }
        public int? KinopoiskId { get; set; }
        public double? KinopoiskRating { get; set; }
        public double? IMDBRating { get; set; }

        public List<Season> Seasons { get; set; }

        public Serial()
        {
            Seasons = new List<Season>();
        }
    }

    public class Season
    {
        public int Number { get; set; }
        public int Episodes { get; set; }
        public string Note { get; set; }

        public List<StateItem> States { get; set; }
    }
    #endregion
    #region GAMES
    public class Games
    {
        public List<Game> Items { get; set; }
        public Games()
        {
            Items = new List<Game>();
        }
    }
    public class Game
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public TimeSpan? PlayTime { get; set; }
        public double Rating { get; set; }
        public List<StateItem> States { get; set; }
    }
    #endregion
    #region BOOKS
    public class Books
    {
        public List<Book> Items { get; set; }
        public Books()
        {
            Items = new List<Book>();
        }
    }
    public class Book
    {
        public int ID { get; set; }
        public string RTitle { get; set; }
        public string OTitle { get; set; }
        public string RAuthorName { get; set; }
        public string OAuthorName { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public double Rating { get; set; }
        public List<StateItem> States { get; set; }
    }
    #endregion
}
