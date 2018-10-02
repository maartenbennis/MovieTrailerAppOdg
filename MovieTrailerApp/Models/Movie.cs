using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTrailerApp.Models
{
    public class Movie
    {
        public string imdbID { get; set; }
        public string Title { get; set; }
        public string Poster { get; set; }
        public int Year { get; set; }
        public string Type { get; set; }
        public string Plot { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public string Writer { get; set; }
        public List<Trailer> TrailerList { get; set; }
    }

    public class SearchModel
    {
        public string SearchQuery { get; set; }
        public List<Movie> Movies { get; set; }

        public SearchModel()
        {
            Movies = new List<Movie>();
        }
    }

    public class Trailer
    {
        public string TrailerUrl { get; set; }
        public string TrailerName { get; set; }
        public string Thumbnail { get; set; }
    }

    public class Rootobject
    {
        public Movie[] Search { get; set; }
        public string totalResults { get; set; }
        public string Response { get; set; }
    }


}