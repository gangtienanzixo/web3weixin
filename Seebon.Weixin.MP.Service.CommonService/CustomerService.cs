using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.CommonService
{
    public class CustomerService
    {
        seebonweixinEntities entities = new seebonweixinEntities();

        /// <summary>
        /// 是否客服
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public bool IsCustomer(string openid)
        {
            var customer = entities.Customer.FirstOrDefault(p => p.custid == openid);
            return customer != null;
        }

        /// <summary>
        /// 客服登录
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public bool CustomerLogin(string openid)
        {
            customerlog log = new customerlog {custid = openid, logintime = DateTime.Now};
            entities.customerlog.Add(log);//记录登录
            if (entities.SaveChanges() > 0)//更改客服登录状态，时间
            {
                Customer customer = entities.Customer.FirstOrDefault(p => p.custid == openid);
                if (customer != null)
                {
                    customer.status = 1;
                    customer.logintime = DateTime.Now;
                    customer.logid = log.id;
                }
            
                return entities.SaveChanges() > 0;
            }
            return false;
        }

        /// <summary>
        /// 客服登出
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="clientList"></param>
        /// <returns></returns>
        public bool CustomerLogout(string openid,out IList<OnlineClient> clientList)
        {
            Customer customer = entities.Customer.FirstOrDefault(p => p.custid == openid && p.logintime!=null);
            clientList = null;
            if (customer != null)
            {
                customerlog log = entities.customerlog.Find(customer.logid);//记录登出时间
                if(log!=null)
                    log.logouttime = DateTime.Now;

                customer.status = 2;//客服状态置为登出状态
                customer.connecttime = null;
                customer.logintime = null;
                customer.clientid = null;
                customer.logid = null;

                //断开当前连接客户
                clientList = entities.OnlineClient.Where(p=>p.ConnectUserName==customer.accountname).ToList();
                foreach (var onlineClient in clientList)
                {
                    if (onlineClient.Status == (int) OnlineClientStatus.Connected)
                    {
                        customerrecord record =
                            entities.customerrecord.FirstOrDefault(
                                p => p.custid == openid && p.clientid == onlineClient.OpenId && p.finishtime == null);
                        if (record != null)
                        {
                            record.finishtime = DateTime.Now;
                            record.finishtype = 1;
                        }
                    }
                    entities.Entry(onlineClient).State = EntityState.Deleted;
                }

                return entities.SaveChanges() > 0;
            }
            return false;
        }

        /// <summary>
        /// 接入到下一个客户
        /// </summary>
        /// <param name="custopenid"></param>
        /// <param name="msg"></param>
        /// <param name="disconclientOpenid"></param>
        /// <param name="conclientOpenid"></param>
        /// <returns></returns>
        public bool ConnectToNext(string custopenid,out string msg,out string disconclientOpenid,out string conclientOpenid)
        {
            AnswerWord answerWord = new AnswerWord();
            Customer customer = entities.Customer.FirstOrDefault(p => p.custid == custopenid);
            msg = null;
            disconclientOpenid = null;
            conclientOpenid = null;
            if (customer == null) 
                return false;
            //登记客服与当前客户的连线记录
            customerrecord currecord =
                entities.customerrecord.FirstOrDefault(
                    p => p.custid == custopenid && p.clientid == customer.clientid && p.finishtime == null);
            if (currecord != null)
            {
                currecord.finishtime = DateTime.Now;
                currecord.finishtype = 1;
            }
            OnlineClient client = entities.OnlineClient.FirstOrDefault(p => p.OpenId == customer.clientid&&p.Status==(int)OnlineClientStatus.Connected);//删除在线客户记录
            if (client != null)
            {
                disconclientOpenid = client.OpenId;
                entities.OnlineClient.Remove(client);
                entities.SaveChanges();
            }
            
            //登记客服与接下来的客户的连线记录
            OnlineClient nextclient = entities.OnlineClient.FirstOrDefault(p => p.ConnectUserName == customer.accountname);
            if (nextclient != null)//有客户在等待
            {
                nextclient.Status = (int)OnlineClientStatus.Connected; //接入
                customerrecord record = new customerrecord()
                {
                    custid = custopenid,
                    clientid = nextclient.OpenId,
                    starttime = DateTime.Now
                };
                entities.customerrecord.Add(record);

                customer.clientid = nextclient.OpenId;
                customer.connecttime = DateTime.Now;
                entities.SaveChanges();

                msg = answerWord.CustNewclient;
                conclientOpenid = nextclient.OpenId;
                return true;
            }
            else//没有客户在等待
            {
                customer.clientid = null;//把客服状态设为有空
                customer.connecttime = null;
                entities.SaveChanges();
                msg = answerWord.CustNotWaitingCustomer;
                return false;
            }
        }

        /// <summary>
        /// 断开与客服的连接
        /// </summary>
        /// <param name="strCustomerName"></param>
        /// <returns></returns>
        public bool DisConCustomer(string strCustomerName)
        {
            var customer = entities.Customer.FirstOrDefault(p => p.accountname == strCustomerName);
            if (customer != null)
            {
                customer.clientid = null; //把客服状态设为有空
                customer.connecttime = null;
                return entities.SaveChanges()>0;
            }
            return false;
        }

        /// <summary>
        /// 根据客服openid判断是否和客户连线中
        /// </summary>
        /// <param name="custopenid">客服openid</param>
        /// <param name="clientopenid">连线的客户openid</param>
        /// <returns></returns>
        public bool IsConnection(string custopenid, out string clientopenid)
        {
            Customer customer = entities.Customer.FirstOrDefault
                (p => p.custid == custopenid && p.status == 1 && p.logintime != null && p.clientid != null && p.connecttime != null);//&& EntityFunctions.DiffHours(p.logintime,DateTime.Now)<24
            if (customer != null)
            {
                clientopenid = customer.clientid;
                return true;
            }
            clientopenid = "";
            return false;
        }

        /// <summary>
        /// 客服是否已经登录
        /// </summary>
        /// <param name="strCustOpenid"></param>
        /// <returns></returns>
        public bool IsCustomerLogin(string strCustOpenid)
        {
            Customer customer = entities.Customer.FirstOrDefault(p => p.custid == strCustOpenid && p.status==1 && p.logintime != null);
            return customer!=null;
        }

        /// <summary>
        /// 接入客服
        /// </summary>
        /// <param name="clientopenid">客户openid</param>
        /// <param name="cusopenid">客服openid</param>
        /// <returns></returns>
        public bool ConnectToCustomer(string clientopenid,string cusopenid)
        {
            Customer customer = entities.Customer.FirstOrDefault(p => p.custid == cusopenid);//接入的客服
            if (customer != null)
            {
                CustomerConnectRecord(cusopenid, clientopenid);

                customer.clientid = clientopenid;//更改客服接入的客户
                customer.connecttime = DateTime.Now;

                OnlineClient client = new OnlineClient()//客户状态进入接入客服状态
                {
                    OpenId = clientopenid,
                    OnlineTime = DateTime.Now,
                    Status = (int)OnlineClientStatus.Connected,
                    ConnectUserName = customer.accountname
                };
                entities.OnlineClient.Add(client);
                return entities.SaveChanges() > 0;
            }
            return false;
        }

        /// <summary>
        /// 登记客服接入客户记录
        /// </summary>
        /// <param name="cusopenid">客服openid</param>
        /// <param name="clientopenid">客户openid</param>
        private void CustomerConnectRecord(string cusopenid, string clientopenid)
        {
            customerrecord record = new customerrecord() //客服接入记录表
            {
                custid = cusopenid,
                clientid = clientopenid,
                starttime = DateTime.Now
            };
            entities.customerrecord.Add(record);
            //entities.SaveChanges();
        }

        /// <summary>
        /// 客服是否在线
        /// </summary>
        /// <param name="strusername"></param>
        /// <returns></returns>
        public Customer CustomerOnline(string strusername)
        {
            var customer = entities.Customer.FirstOrDefault(p => p.accountname == strusername && EntityFunctions.DiffHours(p.logintime, DateTime.Now) < 24&&p.status==1);
            return customer;
        }

        /// <summary>
        /// 客服是否有空闲
        /// </summary>
        /// <returns></returns>
        public bool IsCustomerAvailable(string strusername)
        {
            var customer = entities.Customer.FirstOrDefault(p => p.accountname == strusername && p.status == 1 && p.clientid == null && p.connecttime==null);
            return customer != null;
        }

        public string GetCustUserName(string openid)
        {
            var customer = entities.Customer.FirstOrDefault(p => p.custid == openid);
            return customer!=null ? customer.accountname : "";
        }

        public string GetCustOpenId(string username)
        {
            var customer = entities.Customer.FirstOrDefault(p => p.accountname == username);
            return customer != null ? customer.custid : "";
        }

        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="fixedAnswer"></param>
        public void AddModel(Customer Customers)
        {
            entities.Customer.Add(Customers);
            entities.SaveChanges();
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <returns></returns>
        public List<Customer> GetPageList(int pageIndex, int pageSize, ref int totalCount)
        {
            var PageView = (from p in entities.Customer
                            orderby p.id descending
                            select p).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            totalCount = entities.Customer.Count();
            return PageView.ToList();
        }
        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public Customer GetModel(int id)
        {
            return entities.Customer.Find(id);
        }
        /// <summary>
        /// 获取多个值
        /// </summary>
        /// <returns>数值ID</returns>
        public List<Customer> GetList(int[] id)
        {
            return entities.Customer.Where(d => id.Contains(d.id)).ToList();
        }
        public List<Customer> GetList()
        {
            return entities.Customer.ToList();
        }

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="fixedAnswer"></param>
        public void Remove(Customer Customers)
        {
            entities.Customer.Remove(Customers);
            entities.SaveChanges();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="fixedAnswer"></param>
        public void Update(Customer Customers)
        {
            var newModel = GetModel(Customers.id);
            newModel.accountname = Customers.accountname;
            newModel.clientid = Customers.clientid;
            newModel.connecttime = Customers.connecttime;
            newModel.custid = Customers.custid;
            newModel.logid = Customers.logid;
            newModel.logintime = Customers.logintime;
            newModel.status = Customers.status;
            entities.SaveChanges();
        }
    }
}