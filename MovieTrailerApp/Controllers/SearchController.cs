using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MovieTrailerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MovieTrailerApp.Controllers
{
    public class SearchController : ApiController
    {
        static HttpClient client = new HttpClient();
        string OmdbApiKey = System.Configuration.ConfigurationManager.AppSettings["OmdbApiKey"].ToString();
        string YoutubeApiKey = System.Configuration.ConfigurationManager.AppSettings["YoutubeApiKey"].ToString();    

       //Get movie by ImdbID and use Movie title to search for trailers
        [Route("api/search/{ImdbId}/{MovieTitle}")]
        public async Task<Movie> GetMoviesById(string ImdbId, string MovieTitle)
        {
            Movie mMovie = HttpContext.Current.Cache.Get("Trailer_" + ImdbId) as Movie;
            if(mMovie == null)
            {             
                string path = "http://www.omdbapi.com/?r=json&type=movie&i=" + ImdbId + "&apikey=" + OmdbApiKey;
                HttpResponseMessage ResponseMessage = await client.GetAsync(path);
                if (ResponseMessage.IsSuccessStatusCode)
                {
                    mMovie = await ResponseMessage.Content.ReadAsAsync<Movie>();                
                }
                //Add default image if result has no poster
                if (mMovie.Poster == "N/A")
                    mMovie.Poster = "/Content/NoPoster.jpg";

                List<Trailer> lstTrailer = new List<Trailer>();

                YouTubeService ytService = new YouTubeService(new BaseClientService.Initializer() { ApiKey= YoutubeApiKey, ApplicationName= this.GetType().ToString() });

                var searchListRequest = ytService.Search.List("snippet");
                searchListRequest.Q = mMovie.Title + " Official+trailer"; // Replace with your search term.
                searchListRequest.MaxResults = 20;

                // Call the search.list method to retrieve results matching the specified query term.
                var searchListResponse = await searchListRequest.ExecuteAsync();
                foreach (var searchResult in searchListResponse.Items)
                {
                    //for better results only add youtube trailers with the name of the movie in the title and the word trailer
                    if (searchResult.Snippet.Title.ToLower().Contains(mMovie.Title.ToLower()) && searchResult.Snippet.Title.ToLower().Contains("trailer"))
                    {
                        Trailer t = new Trailer();                    
                        t.TrailerName = searchResult.Snippet.Title;
                        if (t.TrailerName.Length > 65)// to long text result don't look nice in the design
                            t.TrailerName = searchResult.Snippet.Title.Substring(0, 65);
                        t.TrailerUrl = "//www.youtube.com/embed/" + searchResult.Id.VideoId;
                        if (searchResult.Snippet.Thumbnails.Maxres != null)
                            t.Thumbnail = searchResult.Snippet.Thumbnails.Maxres.Url;
                        else if (searchResult.Snippet.Thumbnails.High != null)
                            t.Thumbnail = searchResult.Snippet.Thumbnails.High.Url;
                        else if (searchResult.Snippet.Thumbnails.Medium != null)
                            t.Thumbnail = searchResult.Snippet.Thumbnails.Medium.Url;
                        else
                            t.Thumbnail = searchResult.Snippet.Thumbnails.Default__.Url;

                        lstTrailer.Add(t);
                    }
                }
                mMovie.TrailerList = lstTrailer.Take(8).ToList();// Only show the first 8 matching results
                HttpContext.Current.Cache.Insert("Trailer_" + ImdbId, mMovie, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));//add to cache sliding 1 hour
            }
            return mMovie;
        }      

        //Get al movies matching search result
        [Route("api/search/{SearchQuery}")]
        public async Task<List<Movie>> GetMoviesSearch(string SearchQuery)
        {            
            List<Movie> LstMovies = HttpContext.Current.Cache.Get("SearchQuery_"+SearchQuery) as List<Movie>; ;

            if (LstMovies == null)
            {
                string path = "http://www.omdbapi.com/?r=json&type=movie&s=" + SearchQuery + "&apikey=" + OmdbApiKey;
                HttpResponseMessage ResponseMessage = await client.GetAsync(path);
                if (ResponseMessage.IsSuccessStatusCode)
                {
                    var res = await ResponseMessage.Content.ReadAsAsync<Rootobject>();
                    LstMovies = res.Search.ToList();
                }
                //Add default image to results without a poster
                foreach (Movie m in LstMovies.Where(x => x.Poster == "N/A"))
                {
                    m.Poster = "/Content/NoPoster.jpg";
                }
                HttpContext.Current.Cache.Insert("SearchQuery_" + SearchQuery, LstMovies, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));//add to cache sliding 1 hour
            }
            return LstMovies;
        }

    }

}
