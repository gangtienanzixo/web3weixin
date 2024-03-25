using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seebon.Weixin.MP.Service.CommonService;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.Areas.Admin.Controllers
{
    public class ABaseController : Controller
    {
        //
        // GET: /Admin/ABase/
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            AdminService Admin = new AdminService();
            if (!Admin.CheckIsLogin())
                filterContext.Result = RedirectToAction("AdminLogin", "Admin");
            base.OnActionExecuting(filterContext);
        }

    }
}
