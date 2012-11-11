using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NavMvc;
using NavMvc.NavItems;

namespace NavTests.Controllers
{
    public class HomeController : Controller
    {
        [NavItem("MainNav", Title = "Home", Description = "This little piggy went wee wee wee all the way here", OrderingHint = -1)]
        public ActionResult Index()
        {
            ViewBag.Message = "Home Page - Welcome!";
            return View("CommonPage");
        }
    }
}
