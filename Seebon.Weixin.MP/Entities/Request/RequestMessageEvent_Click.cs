using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seebon.Weixin.MP.Entities
{
    /// <summary>
    /// 事件
    /// </summary>
    public class RequestMessageEvent_Click : RequestMessageEventBase, IRequestMessageEventBase
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public override Event Event
        {
            get { return Event.CLICK; }
        }
    }
}
