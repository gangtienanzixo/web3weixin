using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Seebon.Weixin.MP.AdvancedAPIs;
using Seebon.Weixin.MP.Common;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.CommonService
{
   public class FocusUserService
    {
       seebonweixinEntities db = new seebonweixinEntities();
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <returns></returns>
        public List<FocusUsers> GetPageList(int pageIndex, int pageSize,string key, ref int totalCount)
        {
            var PageView = (from p in db.FocusUsers
                            where p.Openid.Contains(key) || p.nickname.Contains(key)
                         orderby p.Id descending
                         select p).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            totalCount = GetKeyCount(key).Count();
            return PageView.ToList();
        }

        public List<FocusUsers> GetKeyCount(string key)
        {
            return db.FocusUsers.Where(p => p.Openid.Contains(key) || p.nickname.Contains(key)).ToList();
        }
        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public FocusUsers GetModel(int id)
        {
            return db.FocusUsers.Find(id);
        }

       public FocusUsers GetModel(string openid)
       {
           return db.FocusUsers.FirstOrDefault(p => p.Openid == openid);
       }

       /// <summary>
       /// 获取关注者列表
       /// </summary>
       /// <param name="accessToken"></param>
       /// <param name="nextOpendId"></param>
        public void GetWeixinList(string accessToken,string nextOpendId)
        {           
            AdvancedAPIs.OpenIdResultJson OpenIdList = new AdvancedAPIs.OpenIdResultJson();
            OpenIdList = User.Get(accessToken, nextOpendId);
            if (OpenIdList != null)
            {
                List<string> OpenId = new List<string>();
                OpenId = OpenIdList.data.openid;
                foreach (var id in OpenId)
                {
                    AdvancedAPIs.UserInfoJson UserInfo = new AdvancedAPIs.UserInfoJson();
                    UserInfo = User.Info(accessToken, id);
                    if (UserInfo != null)
                    {
                        var IsAdd = db.FocusUsers.Where(p => UserInfo.openid == p.Openid).FirstOrDefault();
                        if (IsAdd == null)
                        {
                            FocusUsers focususer = new FocusUsers();
                            focususer.Openid = UserInfo.openid;
                            focususer.nickname = UserInfo.nickname;
                            focususer.sex = UserInfo.sex;
                            focususer.city = UserInfo.city;
                            focususer.country = UserInfo.country;
                            focususer.province = UserInfo.province;
                            focususer.language = UserInfo.language;
                            focususer.headimgurl = UserInfo.headimgurl;
                            DateTime stime = Helpers.DateTimeHelper.GetDateTimeFromXml(UserInfo.subscribe_time);
                            focususer.subscribe_time = stime.ToString();
                            db.FocusUsers.Add(focususer);
                            db.Configuration.ValidateOnSaveEnabled = false;
                            int count = db.SaveChanges();
                            db.Configuration.ValidateOnSaveEnabled = true;
                        }
                    }
                }
            }
        }

       /// <summary>
       /// 记录一条关注用户的记录
       /// </summary>
       /// <param name="openid"></param>
       /// <returns></returns>
       public bool CreateFocusUser(string openid)
       {
           UserInfoJson userInfo = User.Info(Common.Common.GetAccessToken, openid);
           if (userInfo == null) return false;
           var isAdd = db.FocusUsers.FirstOrDefault(p => userInfo.openid == p.Openid);
           if (isAdd != null) return false;
           FocusUsers focususer = new FocusUsers
           {
               Openid = userInfo.openid,
               nickname = userInfo.nickname,
               sex = userInfo.sex,
               city = userInfo.city,
               country = userInfo.country,
               province = userInfo.province,
               language = userInfo.language,
               headimgurl = userInfo.headimgurl
           };
           DateTime stime = Helpers.DateTimeHelper.GetDateTimeFromXml(userInfo.subscribe_time);
           focususer.subscribe_time = stime.ToString();
           db.FocusUsers.Add(focususer);
           return db.SaveChanges()>0;
       }

       /// <summary>
       /// 根据openid删除关注用户的记录
       /// </summary>
       /// <param name="openid"></param>
       /// <returns></returns>
       public bool DeleteFocusUser(string openid)
       {
           FocusUsers focusUsers = db.FocusUsers.FirstOrDefault(p => p.Openid == openid);
           if (focusUsers == null) return false;
           db.Entry(focusUsers).State = EntityState.Deleted;
           return db.SaveChanges()>0;
       }
    }
}
