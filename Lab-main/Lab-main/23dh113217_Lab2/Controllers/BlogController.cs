using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _23dh113217_Lab2.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blogs
        public ActionResult Blog()
        {
            return View();
        }
    }
}