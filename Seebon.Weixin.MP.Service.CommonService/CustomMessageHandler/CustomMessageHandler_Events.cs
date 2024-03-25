using System;
using Seebon.Weixin.MP.Entities;

namespace Seebon.Weixin.MP.Service.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase reponseMessageEvent = null;
            //菜单点击，需要跟创建菜单时的Key匹配
            switch (requestMessage.EventKey)
            {
                    //case "OneClick":
                    //    {
                    //        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                    //        reponseMessage = strongResponseMessage;
                    //        strongResponseMessage.Content = "您点击了底部按钮。";
                    //    }
                    //    break;
                    //case "SubClickRoot_Text":
                    //    {
                    //        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                    //        reponseMessage = strongResponseMessage;
                    //        strongResponseMessage.Content = "您点击了子菜单按钮。";
                    //    }
                    //    break;
                    //case "SubClickRoot_News":
                    //    {
                    //        var strongResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                    //        reponseMessage = strongResponseMessage;
                    //        strongResponseMessage.Articles.Add(new Article()
                    //        {
                    //            Title = "您点击了子菜单图文按钮",
                    //            Description = "您点击了子菜单图文按钮，这是一条图文信息。",
                    //            PicUrl = "http://weixin.Seebon.com/Images/qrcode.jpg",
                    //            Url = "http://weixin.Seebon.com"
                    //        });
                    //    }
                    //    break;
                    //case "SubClickRoot_Music":
                    //    {
                    //        var strongResponseMessage = CreateResponseMessage<ResponseMessageMusic>();
                    //        reponseMessage = strongResponseMessage;
                    //        strongResponseMessage.Music.MusicUrl = "http://weixin.Seebon.com/Content/music1.mp3";
                    //    }
                    //    break;
                case "menu_online_discuss": //在线洽谈
                    FixedAnswerService answersrvdis = new FixedAnswerService();
                    reponseMessageEvent = answersrvdis.GetResponseMessage(requestMessage, "1");
                    break;
                case "menu_online_customer": //在线洽谈
                    FixedAnswerService answersrvcus = new FixedAnswerService();
                    reponseMessageEvent = answersrvcus.GetResponseMessage(requestMessage, "2");
                    break;
                case "menu_self_binduser": //绑定用户
                {
                    ResponseBindMessage(ref reponseMessageEvent);
                }
                    break;
                case "menu_self_welfare": //查询社保

                    var userServiceWelfare = new UserService();
                    if (!userServiceWelfare.CheckBinded(WeixinOpenId))
                        ResponseBindMessage(ref reponseMessageEvent);
                    else
                    {
                        var selfQueryService = new SelfQueryService();
                        var lastWelfare = selfQueryService.GetLastThreeWelfare(WeixinOpenId);
                        string msg = string.Empty;
                        if (lastWelfare != null)
                            msg =
                                string.Format(
                                    Common.Common.ConvertoWordWrap(
                                        @"尊敬的{0}:\n\n您的社保最新缴费信息\n年份:{1}\n月份:{2}\n单位缴费(合计):{3}\n个人缴费(合计):{4}\n\n点击""查看全文""显示更多信息"),
                                    lastWelfare.Name, lastWelfare.Year, lastWelfare.Month, lastWelfare.CompTotal,
                                    lastWelfare.PerTotal);
                        else
                            msg =
                                Common.Common.ConvertoWordWrap(
                                    @"您好，查询不到您最近三个月的缴费记录。\n如果您有疑问可以和您的客服专员联系。\n点击“查看全文”查询其他月份的缴费记录");
                        var responseMessageWelfare = CreateResponseMessage<ResponseMessageNews>();
                        responseMessageWelfare.Articles.Add(new Article()
                        {
                            Title = "社保查询",
                            PicUrl = string.Empty,
                            Description = msg,
                            Url = "http://weixin.seebon.com/SelfService/SearchWelfare?wp_tokenid=" + WeixinOpenId
                        });
                        reponseMessageEvent = responseMessageWelfare;
                    }
                    break;
                case "menu_self_acc": //查询公积金
                {
                    var userServiceAcc = new UserService();
                    if (!userServiceAcc.CheckBinded(WeixinOpenId))
                        ResponseBindMessage(ref reponseMessageEvent);
                    else
                    {
                        var selfQueryService = new SelfQueryService();
                        var lastAcc = selfQueryService.GetLastThreeAcc(WeixinOpenId);
                        string msg = string.Empty;
                        if (lastAcc != null)
                        {
                            msg =
                                string.Format(
                                    Common.Common.ConvertoWordWrap(
                                        @"尊敬的{0}:\n\n您的公积金最新缴费信息\n年份:{1}\n月份:{2}\n缴费基数:{3}\n单位缴费(合计):{4}\n个人缴费(合计):{5}\n\n点击""查看全文""显示更多信息"),
                                    lastAcc.Name, lastAcc.Year, lastAcc.Month, lastAcc.BaseAmount, lastAcc.CompAmount,
                                    lastAcc.PerAmount);
                        }
                        else
                            msg =
                                Common.Common.ConvertoWordWrap(
                                    @"您好，查询不到您最近三个月的缴费记录。\n如果您有疑问可以和您的客服专员联系。\n点击“查看全文”查询其他月份的缴费记录");
                        var responseMessageAcc = CreateResponseMessage<ResponseMessageNews>();
                        responseMessageAcc.Articles.Add(new Article()
                        {
                            Title = "公积金查询",
                            PicUrl = string.Empty,
                            Description = msg,
                            Url = "http://weixin.seebon.com/SelfService/SearchAcc?wp_tokenid=" + WeixinOpenId
                        });
                        reponseMessageEvent = responseMessageAcc;
                    }
                }
                    break;
                default:
                {
                    var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                    reponseMessageEvent = strongResponseMessage;
                    strongResponseMessage.Content = "您点击了按钮,eventkey: " + requestMessage.EventKey;
                }
                    break;
            }

            return reponseMessageEvent;
        }

        /// <summary>
        /// 返回绑定消息
        /// </summary>
        /// <param name="reponseMessage"></param>
        private void ResponseBindMessage(ref IResponseMessageBase reponseMessage)
        {
            string strUrl = string.Format("http://weixin.seebon.com/SelfService/BindUser?wx_tokenid={0}", WeixinOpenId);
            var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();

            strongResponseMessage.Content = string.Format(Common.Common.ConvertoWordWrap("您好，欢迎您使用仕邦账号绑定功能。\n\n请先 <a href=\"{0}\">绑定账号</a> ，即可享受仕邦微信更多的服务：\n\n1、查询公积金\n\n2、查询社保。"), strUrl);
            reponseMessage = strongResponseMessage;
        }

        public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "您刚才发送了ENTER事件请求。";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            throw new Exception("暂不可用");
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            FocusUserService focusUserSrv = new FocusUserService();
            focusUserSrv.CreateFocusUser(requestMessage.FromUserName);
            var fixedAnswerService = new FixedAnswerService();
            var responseMessage = fixedAnswerService.GetResponseMessage(requestMessage,"$welcome$");
            return responseMessage;
        }

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            FocusUserService focusUserSrv = new FocusUserService();
            focusUserSrv.DeleteFocusUser(requestMessage.FromUserName);
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "有空再来";
            return responseMessage;
        }
    }
}