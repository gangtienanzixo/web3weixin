using System;
using System.Linq;
using Seebon.Weixin.MP.Entities;
using Seebon.Weixin.MP.Helpers;

namespace Seebon.Weixin.MP.Service.CommonService
{
    /// <summary>
    /// 事件处理程序，此代码的简化MessageHandler方法已由/CustomerMessageHandler/CustomerMessageHandler_Event.cs完成，
    /// 此文件不再更新。
    /// </summary>
    public class EventService
    {
        public ResponseMessageBase GetResponseMessage(RequestMessageEventBase requestMessage)
        {
            var db = new Model.seebonweixinEntities();
            ResponseMessageBase responseMessage = null;
            switch (requestMessage.Event)
            {
                case Event.ENTER:
                    {
                        var strongResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                        strongResponseMessage.Content = "您刚才发送了ENTER事件请求。";
                        responseMessage = strongResponseMessage;
                        break;
                    }
                case Event.LOCATION:
                    throw new Exception("暂不可用");
                    //break;
                case Event.subscribe://订阅
                    {
                        var strongResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                        strongResponseMessage.Content = "欢迎关注仕邦微信公众平台。\r\n仕邦官方网址：http://www.Seebon.com";
                        responseMessage = strongResponseMessage;
                        break;
                    }
                case Event.unsubscribe://退订
                    {
                        //实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
                        //unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。
                        var strongResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                        strongResponseMessage.Content = "有空再来";
                        responseMessage = strongResponseMessage;
                        break;
                    }
                case Event.CLICK://菜单点击事件，根据自己需要修改
                    {
                        var strongResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                        var fixedAnswer = db.FixedAnswer.ToList();
                        foreach (var fixedAnswerItem in fixedAnswer)
                        {
                            if (requestMessage.EventKey == fixedAnswerItem.key)
                            {
                                strongResponseMessage.Content = fixedAnswerItem.content;
                                responseMessage = strongResponseMessage;
                                break;
                            }
                        }
                        //throw new Exception("Demo中还没有加入CLICK的测试！");
                        break;
                    }    
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return responseMessage;         
        }
    }
}