using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Seebon.Weixin.MP.Entities;
using Seebon.Weixin.MP.Helpers;
using Seebon.Weixin.MP.HttpUtility;
using System.Web.Script.Serialization;

namespace Seebon.Weixin.MP.Service.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public string SendMessage(FormCollection c)
        {
            RequestMessageText text = new RequestMessageText
            {
                ToUserName = "gh_358400b5e5b6",
                FromUserName = c["openid"],//"oXJx0jv7inCkMvITOn1eAj9yOvMc",
                CreateTime=DateTime.Now,
                Content=c["msg"],
                MsgId = 5946018059624907120
            };

            Encoding encoding = Encoding.GetEncoding("GB2312");

            string strUrl =string.Format("http://{0}:{1}/weixin", Request.Url.Host,Request.Url.Port);
            string postData = ConvertEntityToXml(text).ToString();
            byte[] data = encoding.GetBytes(postData);

            // 准备请求...
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(strUrl);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = data.Length;
            Stream newStream = myRequest.GetRequestStream();
            // 发送数据
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            return "发送成功";
        }

        /// <summary>
        /// 将实体转为XML
        /// </summary>
        /// <typeparam name="T">RequestMessage或ResponseMessage</typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public  XDocument ConvertEntityToXml<T>(T entity) where T : class , new()
        {
            entity = entity ?? new T();
            var doc = new XDocument();
            doc.Add(new XElement("xml"));
            var root = doc.Root;
            
            var propNameOrder = new List<string>();
            //不同返回类型需要对应不同特殊格式的排序
            if (entity is RequestMessageImage)
            {
                propNameOrder.AddRange(new[] { "ToUserName", "FromUserName", "CreateTime", "MsgType", "PicUrl", "MediaId", "MsgId" });
            }
            else if (entity is RequestMessageVoice)
            {
                propNameOrder.AddRange(new[] { "ToUserName", "FromUserName", "CreateTime", "MsgType", "MediaId", "Format", "MsgID " });
            }
            else if (entity is RequestMessageLink)
            {
                propNameOrder.AddRange(new[] { "ToUserName", "FromUserName", "CreateTime", "MsgType", "Title", "Description","Url","MsgID " });
            }
            else if (entity is RequestMessageLocation)
            {
                propNameOrder.AddRange(new[] { "ToUserName", "FromUserName", "CreateTime", "MsgType", "Location_X", "Location_Y", "Scale", "Label", "MsgID " });
            }
            else//如Text类型
            {
                propNameOrder.AddRange(new[] { "ToUserName", "FromUserName", "CreateTime", "MsgType", "Content", "MsgId" });
            }

            Func<string, int> orderByPropName = propNameOrder.IndexOf;

            var props = entity.GetType().GetProperties().OrderBy(p => orderByPropName(p.Name)).ToList();
            foreach (var prop in props)
            {
                var propName = prop.Name;
                if (propName == "Music")
                {
                    //音乐格式
                    var musicElement = new XElement("Music");
                    var music = prop.GetValue(entity, null) as Music;
                    var subNodes = ConvertEntityToXml(music).Root.Elements();
                    musicElement.Add(subNodes);
                    root.Add(musicElement);
                }
                else
                {
                    switch (prop.PropertyType.Name)
                    {
                        case "String":
                            root.Add(new XElement(propName,
                                                  new XCData(prop.GetValue(entity, null) as string ?? "")));
                            break;
                        case "DateTime":
                            root.Add(new XElement(propName, DateTimeHelper.GetWeixinDateTime((DateTime)prop.GetValue(entity, null))));
                            break;
                        case "Boolean":
                            if (propName == "FuncFlag")
                            {
                                root.Add(new XElement(propName, (bool)prop.GetValue(entity, null) ? "1" : "0"));
                            }
                            else
                            {
                                goto default;
                            }
                            break;
                        case "ResponseMsgType":
                            root.Add(new XElement(propName, prop.GetValue(entity, null).ToString().ToLower()));
                            break;
                        case "Article":
                            root.Add(new XElement(propName, prop.GetValue(entity, null).ToString().ToLower()));
                            break;
                        default:
                            root.Add(new XElement(propName, prop.GetValue(entity, null)));
                            break;
                    }
                }
            }
            return doc;
        }

    }
}
