using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NavMvc;
using NavMvc.NavItems;

namespace NavTests.Controllers
{
    public class ContentController : Controller
    {
        [NavItem("MainNav", SubNavContext = "ContentSubNav", Title="Site Content", Description = "Enter here for something interesting!")]
        [NavItem("ContentSubNav", Title = "Content First Page", Description = "This is the same link as the parent menu", OrderingHint = 0)]
        public ActionResult Content1()
        {
            ViewBag.Message = "Content Home";
            return View("ContentPage");
        }

        [NavItem("ContentSubNav", Title = "Content Second Page", Description = "Another content page", OrderingHint = 1)]
        public ActionResult Content2()
        {
            this.AddNavItem("MainNav", new UrlNavItem {
                Title = "Poof!",
                Url = "http://www.markonthenet.com",
                OrderingHint = 9999
            });
            ViewBag.Message = "Content Page 2";
            return View("ContentPage");
        }

        [NavItem("ContentSubNav", Title = "Content Third Page", Description = "Another content page", OrderingHint = 2)]
        public ActionResult Content3()
        {
            ViewBag.Message = "Content Page 3";
            return View("ContentPage");
        }

        [NavItem("LeftNav", Title = "Left Nav Content 1", Description = "Left Item 1", OrderingHint = 2)]
        public ActionResult Left1()
        {
            ViewBag.Message = "Left Nav Page 1";
            return View("ContentPage");
        }

        [NavItem("LeftNav", Title = "Left Nav Content 2", Description = "Left Item 2", OrderingHint = 2)]
        public ActionResult Left2()
        {
            ViewBag.Message = "Left Nav Page 2";
            return View("ContentPage");
        }

        [NavItem("LeftNav", Title = "Left Nav Content 3", Description = "Left Item 3", OrderingHint = 2)]
        public ActionResult Left3()
        {
            ViewBag.Message = "Left Nav Page 3";
            return View("ContentPage");
        }
    }
}
