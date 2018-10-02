using MovieTrailerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MovieTrailerApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";  

            return View(new SearchModel());
        }


        
        [HttpPost]
        public async Task<ActionResult> Index(string SearchQuery)
        {
            SearchModel m = new SearchModel();
            m.SearchQuery = SearchQuery;           

            HttpClient client = new HttpClient();
            string path = "http://localhost:2089/api/search/" + SearchQuery;
            HttpResponseMessage ResponseMessage = await client.GetAsync(path);
            if (ResponseMessage.IsSuccessStatusCode)
            {
                m.Movies = await ResponseMessage.Content.ReadAsAsync<List<Movie>>();                
            }      
            return View(m);
        }



    }
}