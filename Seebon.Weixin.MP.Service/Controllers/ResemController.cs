using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seebon.Weixin.MP.Service.CommonService;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.Controllers
{
    public class ResemController : Controller
    {
        //
        // GET: /Resem/
        ResemService RBLL = new ResemService();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Resem resem)
        {
            if (ModelState.IsValid)
            {
                resem.Guid = Guid.NewGuid().ToString();
                int newId = RBLL.Add(resem);
                return RedirectToAction("Success", "Resem", new { id = newId, guid = resem.Guid });
            }
            else
            {
                return View();
            }
        }

        public ActionResult Success(int id, string guid)
        {
            string s = "http://qr.liantu.com/api.php?text=http://weixin.seebon.com/Resem/GetView/" + id + "?guid=" + guid + "";
            ViewData["imgsrc"] = s;
            return View();
        }

        public ActionResult GetView(int id, string guid)
        {
            return View(RBLL.GetModel(id, guid));
        }
    }
}
