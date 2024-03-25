 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.Security;
using Seebon.Weixin.MP.Service.CommonService;
using Seebon.Weixin.MP.Service.CommonService.Model;
using System.Configuration;
namespace Seebon.Weixin.MP.Service.Areas.Admin.Controllers
{
    public class FocusUserController : ABaseController
    {
        //
        // GET: /Admin/FocusUser/
        FocusUserService FBLL = new FocusUserService();

        public ActionResult Index(int? p, string key = "")
        {
            ViewData["key"] = key;
            int pageIndex = p ?? 1;
            int pageSize = 10;
            int totalCount = 0;
            var FocusUser = FBLL.GetPageList(pageIndex, pageSize, key,ref totalCount);
            var FocusUserIPagedList = new StaticPagedList<FocusUsers>(FocusUser, pageIndex, pageSize, totalCount);
            return View(FocusUserIPagedList);
        }

        public ActionResult FocusUserManger(int id)
        {
            return View(FBLL.GetModel(id));
        }

        [HttpPost]
        public ActionResult GetWeiXinList(string accesstoken, string nextopenid)
        {
            if (accesstoken != "" && accesstoken != "")
            {
                FBLL.GetWeixinList(accesstoken, nextopenid);
            }
            return RedirectToAction("Index", "FocusUser");
        }

    }
}
