using System;
using System.Globalization;
using System.Linq;
using Seebon.Weixin.MP.Entities;
using System.Collections.Generic;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.CommonService
{
    public class FixedAnswerService
    {
        seebonweixinEntities entities = new seebonweixinEntities();

        /// <summary>
        /// 根据接收到的文本信息查找对应的回复信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="bolCustomer"></param>
        /// <returns></returns>
        public ResponseMessageBase GetResponseMessage(RequestMessageText requestMessage ,bool bolCustomer=false)
        {
            var fixedAnswer = entities.FixedAnswer.ToList();
            var selFixedAnswer = new FixedAnswer();
            var bolFlag = false;
            string strkey = requestMessage.Content;
            foreach (var fixedAnswerItem in fixedAnswer.Where(fixedAnswerItem => strkey == fixedAnswerItem.key))
            {
                selFixedAnswer = fixedAnswerItem;
                bolFlag = true;
                break;
            }

            ResponseMessageBase responseMessage = null;
            if (bolFlag)//存在对应的回复
            {
                switch (selFixedAnswer.MsgType.ToLower())
                {
                    case "text":
                        responseMessage =
                            ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
                        ((ResponseMessageText) responseMessage).Content = selFixedAnswer.content.Replace("\\n",
                            ((char) 10).ToString(CultureInfo.InvariantCulture));
                        break;
                    case "music":
                        responseMessage =
                            ResponseMessageBase.CreateFromRequestMessage<ResponseMessageMusic>(requestMessage);
                        ((ResponseMessageMusic) responseMessage).Music.MusicUrl = selFixedAnswer.MusicUrl;
                        break;
                    case "news":
                        responseMessage =
                            ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);
                        ((ResponseMessageNews) responseMessage).Articles.Add(new Article
                        {
                            Title = selFixedAnswer.Title,
                            PicUrl = selFixedAnswer.PicUrl,
                            Description =
                                selFixedAnswer.Description.Replace("\\n",
                                    ((char) 10).ToString(CultureInfo.InvariantCulture)),
                            Url = selFixedAnswer.Url
                        });
                        break;
                    case "concustomer":
                        if(!bolCustomer)
                        {
                            responseMessage =
                                ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
                            ((ResponseMessageText) responseMessage).Content = selFixedAnswer.content.Replace("\\n",
                                ((char) 10).ToString(CultureInfo.InvariantCulture));
                            //等待接入在线服务人员
                            var clientServicewait = new ClientService();
                            clientServicewait.WaitingForCustomer(requestMessage.FromUserName, selFixedAnswer.AccessUser);
                        }
                        break;
                    case "exit":
                        responseMessage =
                            ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
                        //离开接入在线服务人员
                        var clientServiceexit = new ClientService();
                        if (clientServiceexit.ExitWaiting(requestMessage.FromUserName))//退出服务
                        {
                            ((ResponseMessageText)responseMessage).Content = selFixedAnswer.content.Replace("\\n", ((char)10).ToString(CultureInfo.InvariantCulture)).Split(new[]{';'})[0];
                        }
                        else//没有在服务中
                        {
                            ((ResponseMessageText)responseMessage).Content = selFixedAnswer.content.Replace("\\n", ((char)10).ToString(CultureInfo.InvariantCulture)).Split(new[]{';'})[1];
                        }
                        break;
                }
            }
            else
            {
                responseMessage =
                            ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
                var defaultAnswer = entities.FixedAnswer.FirstOrDefault(p => p.key == "default");
                ((ResponseMessageText) responseMessage).Content = defaultAnswer != null ? defaultAnswer.content : Common.Common.ConvertoWordWrap("很抱歉，您的问题我暂时无法解决。请回复序号获取在线服务：\n[1]在线销售\n[2]在线客服\n\n-回复\"dh\"获取导航菜单");
            }
            return responseMessage;
        }

        public ResponseMessageBase GetResponseMessage(IRequestMessageBase requestMessage, string strCommand)
        {
            var fixedAnswer = entities.FixedAnswer.ToList();
            var selFixedAnswer = new Model.FixedAnswer();
            var bolFlag = false;
            foreach (var fixedAnswerItem in fixedAnswer.Where(fixedAnswerItem => strCommand == fixedAnswerItem.key))//查找有没有对应的回复
            {
                selFixedAnswer = fixedAnswerItem;
                bolFlag = true;
                break;
            }

            ResponseMessageBase responseMessage = null;
            if (bolFlag)//有对应回复则返回回复内容
            {
                switch (selFixedAnswer.MsgType.ToLower())
                {
                    case "text":
                        responseMessage =
                            ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
                        ((ResponseMessageText)responseMessage).Content = selFixedAnswer.content.Replace("\\n", ((char)10).ToString(CultureInfo.InvariantCulture));
                        break;
                    case "music":
                        responseMessage =
                            ResponseMessageBase.CreateFromRequestMessage<ResponseMessageMusic>(requestMessage);
                        ((ResponseMessageMusic)responseMessage).Music.MusicUrl = selFixedAnswer.MusicUrl;
                        break;
                    case "news":
                        responseMessage =
                            ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);
                        ((ResponseMessageNews)responseMessage).Articles.Add(new Article
                        {
                            Title = selFixedAnswer.Title,
                            PicUrl = selFixedAnswer.PicUrl,
                            Description = selFixedAnswer.Description.Replace("\\n", ((char)10).ToString(CultureInfo.InvariantCulture)),
                            Url = selFixedAnswer.Url
                        });
                        break;

                }
            }
            else
            {
                responseMessage =
                            ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
                var defaultAnswer = entities.FixedAnswer.FirstOrDefault(p => p.key == "default");
                ((ResponseMessageText)responseMessage).Content = defaultAnswer != null ? defaultAnswer.content : "很抱歉，您的问题我暂时无法解决。如果是销售问题您可以联系我们的在线销售，客服问题请联系我们的在线客服，谢谢！";
            }
            return responseMessage;

        }
        
        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="fixedAnswer"></param>
        public void AddModel(FixedAnswer fixedAnswer)
        {
            entities.FixedAnswer.Add(fixedAnswer);
            entities.SaveChanges();
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <returns></returns>
        public List<FixedAnswer> GetPageList(int pageIndex, int pageSize, ref int totalCount)
        {
            var PageView = (from p in entities.FixedAnswer
                             orderby p.id descending
                             select p).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            totalCount = entities.FixedAnswer.Count();
            return PageView.ToList();
        }
        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public FixedAnswer GetModel(int id)
        {
            return entities.FixedAnswer.Find(id);
        }
        /// <summary>
        /// 获取多个值
        /// </summary>
        /// <returns>数值ID</returns>
        public List<FixedAnswer> GetList(int[] id)
        {
            return entities.FixedAnswer.Where(d => id.Contains(d.id)).ToList();
        }
        public List<FixedAnswer> GetList()
        {
            return entities.FixedAnswer.ToList();
        }

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="fixedAnswer"></param>
        public void Remove(FixedAnswer fixedAnswer)
        {
            entities.FixedAnswer.Remove(fixedAnswer);
            entities.SaveChanges();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="fixedAnswer"></param>
        public void Update(FixedAnswer fixedAnswer)
        {
            var newModel = GetModel(fixedAnswer.id);
            newModel.content = fixedAnswer.content;
            newModel.Description = fixedAnswer.Description;
            newModel.HQMusicUrl = fixedAnswer.HQMusicUrl;
            newModel.key = fixedAnswer.key;
            newModel.MsgType = fixedAnswer.MsgType;
            newModel.MusicUrl = fixedAnswer.MusicUrl;
            newModel.PicUrl = fixedAnswer.PicUrl;
            newModel.Title = fixedAnswer.Title;
            newModel.Url = fixedAnswer.Url;
            entities.SaveChanges();
        }

        /// <summary>
        /// 返回一个指定key值的回复
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public FixedAnswer GetModel(string key)
        {
            return entities.FixedAnswer.FirstOrDefault(p => p.key == key);
        }

        /// <summary>
        /// 返回指定的客服命令的对象
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public FixedAnswer GetCustomerModel(string keyWord)
        {
            var fixedAnswer =
                entities.FixedAnswer.FirstOrDefault(fixedansweritem => fixedansweritem.key == keyWord && fixedansweritem.MsgType == "concustomer");
            return fixedAnswer;
        }
    }


}