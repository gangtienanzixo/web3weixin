﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Seebon.Weixin.MP.CommonAPIs;
using Seebon.Weixin.MP.Entities;
using Seebon.Weixin.MP.HttpUtility;

namespace Seebon.Weixin.MP.AdvancedAPIs
{
    public static class Groups
    {
        /// <summary>
        /// 发送文本信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static GroupsJson Get(string accessToken)
        {
            var urlFormat = "https://api.weixin.qq.com/cgi-bin/groups/get?access_token={0}";
            var url = string.Format(urlFormat, accessToken);
            return HttpUtility.Get.GetJson<GroupsJson>(url);
        }

        /// <summary>
        /// 创建分组
        /// </summary>
        /// <returns></returns>
        public static CreateGroupResult Create(string accessToken, string name)
        {
            var urlFormat = "https://api.weixin.qq.com/cgi-bin/groups/create?access_token={0}";
            var data = new
            {
                group = new
                {
                    name = name
                }
            };
            return CommonJsonSend.Send<CreateGroupResult>(accessToken, urlFormat, data);
        }

        /// <summary>
        /// 创建分组
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="id"></param>
        /// <param name="name">分组名字（30个字符以内）</param>
        /// <returns></returns>
        public static WxJsonResult Update(string accessToken, int id, string name)
        {
            var urlFormat = "https://api.weixin.qq.com/cgi-bin/groups/update?access_token={0}";
            var data = new
            {
                group = new
                {
                    id = id,
                    name = name
                }
            };
            return CommonJsonSend.Send(accessToken, urlFormat, data);
        }

        /// <summary>
        /// 移动用户分组
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <param name="toGroupId"></param>
        /// <returns></returns>
        public static WxJsonResult MemberUpdate(string accessToken, string openId,int toGroupId)
        {
            var urlFormat = "https://api.weixin.qq.com/cgi-bin/groups/members/update?access_token={0}";
            var data = new
            {
                openid=openId,
                to_groupid =toGroupId
            };
            return CommonJsonSend.Send(accessToken, urlFormat, data);
        }
    }
}
