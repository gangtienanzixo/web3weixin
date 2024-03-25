using System.Linq;
using Seebon.Weixin.MP.Entities;
using Seebon.Weixin.MP.Helpers;

namespace Seebon.Weixin.MP.Service.CommonService
{
    public class TextService
    {
        public ResponseMessageText GetResponseMessage(RequestMessageText requestMessage)
        {
            var db = new Model.seebonweixinEntities();
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

            var fixedAnswer = db.FixedAnswer.ToList();
            bool bolFlag = true;
            foreach (var fixedAnswerItem in fixedAnswer)
            {
                if (requestMessage.Content == fixedAnswerItem.key)
                {
                    responseMessage.Content = fixedAnswerItem.content;
                    bolFlag = false;
                    break;
                }
            }
            if (bolFlag)
                responseMessage.Content = db.FixedAnswer.SingleOrDefault(p => p.key == "default").content;
            return responseMessage;
        }

        public string GetText()
        {
            var db = new Model.seebonweixinEntities();
            var answer = db.FixedAnswer.FirstOrDefault(p => p.key == "aa");
            if (answer != null) return answer.content;
            else return "不存在aa";
        }
    }
}