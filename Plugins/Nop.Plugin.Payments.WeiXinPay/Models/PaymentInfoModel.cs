using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.WeiXinPay.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public string URL { get; set; }
    }
}