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
    public class ManageController : ABaseController
    {
        //
        // GET: /Admin/Manage/
        Administrator AModel = new Administrator();
        AdminService ABLL = new AdminService();

        public ActionResult Index()
        {
            return View();
        }

        //管理员列表
        public ActionResult AdminList(int? p)
        {
            int pageIndex = p ?? 1;
            int pageSize = 7;
            int totalCount = 0;
            var Admins = ABLL.GetPageList(pageIndex, pageSize, ref totalCount);
            var AdminsAsIPagedList = new StaticPagedList<Administrator>(Admins, pageIndex, pageSize, totalCount);
            return View(AdminsAsIPagedList);
        }

        //后台首页
        public ActionResult AdminCenter()
        {
            return View();
        }


        [HttpPost]
        public string DeleteAll(string cb_id)
        {
            string IsOK = "ok";
            try
            {
                string[] scb_id = cb_id.Split(',');
                int[] acb_id = new int[scb_id.Length-1];
                for (int i = 0; i < scb_id.Length - 1; i++)
                {
                    acb_id[i] = int.Parse(scb_id[i].ToString());
                }
                foreach (var Dadmin in ABLL.GetList(acb_id))
                {
                    ABLL.Remove(Dadmin);                    
                }
            }
            catch (Exception e)
            {
                IsOK = e.Message;
            }
            return IsOK;
            
        }


        #region 添加，编辑
        [HttpGet]
        public ActionResult AdminManger(int id = 0)
        {
            if (id != 0)
            {
                AModel = ABLL.GetModel(id);
                return View(AModel);
            }
            return View();
        }
        [HttpPost]
        public ActionResult AdminManger(Administrator administrator, string UserPwd2)
        {
            if (ModelState.IsValid)
            {
                if (UserPwd2 == "")
                {
                    ModelState.AddModelError("UserPwd2", "请再次输入密码");
                    return View();
                }
                else if (!administrator.UserPwd.Equals(UserPwd2))
                {
                    ModelState.AddModelError("UserPwd2", "两次输入密码不一致,请重新输入！");
                    return View();
                }

                administrator.UserPwd = ABLL.EncryptSHA1(administrator.UserPwd);
                if (administrator.Id == 0)
                {
                    bool IsAdd = ABLL.Add(administrator);
                  if (!IsAdd)
                  {
                      ModelState.AddModelError("UserName", "该账号已经被注册了！");
                      return View();
                  }
                }
                else
                {
                    ABLL.Update(administrator);
                }
                return RedirectToAction("AdminList");
            }
            else
            {
                return View(administrator);
            }
        }
        #endregion

    }
}

