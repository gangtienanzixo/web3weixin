﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Seebon.Weixin.MP.Entities;
using Seebon.Weixin.MP.HttpUtility;

namespace Seebon.Weixin.MP.CommonAPIs
{
    public static class CommonJsonSend
    {
        /// <summary>
        /// 向需要AccessToken的API发送消息的公共方法
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="urlFormat"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static WxJsonResult Send(string accessToken, string urlFormat, object data)
        {
            return Send<WxJsonResult>(accessToken, urlFormat, data);
        }

        public static T Send<T>(string accessToken, string urlFormat, object data)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            var jsonString = js.Serialize(data);

            using (MemoryStream ms = new MemoryStream())
            {
                var bytes = Encoding.UTF8.GetBytes(jsonString);
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);

                var url = string.Format(urlFormat, accessToken);
                var result = Post.PostGetJson<T>(url, null, ms);
                return result;
            }
        }
    }
}
