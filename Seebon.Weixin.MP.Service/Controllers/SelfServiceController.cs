using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seebon.Weixin.MP.Service.CommonService;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.Controllers
{
    public class SelfServiceController : Controller
    {
        //
        // GET: /SelfService/

        /// <summary>
        /// 绑定用户
        /// </summary>
        /// <param name="wx_tokenid"></param>
        /// <param name="reBind">1-重新绑定</param>
        /// <returns></returns>
        public ActionResult BindUser(string wx_tokenid, string reBind = "0",string DelBind="0")
        {
            if (reBind == "1")
                return View();
            var userService = new UserService();
            if (userService.CheckBinded(wx_tokenid))//绑定则返回已经绑定页面
            {
                if (DelBind == "1")
                {
                    userService.DelBindUser(wx_tokenid);
                }
                else
                {
                    ViewBag.IsBind = true;
                    ViewBag.OpenId = wx_tokenid;
                }
                return View();
            }
            return View();//未绑定则返回绑定页面
        }
        /// <summary>
        /// 绑定用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="reBind">1-重新绑定</param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult BindUser(ViewUserAccount user,string reBind="0")
        {
            var userService = new UserService();
            if (!userService.CheckLogin(user.UserName, user.Password))//账号密码不正确则返回重新填写
            {
                return Content("error");
            }
            //验证正确则返回成功页面，用ViewBag.IsBind记录
            userService.BindUser(user,reBind=="1");
            ViewBag.IsBind = true;
            return Content(user.wx_tokenid);
        }

        /// <summary>
        /// 查询过去一年社保
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchWelfare(string wp_tokenid)
        {
            var selfQueryService = new SelfQueryService();
            var welfareList = selfQueryService.GetWelfareDetail(wp_tokenid, DateTime.Now);
            ViewBag.OpenId = wp_tokenid;
            return View(welfareList);
        }

        /// <summary>
        /// 获取json格式的指定年的社保
        /// </summary>
        /// <param name="wp_tokenid"></param>
        /// <param name="i">1-过去一年，2-再过去一年，以此类推</param>
        /// <returns></returns>
        public JsonResult LastYearWelfare(string wp_tokenid,int i=1)
        {
            var selfQueryService = new SelfQueryService();
            int iLast12 = 12-i*12;
            var welfareList = selfQueryService.GetWelfareDetail(wp_tokenid, DateTime.Now.AddMonths(iLast12));
            return Json(welfareList,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询指定年月社保
        /// </summary>
        /// <param name="wp_tokenid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public ActionResult WelfareDetail(string wp_tokenid,int? year,int? month)
        {
            
            return View((new SelfQueryService()).GetwelfareObject(wp_tokenid, year, month));
        }
        /// <summary>
        /// 查询过去一年公积金
        /// </summary>
        /// <param name="wp_tokenid"></param>
        /// <returns></returns>
        public ActionResult SearchAcc(string wp_tokenid)
        {
            var selfQueryService = new SelfQueryService();
            var AccList = selfQueryService.GetAccDetail(wp_tokenid, DateTime.Now);
            ViewBag.OpenId = wp_tokenid;
            return View(AccList);
        }
        /// <summary>
        /// 查询指定年月公积金
        /// </summary>
        /// <param name="wp_tokenid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public ActionResult AccDetail(string wp_tokenid, int? year, int? month)
        {
            return View((new SelfQueryService()).GetAccObject(wp_tokenid, year, month));
        }
    }
}
