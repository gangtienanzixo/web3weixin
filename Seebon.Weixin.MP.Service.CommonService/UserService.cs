using System;
using System.Linq;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.CommonService
{
    public class UserService
    {
        /// <summary>
        /// 由openid获取绑定用户的对象
        /// </summary>
        /// <param name="wx_tokenid"></param>
        /// <returns></returns>
        public BindUsers GetUserByOpenId(string wx_tokenid)
        {
            return (new seebonweixinEntities()).BindUsers.FirstOrDefault(p => p.weixinid == wx_tokenid && p.isenabled);
        }
        /// <summary>
        /// 验证是否绑定成功
        /// </summary>
        /// <param name="wx_tokenid"></param>
        /// <returns></returns>
        public bool CheckBinded(string wx_tokenid)
        {
            return GetUserByOpenId(wx_tokenid) != null;
        }

        /// <summary>
        /// 验证账户和密码是否正确
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool CheckLogin(string name, string pwd)
        {
            bool login = true;
            var db = new seebonweixinEntities();

            s_user_Login_Result result = db.s_user_Login(name, pwd, "").FirstOrDefault();
            if (result==null||result.userID < 1)
            {
                db.newseebon_Login(name, pwd);
                s_user_Login_Result vResult = db.s_user_Login(name, pwd, "").FirstOrDefault();
                if (vResult==null||vResult.userID < 1)
                {
                    login = false;
                }
            }
            return login;
        }

        /// <summary>
        /// 绑定用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="reBind">true-重新绑定</param>
        public void BindUser(ViewUserAccount user, bool reBind = false)
        {
            var db = new seebonweixinEntities();
            if (reBind)//重新绑定则把之前的绑定设为不可用
            {
                var users = db.BindUsers.Where(p => p.weixinid == user.wx_tokenid);
                foreach (var binduser in users)
                {
                    binduser.isenabled = false;
                }

            }
            var bindusers = new BindUsers
            {
                idcard = user.UserName,
                weixinid = user.wx_tokenid,
                bindtime = DateTime.Now,
                isenabled = true
            };

            db.BindUsers.Add(bindusers);
            db.SaveChanges();
        }
        /// <summary>
        /// 解除绑定用户
        /// </summary>
        /// <param name="wx_tokenid">wx_tokenid</param>
        public void DelBindUser(string wx_tokenid)
        {
            var db = new seebonweixinEntities();
            var users = db.BindUsers.Where(p => p.weixinid == wx_tokenid);
            foreach (var binduser in users)
            {
                binduser.isenabled = false;
            }
            db.SaveChanges();
        }
    }
}