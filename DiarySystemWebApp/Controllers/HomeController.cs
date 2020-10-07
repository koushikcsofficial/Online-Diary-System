using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiarySystemWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //string email = Session["LoginEmail"].ToString();
            if (Session["LoginEmail"] == null)
            {
                return RedirectToAction("Logout", "Authentication");
            }
            return RedirectToAction("Index","Diary");
        }
    }
}