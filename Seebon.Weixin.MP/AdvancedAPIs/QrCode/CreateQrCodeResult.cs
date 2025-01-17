﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seebon.Weixin.MP.Entities;

namespace Seebon.Weixin.MP.AdvancedAPIs
{
    /// <summary>
    /// 二维码创建返回结果
    /// </summary>
    public class CreateQrCodeResult : WxJsonResult
    {
        public string ticket { get; set; }
        public int expire_seconds { get; set; }
    }
}
