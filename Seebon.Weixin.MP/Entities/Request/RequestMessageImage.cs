using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seebon.Weixin.MP.Entities
{
    public class RequestMessageImage : RequestMessageBase, IRequestMessageBase
    {
        public override RequestMsgType MsgType
        {
            get { return RequestMsgType.Image; }
        }
        public string PicUrl { get; set; }
        public string MediaId { get; set; }
    }
}
