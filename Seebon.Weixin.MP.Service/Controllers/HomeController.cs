using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Seebon.Weixin.MP.Service.CommonService;

namespace Seebon.Weixin.MP.Service.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {
            //TextService textService = new TextService();
            //ViewBag.text = textService.GetText();
            //return View();
            return Redirect("/pages/index.html");
        }
    }
}
