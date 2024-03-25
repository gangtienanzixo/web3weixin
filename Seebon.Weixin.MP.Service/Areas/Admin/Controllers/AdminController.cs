using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seebon.Weixin.MP.Service.CommonService;
using Seebon.Weixin.MP.Service.CommonService.Model;
using System.Configuration;
using System.Web.Security;

namespace Seebon.Weixin.MP.Service.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        Administrator AModel = new Administrator();
        AdminService ABLL = new AdminService();
        //
        // GET: /Admin/Admin/

        public ActionResult Index()
        {
            return View();
        }

        //登录
        public ActionResult AdminLogin()
        {
            return View();
        }

        //后台首页
        public ActionResult AdminIndex()
        {

            return View();
        }

        //退出
        [HttpGet]
        public ActionResult AdminExit()
        {
            ABLL.ClearLoginCookies();
            return RedirectToAction("AdminLogin", "Admin");

        }
        [HttpPost]
        public ActionResult AdminLogin([Bind(Include = "UserName,UserPwd")] Administrator AdminLogin)
        {
            if (ABLL.ValidateAdmin(AdminLogin.UserName, AdminLogin.UserPwd))
            {
                AdminLogin.UserPwd = ABLL.EncryptSHA1(AdminLogin.UserPwd);
                ABLL.SaveLoginCookies(AdminLogin.UserName, AdminLogin.UserPwd);//保存COOKIE
                return RedirectToAction("AdminIndex", "Admin");
            }
            else
            {
                ModelState.AddModelError("", "您输入的账号或密码错误");
                return View();
            }
        }

        private bool ValidateAdmin(string UserName, string Userpwd)
        {
            //return false;
            Userpwd = ABLL.EncryptSHA1(Userpwd);
            var db = new seebonweixinEntities();
            var Admin = (from A in db.Administrator where A.UserName == UserName && A.UserPwd == Userpwd select A).FirstOrDefault();
            return (Admin != null);
        }

    }
}
