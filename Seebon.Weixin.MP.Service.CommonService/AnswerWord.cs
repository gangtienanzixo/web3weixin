using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seebon.Weixin.MP.Service.CommonService
{
    public class AnswerWord
    {
        public string CustNewclient 
        {
            get
            {
                return "【系统信息】有一位新客户\"{0}\"已经接入。";
            }
        }

        public string ClientCustomerNotOnline
        {
            get
            {
                return "【温馨提示】当前没有客服在线，请在稍候时间再试或者 <a href=\"http://weixin.seebon.com/online/index \">在线留言</a> ，谢谢！";
            }
        }

        public string ClientConnectToCustomer
        {
            get
            {
                return "【温馨提示】您好，已经接入在线客服，您现在可以和客服进行对话。";
            }
        }

        public string ClientCannotConnectToCustomer
        {
            get
            {
                return "【温馨提示】暂时不能接入客服，请稍候再试。";
            }
        }

        public string ClientWaitingToConnect
        {
            get
            {
                return "【温馨提示】在线客服正忙，我们将尽快和你连线，请稍等；您也可以发送【00】退出等待服务";
            }
        }

        public string CustLogout
        {
            get
            {
                return "【温馨提示】您已退出登录，谢谢！";
            }
        }

        public string CustLogin
        {
            get
            {
                return "【系统信息】您已登录成功。";
            }
        }
        

        public string ClientLogoutAndDisconnect
        {
            get
            {
                return "【温馨提示】您好，在线客服已经离线，您已经断开与客服的连线，谢谢。";
            }
        }

        public string ClientDisconnect
        {
            get
            {
                return "【温馨提示】您好，已经断开在线客服，感谢您的咨询。";
            }
        }

        public string ClientConnect
        {
            get
            {
                return "【温馨提示】您好，已经接入在线客服，您现在可以和客服进行对话。";
            }
        }

        public string ClientDisconnected
        {
            get
            {
                return "感谢您的咨询。再见！";
            }
        }

        public string CustDisconnected
        {
            get
            {
                return "【温馨提示】您好，客户已经断开连接，您可以发送$66$接入下一个客户";
            }
        }

        public string CustNotWaitingCustomer
        {
            get
            {
                return "【系统信息】暂时没有客户接入";
            }
        }
    }
}
