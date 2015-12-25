using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicLibrary.Controllers
{
    public class HomeController : Controller
    {
        // Home page - Music Library browser page to enable searching for and playing media
        public ActionResult Index()
        {
            return View();
        }

   
    }
}