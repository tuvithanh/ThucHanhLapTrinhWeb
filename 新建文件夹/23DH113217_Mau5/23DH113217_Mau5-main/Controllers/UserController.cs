using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _23DH113217_Mau5.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult User()
        {
            return View();
        }
        public ActionResult Welcome()
        {
            return View();
        }
    }
}