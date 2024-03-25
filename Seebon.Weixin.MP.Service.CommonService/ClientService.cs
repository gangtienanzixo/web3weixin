using System.Collections.Generic;
using System.Data;
using System.Linq;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.CommonService
{
    public class ClientService
    {
        /// <summary>
        /// 进入等待接入队列
        /// </summary>
        /// <param name="clientopenId"></param>
        /// <param name="connectUser"></param>
        public void WaitingForCustomer(string clientopenId,string connectUser )
        {
            using (seebonweixinEntities entities = new seebonweixinEntities())
            {
                OnlineClient curOnlineClient =
                    entities.OnlineClient.FirstOrDefault(p => p.OpenId == clientopenId && p.Status == 1&&p.ConnectUserName==connectUser);
                if (curOnlineClient != null)
                {
                    return;
                }
                OnlineClient onlineClient = new OnlineClient { OpenId = clientopenId, Status = 1, ConnectUserName = connectUser };
                entities.OnlineClient.Add(onlineClient);
                entities.SaveChanges();
            }
        }

        /// <summary>
        /// 退出等待或对话队列
        /// </summary>
        /// <param name="clientopenId"></param>
        public bool ExitWaiting(string clientopenId)
        {
            using (seebonweixinEntities entities = new seebonweixinEntities())
            {
                OnlineClient onlineClient = GetOnServiceClient(clientopenId);

                if (onlineClient == null)
                {
                    return false;
                }
                if (onlineClient.Status == 2)//与客服连线中
                {
                    CustomerService customersrv = new CustomerService();
                    customersrv.DisConCustomer(onlineClient.ConnectUserName);
                }
                entities.Entry(onlineClient).State = EntityState.Deleted;
                entities.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 客户是否已经和客服已经连线
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public OnlineClient GetOnServiceClient(string openId)
        {
            using (seebonweixinEntities entities = new seebonweixinEntities())
            {
                OnlineClient onlineClient = entities.OnlineClient.FirstOrDefault(c => c.OpenId == openId && c.Status==(int)OnlineClientStatus.Connected);//连线中的客户
                return onlineClient;
            }
        }

        /// <summary>
        /// 返回全部咨询用户列表
        /// </summary>
        /// <returns></returns>
        public IList<getOnlineClientList_Result> OnlineClientList()
        {
            using (seebonweixinEntities entities = new seebonweixinEntities())
            {
                return entities.getOnlineClientList().ToList();
            }
        }

        /// <summary>
        /// 添加对话记录
        /// </summary>
        /// <param name="record"></param>
        public void CreateRecordDialog(DialogRecord record)
        {
            using (seebonweixinEntities entities = new seebonweixinEntities())
            {
                entities.DialogRecord.Add(record);
                entities.SaveChanges();
            }
        }

        /// <summary>
        /// 返回当前用户的对话记录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public IList<DialogRecord> MyMsgList(string username,int pageIndex, int pageSize, ref int totalCount)
        {
            using (seebonweixinEntities entities = new seebonweixinEntities())
            {
                IQueryable<DialogRecord> dialoglist;
                var admin = entities.Administrator.FirstOrDefault(p => p.UserName == username);

                if (admin!=null && admin.UserType == 1)
                {
                    dialoglist =
                        entities.DialogRecord.OrderByDescending(p => p.SendTime)
                            .Skip((pageIndex - 1)*pageSize)
                            .Take(pageSize);
                    totalCount = entities.DialogRecord.Count();
                }
                else
                {
                    dialoglist = entities.DialogRecord.Where(p => p.FromId == username || p.ToId == username).OrderByDescending(p=>p.SendTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                    totalCount = entities.DialogRecord.Count(p => p.FromId == username || p.ToId == username);
                }
                    
                return dialoglist.ToList();
            }
        }

        /// <summary>
        /// 返回指定客服用户的咨询用户列表
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public IList<getMyOnlineClientList_Result> MyOnlineClientList(string username)
        {
            using (seebonweixinEntities entities = new seebonweixinEntities())
            {
                return entities.getMyOnlineClientList(username).ToList();
            }
        }
    }
}
