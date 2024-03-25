using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seebon.Weixin.MP.Service.CommonService;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.Controllers
{
    public class OnlineController : Controller
    {
        //
        // GET: /online/
       
        public ActionResult Index()
        {
            List<SelectListItem> ListTellType = new List<SelectListItem>();
            ListTellType.Add(new SelectListItem{Text="Tel",Value="Tel"});
            ListTellType.Add(new SelectListItem{Text="QQ",Value="QQ"});
            ListTellType.Add(new SelectListItem{Text="Email",Value="Email"});
            ViewBag.TelType = ListTellType;
            return View();
        }

        [HttpPost]
        public ActionResult Index(GuestBook GuestBook)
        {
            if (ModelState.IsValid)
            {
                GuestBookService gb = new GuestBookService();
                gb.AddGB(GuestBook);
                return Content("ok");

            }
            else
            {
                return Content("error");
            }
            
        }


        public ActionResult Discuss()
        {
            return View();
        }

        public ActionResult Customer()
        {
            return View();
        }
    }
}
