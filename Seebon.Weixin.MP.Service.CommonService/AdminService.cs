using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using Seebon.Weixin.MP.Common;
using Seebon.Weixin.MP.Service.CommonService.Model;
using System.Web.Security;

namespace Seebon.Weixin.MP.Service.CommonService
{
   public class AdminService
    {
       seebonweixinEntities db = new seebonweixinEntities();
       /// <summary>
       /// 保存登陆Cookies
       /// </summary>
       /// <param name="UserName"></param>
       /// <param name="UserPwd"></param>
        public void SaveLoginCookies(string UserName, string UserPwd)
        {
            string CookiesName;
            CookiesName = "Admin_Login";
            HttpCookie NewCookie = new HttpCookie(CookiesName);
            NewCookie.Values["UserName"] =System.Web.HttpUtility.UrlEncode(UserName);
            NewCookie.Values["UserPwd"] = System.Web.HttpUtility.UrlEncode(UserPwd);
            HttpContext.Current.Response.AppendCookie(NewCookie);
        }

       /// <summary>
       /// 清除Cookies
       /// </summary>
        public void ClearLoginCookies()
        {
            string CookiesName;
            CookiesName = "Admin_Login";
            HttpContext.Current.Response.Cookies[CookiesName].Values.Clear();

        }

        /// <summary>
        /// 返回登录成功管理员信息
        /// </summary>
        /// <returns></returns>
        public Administrator GetCurLoginUser()
        {
            Administrator admin = new Administrator();
            const string cookiesName = "Admin_Login";
            admin.UserName = CookieHelper.GetCookieValue(cookiesName, "UserName", true);
            admin.UserPwd = CookieHelper.GetCookieValue(cookiesName, "UserPwd", true);
            return admin;
        }

       /// <summary>
        /// 验证管理员登录是否成功
       /// </summary>
       /// <returns></returns>
       public bool CheckIsLogin()
       {
           Administrator Admin = GetCurLoginUser();
           if (Admin.UserName == "" || Admin.UserPwd == "")
           {
               return false;
           }
           var dtAdmin = (from A in db.Administrator where A.UserName == Admin.UserName && A.UserPwd == Admin.UserPwd select A).FirstOrDefault();
           if (dtAdmin != null)
           {
               return true;
           }
           else
           {
               return false;
           }
        }

        public bool ValidateAdmin(string UserName, string Userpwd)
        {
            //return false;
            Userpwd = EncryptSHA1(Userpwd);
            var Admin = (from A in db.Administrator where A.UserName == UserName && A.UserPwd == Userpwd select A).FirstOrDefault();
            return (Admin != null);
        }
       /// <summary>
       /// 加密
       /// </summary>
       /// <param name="userpwd"></param>
       /// <returns></returns>
        public string EncryptSHA1(string userpwd)
        {
            string pwSalt = "seebon";
            return FormsAuthentication.HashPasswordForStoringInConfigFile(pwSalt + userpwd, "SHA1");
        }

       /// <summary>
       /// 获取分页列表
       /// </summary>
       /// <returns></returns>
        public List<Administrator> GetPageList(int pageIndex, int pageSize, ref int totalCount)
        {
            var PageView = (from p in db.Administrator
                         orderby p.Id descending
                         select p).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            totalCount = db.Administrator.Count();
            return PageView.ToList();
        }
       /// <summary>
       /// 获取多个值
       /// </summary>
       /// <returns>数值ID</returns>
        public List<Administrator> GetList(int[] id)
        {

            return db.Administrator.Where(d =>id.Contains(d.Id)).ToList();
        }

       /// <summary>
       /// 获取一个实体
       /// </summary>
       /// <param name="id">主键ID</param>
       /// <returns></returns>
        public Administrator GetModel(int id)
        {
            return db.Administrator.Find(id);
        }

       /// <summary>
       /// 删除实体对象
       /// </summary>
       /// <param name="Administrator"></param>
        public void Remove(Administrator Administrator)
        {
            db.Administrator.Remove(Administrator);
            db.SaveChanges();
        }
       /// <summary>
       /// 修改
       /// </summary>
       /// <param name="Administrator"></param>
        public void Update(Administrator Administrator)
        {
            var newAdmin =GetModel(Administrator.Id);
            newAdmin.IsLock = Administrator.IsLock;
            newAdmin.ReadName = Administrator.ReadName;
            newAdmin.UserEmail = Administrator.UserEmail;
            newAdmin.UserName = Administrator.UserName;
            newAdmin.UserPwd = Administrator.UserPwd;
            newAdmin.UserType = Administrator.UserType;
            db.SaveChanges();
        }
       /// <summary>
       /// 添加数据
       /// </summary>
       /// <param name="Administrator"></param>
        public bool Add(Administrator Administrator)
        {
            var chk_UserName = db.Administrator.Where(a => a.UserName == Administrator.UserName).FirstOrDefault();
            if (chk_UserName != null)
            {
                return false;
            }
            db.Administrator.Add(Administrator);
            db.SaveChanges();
            return true;
        }

    }
}
