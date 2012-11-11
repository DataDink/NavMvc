using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NavMvc;

namespace NavTests.Controllers
{
    [Authorize(Roles = "Everyone")]
    public class AdminController : Controller
    {
        [NavItem("MainNav", SubNavContext="AdminSubNav", Title = "Admin Pages", Description = "You will not see this without permissions", OrderingHint = 999)]
        [NavItem("AdminSubNav", Title = "General Admin", Description = "This is the landing page", OrderingHint = 0)]
        [Authorize(Roles = "Administrators")]
        public ActionResult Index()
        {
            this.SetNavValue("AdminSubNav", "AdminValue", 10);
            ViewBag.Message = "Admin page index";
            return View("CommonPage");
        }

        [NavItem("AdminSubNav", Title = "Admin Page 1", Description = "This is the second page", OrderingHint = 1)]
        public ActionResult Page1()
        {
            this.SetNavValue("AdminSubNav", "AdminValue", 10);
            ViewBag.Message = "Admin page 1";
            return View("CommonPage");
        }

        [NavItem("AdminSubNav", Title = "Admin Page 2", Description = "This is the third page", OrderingHint = 2)]
        public ActionResult Page2()
        {
            this.SetNavValue("AdminSubNav", "AdminValue", 10);
            ViewBag.Message = "Admin page 2";
            return View("CommonPage");
        }

    }
}
