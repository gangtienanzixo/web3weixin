using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Seebon.Weixin.MP.Common
{
    public class CookieHelper
    {

        /// 删除COOKIE对象
        /// <summary>
        /// 删除COOKIE对象
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        public static void Del(string strCookieName)
        {
            HttpCookie objCookie = new HttpCookie(strCookieName.Trim());
            objCookie.Expires = DateTime.Now.AddYears(-5);
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }
        /// 删除某个COOKIE对象某个Key子键
        /// <summary>
        /// 删除某个COOKIE对象某个Key子键，操作成功返回字符串"ok"，如果对象本就不存在，则返回字符串"notExist"
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)。注意：虽是修改功能，实则重建覆盖，所以时间也要重设，因为没办法获得旧的有效期</param>
        /// <returns>如果对象本就不存在，则返回字符串"notExist"，如果操作成功返回字符串"ok"。</returns>
        public static string Del(string strCookieName, string strKeyName, int iExpires)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return "notExist";
            }
            else
            {
                HttpCookie objCookie = HttpContext.Current.Request.Cookies[strCookieName];
                objCookie.Values.Remove(strKeyName);
                if (iExpires > 0)
                {
                    if (iExpires == 1)
                    {
                        objCookie.Expires = DateTime.MaxValue;
                    }
                    else
                    {
                        objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
                    }
                }
                HttpContext.Current.Response.Cookies.Add(objCookie);
                return "ok";
            }
        }
        public static string ClearAll()
        {
            foreach (string k in HttpContext.Current.Request.Cookies.AllKeys)
            {
                HttpContext.Current.Response.Cookies[k].Value = string.Empty;
            }
            return "ok";
        }

        //获得指定的cookie的值
        //参数 IsUrlDecode--表示是否以URL字符串进行解码
        public static string GetCookieValue(string cookiename, bool IsUrlDecode)
        {

            HttpCookie acookie;
            acookie = HttpContext.Current.Request.Cookies[cookiename];
            if (acookie == null)
            {
                acookie = new HttpCookie(cookiename, "");
                return "";
            }
            else
            {
                if (IsUrlDecode)
                {
                    return System.Web.HttpUtility.UrlDecode(acookie.Value);
                }
                else
                {
                    return acookie.Value;
                }

            }

        }

        //获得指定的cookie的值
        //参数 IsUrlDecode--表示是否以URL字符串进行解码
        public static string GetCookieValue(string fcookiename, string cookiename, bool IsUrlDecode)
        {

            HttpCookie acookie;
            acookie = HttpContext.Current.Request.Cookies[fcookiename];
            if (acookie == null)
            {
                acookie = new HttpCookie(fcookiename, "");
                return "";
            }
            else
            {
                if (IsUrlDecode)
                {
                    return System.Web.HttpUtility.UrlDecode(acookie[cookiename]);
                }
                else
                {
                    return acookie[cookiename];
                }

            }

        }



    }

}
