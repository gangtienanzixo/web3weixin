using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web;
using Seebon.Weixin.MP.Context;
using Seebon.Weixin.MP.Entities;
using Seebon.Weixin.MP.MessageHandlers;
using Seebon.Weixin.MP.Helpers;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<MessageContext>
    {
        /*
         * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
         * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
         * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
         */

        public CustomMessageHandler(Stream inputStream)
            : base(inputStream)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;
        }

        public override void OnExecuting()
        {
            AnswerWord answerWord = new AnswerWord();
            if (RequestMessage == null)
            {
                return;
            }
            if (RequestMessage.MsgType == RequestMsgType.Event)//事件直接处理
            {
                ResponseMessage = OnEventRequest(RequestMessage as RequestMessageEventBase);
            }
            else
            {
                if (IsCustomer(RequestMessage.FromUserName))//客服
                {
                    if (IsCustomerLogin(RequestMessage.FromUserName))//登录
                    {
                        if (IsCustCommand())//系统命令（接入下一个，退出）
                            ResponseMessage = DoCustCommand();
                        else
                        {
                            string clientOpenid;
                            if (OnCustService(RequestMessage.FromUserName, out clientOpenid) && RequestMessage != null )//连线中
                            {
                                if (RequestMessage.MsgType == RequestMsgType.Text)
                                {
                                    ReTransmission(RequestMessage.FromUserName, clientOpenid,
                                        ((RequestMessageText) RequestMessage).Content);
                                }
                                else
                                {
                                    ReTransmission(RequestMessage, clientOpenid);
                                }
                            }
                            else//非连线中
                            {
                                CustomerReply();//todo..要忽略登录接入客服提示
                            }
                        }
                        
                    }
                    else//没有登录
                    {
                        if (IsCustCommand(false))
                            ResponseMessage = DoCustCommand(false);
                        else
                            CustomerReply();
                    }
                }
                else//客户
                {
                    string connectUser;
                    if (OnService(RequestMessage.FromUserName, out connectUser))//连线中todo..客服登录超时
                    {
                        if (IsClientCommand())
                            ResponseMessage=DoClientCommand(UserNameToOpenId(connectUser));
                        else if (RequestMessage != null)
                            if (RequestMessage.MsgType == RequestMsgType.Text)
                                ReTransmission(RequestMessage.FromUserName, UserNameToOpenId(connectUser),
                                    ((RequestMessageText) RequestMessage).Content,false);
                            else
                                ReTransmission(RequestMessage, UserNameToOpenId(connectUser));
                    }
                    else//非连线中
                    {
                        if (RequestMessage.MsgType == RequestMsgType.Text)//接收到的是文本信息
                        {
                            string keyWord = ((RequestMessageText)RequestMessage).Content;
                            if (IsConnectCommand(keyWord))//客服连接命令
                            {
                                string custUserName;
                                if (CustomerOnLine(keyWord, out custUserName)) //客服在线
                                {
                                    if (CustomerHasAvailable(custUserName)) //客服有空,进入对话
                                    {
                                        string custOpenId = UserNameToOpenId(custUserName);
                                        CustomerService customersrv = new CustomerService();
                                        var responseMessage = CreateResponseMessage<ResponseMessageText>();//提示客户接入客服
                                        if (customersrv.ConnectToCustomer(RequestMessage.FromUserName, custOpenId))
                                        {
                                            SetNickNameSession(RequestMessage.FromUserName);
                                            AdvancedAPIs.Custom.SendText(Common.Common.GetAccessToken, custOpenId,
                                                string.Format(answerWord.CustNewclient, HttpContext.Current.Session["curclientopenid"].ToString().Split(new[] { ',' })[0])); //提示客服有客户接入
                                            responseMessage.Content = answerWord.ClientConnectToCustomer;
                                        }
                                        else
                                            responseMessage.Content = answerWord.ClientCannotConnectToCustomer;
                                        ResponseMessage = responseMessage;
                                    }
                                    else //客服没空，进入等待队列
                                    {
                                        var clientsrv = new ClientService();
                                        clientsrv.WaitingForCustomer(RequestMessage.FromUserName, custUserName);
                                        var responseMessage = CreateResponseMessage<ResponseMessageText>();
                                        responseMessage.Content = answerWord.ClientWaitingToConnect;
                                        ResponseMessage = responseMessage;
                                    }
                                }
                                else //客服不在线
                                {
                                    var responseMessage = CreateResponseMessage<ResponseMessageText>();
                                    responseMessage.Content = answerWord.ClientCustomerNotOnline; 
                                    ResponseMessage = responseMessage;
                                }
                            }
                            else//非客服连接命令
                            {
                                CustomTextReply();
                            }
                        }
                        else
                        switch (RequestMessage.MsgType)//根据接收到的信息类型返回回复的信息
                        {
                            //case RequestMsgType.Text:
                            //    ResponseMessage = OnTextRequest(RequestMessage as RequestMessageText);
                            //    break;
                            case RequestMsgType.Location:
                                ResponseMessage = OnLocationRequest(RequestMessage as RequestMessageLocation);
                                break;
                            case RequestMsgType.Image:
                                ResponseMessage = OnImageRequest(RequestMessage as RequestMessageImage);
                                break;
                            case RequestMsgType.Voice:
                                ResponseMessage = OnVoiceRequest(RequestMessage as RequestMessageVoice);
                                break;
                            default:
                                throw new UnknownRequestMsgTypeException("未知的MsgType请求类型", null);
                        }
                    }
                }
            }

            CancelExcute = true;
            
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        private void ReTransmission(IRequestMessageBase requestMessage, string toOpenid)
        {
            switch (requestMessage.MsgType)//根据接收到的信息类型返回回复的信息
            {
                case RequestMsgType.Location:
                    ResponseMessage = OnLocationRequest(RequestMessage as RequestMessageLocation);
                    break;
                case RequestMsgType.Image:
                    var requestMessageImage = (RequestMessageImage) requestMessage;
                    ClientService imageclientsrv = new ClientService();
                    DialogRecord imagerecord = new DialogRecord
                    {
                        FromId = requestMessageImage.FromUserName,
                        ToId = toOpenid,
                        msgtype = "image",
                        media_id = requestMessageImage.MediaId
                    };
                    imageclientsrv.CreateRecordDialog(imagerecord);
                    AdvancedAPIs.Custom.SendVoice(Common.Common.GetAccessToken, requestMessageImage.FromUserName,
                        requestMessageImage.MediaId);
                    ResponseMessage = null;
                    break;
                case RequestMsgType.Voice:
                    var requestMessageVoice = (RequestMessageVoice) requestMessage;
                    ClientService voiceClientsrv = new ClientService();
                    DialogRecord voicerecord = new DialogRecord
                    {
                        FromId = requestMessageVoice.FromUserName,
                        ToId = toOpenid,
                        msgtype = "voice",
                        media_id = requestMessageVoice.MediaId
                    };
                    voiceClientsrv.CreateRecordDialog(voicerecord);
                    AdvancedAPIs.Custom.SendVoice(Common.Common.GetAccessToken, requestMessageVoice.FromUserName,
                        requestMessageVoice.MediaId);
                    ResponseMessage = null;
                    break;
                
            }
        }

        /// <summary>
        /// 是否客户连接命令
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        private bool IsConnectCommand(string keyWord)
        {
            var fixedanswerSrv = new FixedAnswerService();
            return fixedanswerSrv.GetCustomerModel(keyWord)!=null;
        }

        /// <summary>
        /// 客服是否在线
        /// </summary>
        /// <param name="key"></param>
        /// <param name="custUserName"></param>
        /// <returns></returns>
        private bool CustomerOnLine(string key, out string custUserName)
        {
            var fixedanswerSrv = new FixedAnswerService();
            FixedAnswer fixedanswer = fixedanswerSrv.GetModel(key);
            if (fixedanswer == null)
            {
                custUserName = string.Empty;
                return false;
            }
            custUserName = fixedanswer.AccessUser;
            return (new CustomerService()).CustomerOnline(custUserName) != null;
        }

        public string OpenIdToUserName(string openid)
        {
            var customersrv = new CustomerService();
            return customersrv.GetCustUserName(openid);
        }

        public string UserNameToOpenId(string username)
        {
            var customersrv = new CustomerService();
            return customersrv.GetCustOpenId(username);
        }

        /// <summary>
        /// 转发信息
        /// </summary>
        /// <param name="fromOpenId"></param>
        /// <param name="toOpenId"></param>
        /// <param name="content"></param>
        /// <param name="bolFromCustomer"></param>
        private void ReTransmission(string fromOpenId,string toOpenId,string content,bool bolFromCustomer=true)
        {
            ClientService client = new ClientService();
            DialogRecord record = new DialogRecord {FromId = fromOpenId, ToId = toOpenId, Content = content,msgtype="text"};
            client.CreateRecordDialog(record);//添加对话记录
            string nickName = string.Empty;
            if (!bolFromCustomer)//客户发的信息
            {
                SetNickNameSession(fromOpenId);
                nickName = string.Format("{0}:",HttpContext.Current.Session["curclientopenid"].ToString().Split(new[] {','})[0]);
            }

            AdvancedAPIs.Custom.SendText(Common.Common.GetAccessToken, toOpenId, string.Format("{0} {1}",nickName,content));
            ResponseMessage = null;
        }

        private void SetNickNameSession(string fromOpenId)
        {
            if (HttpContext.Current.Session["curclientopenid"] == null||HttpContext.Current.Session["curclientopenid"].ToString().Split(new []{','})[1]!=fromOpenId)
            {
                FocusUserService focususerSrv = new FocusUserService();
                FocusUsers focusUsers = focususerSrv.GetModel(fromOpenId);
                HttpContext.Current.Session["curclientopenid"] = focusUsers.nickname + "," + fromOpenId;
            }
        }

        /// <summary>
        /// 执行客服系统命令
        /// </summary>
        /// <param name="bolConnect">是否连线</param>
        private IResponseMessageBase DoCustCommand(bool bolConnect=true)
        {
            AnswerWord answerWord = new AnswerWord();
            var requestmessage = (RequestMessageText) RequestMessage;
            CustomerService customer = new CustomerService();
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            switch (requestmessage.Content)
            {
                case "$88$"://退出
                    IList<OnlineClient> clientList;
                    if (customer.CustomerLogout(requestmessage.FromUserName, out clientList))
                    {
                        responseMessage.Content = answerWord.CustLogout;
                        if (clientList.Count > 0) //提醒客户，客服已离线
                            foreach (var onlineClient in clientList)
                                AdvancedAPIs.Custom.SendText(Common.Common.GetAccessToken, onlineClient.OpenId,
                                   answerWord.ClientLogoutAndDisconnect);
                    }
                    break;
                case "$66$"://接入下一个
                    string msg,disconopenid,conopenid;
                    if (customer.ConnectToNext(requestmessage.FromUserName, out msg, out disconopenid, out conopenid))
                    {
                        
                        if (disconopenid != null)
                            AdvancedAPIs.Custom.SendText(Common.Common.GetAccessToken, disconopenid,
                               answerWord.ClientDisconnect); //提示断开客户
                        if (conopenid != null)
                        {
                            AdvancedAPIs.Custom.SendText(Common.Common.GetAccessToken, conopenid,
                               answerWord.ClientConnectToCustomer); //提示接入客户
                            SetNickNameSession(conopenid);
                            msg = string.Format(msg, HttpContext.Current.Session["curclientopenid"].ToString().Split(new[] { ',' })[0]);
                        }
                            
                        responseMessage.Content = msg;
                    }
                    else
                    {
                        if (disconopenid != null)
                            AdvancedAPIs.Custom.SendText(Common.Common.GetAccessToken, disconopenid,
                               answerWord.ClientDisconnect); //提示断开客户
                        responseMessage.Content = msg;
                    }
                    break;
                case "$33$": //登录
                    if (!bolConnect&&(new CustomerService()).CustomerLogin(RequestMessage.FromUserName))
                        responseMessage.Content =answerWord.CustLogin;
                    break;
            }
            
            return responseMessage;
        }

        /// <summary>
        /// 执行客户系统命令
        /// </summary>
        private IResponseMessageBase DoClientCommand(string cusOpenId)
        {
            AnswerWord answerWord = new AnswerWord();
            var requestmessage = (RequestMessageText)RequestMessage;
            ClientService client = new ClientService();
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            switch (requestmessage.Content)
            {
                case "00"://退出
                    if (client.ExitWaiting(requestmessage.FromUserName))
                    {
                        responseMessage.Content = answerWord.ClientDisconnected;
                        AdvancedAPIs.Custom.SendText(Common.Common.GetAccessToken, cusOpenId,
                                                answerWord.CustDisconnected);
                    }
                    break;
            }
            return responseMessage;
        }

        /// <summary>
        /// 是否客服系统命令
        /// </summary>
        /// <param name="bolConnect">是否连线中</param>
        /// <returns></returns>
        private bool IsCustCommand(bool bolConnect=true)
        {
            if (bolConnect)
            {
                if (RequestMessage.MsgType == RequestMsgType.Text && 
                    ((RequestMessageText)RequestMessage).Content == "$88$"||((RequestMessageText)RequestMessage).Content=="$66$")//如果是在线则只有退出和下一个
                    return true;
                return false;
            }
            if (RequestMessage.MsgType == RequestMsgType.Text &&
                ((RequestMessageText)RequestMessage).Content == "$88$" || ((RequestMessageText)RequestMessage).Content == "$66$" || ((RequestMessageText)RequestMessage).Content == "$33$")//不在线则只有登录
                return true;
            return false;
        }

        /// <summary>
        /// 是否客户系统命令
        /// </summary>
        /// <returns></returns>
        private bool IsClientCommand()
        {
            return RequestMessage.MsgType == RequestMsgType.Text && ((RequestMessageText)RequestMessage).Content == "00";
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        /// <summary>
        /// 客服是否与客户连线中
        /// </summary>
        /// <param name="custopenid"></param>
        /// <param name="clientopenid"></param>
        /// <returns></returns>
        private bool OnCustService(string custopenid, out string clientopenid)
        {
            CustomerService service = new CustomerService();
            return service.IsConnection(custopenid, out clientopenid);
        }

        /// <summary>
        /// 一般文本型信息的回复
        /// </summary>
        public void CustomTextReply()
        {
            ResponseMessage = OnTextRequest(RequestMessage as RequestMessageText);
        }
        
        /// <summary>
        /// 通常的回复
        /// </summary>
        private void CustomReply()
        {
            switch (RequestMessage.MsgType) //根据接收到的信息类型返回回复的信息
            {
                case RequestMsgType.Text:
                    ResponseMessage = OnTextRequest(RequestMessage as RequestMessageText);
                    break;
                case RequestMsgType.Location:
                    ResponseMessage = OnLocationRequest(RequestMessage as RequestMessageLocation);
                    break;
                case RequestMsgType.Image:
                    ResponseMessage = OnImageRequest(RequestMessage as RequestMessageImage);
                    break;
                case RequestMsgType.Voice:
                    ResponseMessage = OnVoiceRequest(RequestMessage as RequestMessageVoice);
                    break;
                default:
                    throw new UnknownRequestMsgTypeException("未知的MsgType请求类型", null);
            }
        }

        /// <summary>
        /// 通常的回复
        /// </summary>
        private void CustomerReply()
        {
            switch (RequestMessage.MsgType) //根据接收到的信息类型返回回复的信息
            {
                case RequestMsgType.Text:
                    var fixedAnswerService = new FixedAnswerService();
                    ResponseMessage = fixedAnswerService.GetResponseMessage(RequestMessage as RequestMessageText,true);
                    break;
                case RequestMsgType.Location:
                    ResponseMessage = OnLocationRequest(RequestMessage as RequestMessageLocation);
                    break;
                case RequestMsgType.Image:
                    ResponseMessage = OnImageRequest(RequestMessage as RequestMessageImage);
                    break;
                case RequestMsgType.Voice:
                    ResponseMessage = OnVoiceRequest(RequestMessage as RequestMessageVoice);
                    break;
                default:
                    throw new UnknownRequestMsgTypeException("未知的MsgType请求类型", null);
            }
        }


        /// <summary>
        /// 客户是否与客服连线中
        /// </summary>
        /// <param name="clientuser"></param>
        /// <param name="connectuser"></param>
        /// <returns></returns>
        public  bool OnService(string  clientuser,out string connectuser)
        {
            ClientService service = new ClientService();
            OnlineClient client = service.GetOnServiceClient(clientuser);
            if (client != null)
            {
                //todo..客服在线
                connectuser = client.ConnectUserName;
                return true;
            }
            connectuser = string.Empty;
            return false;
        }

        /// <summary>
        /// 是否登录
        /// </summary>
        /// <returns></returns>
        protected  bool IsCustomerLogin(string strCustOpenid)
        {
            CustomerService customersrv = new CustomerService();
            return customersrv.IsCustomerLogin(strCustOpenid);
        }

        /// <summary>
        /// 客服登录
        /// </summary>
        protected  void CustomerLogin()
        {
            (new CustomerService()).CustomerLogin(RequestMessage.FromUserName);
        }

        /// <summary>
        /// 是否客服
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        protected  bool IsCustomer(string openid)
        {
            return (new CustomerService()).IsCustomer(openid);
        }

        /// <summary>
        /// 客服是否有空
        /// </summary>
        public bool CustomerHasAvailable(string username)
        {
            return (new CustomerService()).IsCustomerAvailable(username);
        }

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var fixedAnswerService = new FixedAnswerService();
            var responseMessage = fixedAnswerService.GetResponseMessage(requestMessage);
            
            return responseMessage;
        }

        /// <summary>
        /// 处理位置请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {
            var locationService = new LocationService();
            var responseMessage = locationService.GetResponseMessage(requestMessage as RequestMessageLocation);
            return responseMessage;
        }

        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageNews>();
            responseMessage.Articles.Add(new Article()
            {
                Title = "您刚才发送了图片信息",
                Description = "您发送的图片将会显示在边上",
                PicUrl = requestMessage.PicUrl,
                Url = "http://weixin.Seebon.com"
            });
            return responseMessage;
        }

        /// <summary>
        /// 处理语音请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageMusic>();
            responseMessage.Music.MusicUrl = "http://weixin.Seebon.com/Content/music1.mp3";
            return responseMessage;
        }

        /// <summary>
        /// 处理链接消息请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = 
                string.Format(@"您发送了一条连接信息：\nTitle：{0}\nDescription:{1}\nUrl:{2}".Replace("\\n", ((char)10).ToString(CultureInfo.InvariantCulture)), 
                requestMessage.Title, requestMessage.Description, requestMessage.Url);
            return responseMessage;
        }

        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEventRequest(RequestMessageEventBase requestMessage)
        {
            var eventResponseMessage = base.OnEventRequest(requestMessage);//对于Event下属分类的重写方法，见：CustomerMessageHandler_Events.cs
            //TODO: 对Event信息进行统一操作
            return eventResponseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            return responseMessage;
        }
        
    }
}
