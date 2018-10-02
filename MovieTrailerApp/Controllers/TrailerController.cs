using MovieTrailerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace MovieTrailerApp.Controllers
{
    public class TrailerController : Controller
    {
        // GET: Trailer
        //Output caching by imdbID 
        [OutputCache(Duration = 60, VaryByParam = "ImdbId", Location = System.Web.UI.OutputCacheLocation.Server)]
        public async Task<ActionResult> Index(string ImdbId, string movie)
        {
            HttpClient client = new HttpClient();
            Movie m = new Movie();
            string path = "http://localhost:2089/api/search/" + ImdbId + "/" + Url.Encode(movie.Replace(":", " "));
            HttpResponseMessage ResponseMessage = await client.GetAsync(path);
            if (ResponseMessage.IsSuccessStatusCode)
            {
                m = await ResponseMessage.Content.ReadAsAsync<Movie>();
            }

            return View(m);
        }
    }
}