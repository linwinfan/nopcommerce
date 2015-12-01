using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.WeiXinPay
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //Notify
            routes.MapRoute("Plugin.Payments.WeiXinPay.Notify",
                 "Plugins/PaymentWeiXinPay/Notify",
                 new { controller = "PaymentWeiXinPay", action = "Notify" },
                 new[] { "Nop.Plugin.Payments.WeiXinPay.Controllers" }
            );

            //Notify
            routes.MapRoute("Plugin.Payments.WeiXinPay.Return",
                 "Plugins/PaymentWeiXinPay/Return",
                 new { controller = "PaymentWeiXinPay", action = "Return" },
                 new[] { "Nop.Plugin.Payments.WeiXinPay.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.WeiXinPay.MakeQRCode",
                 "Plugins/PaymentWeiXinPay/MakeQRCode",
                 new { controller = "PaymentWeiXinPay", action = "MakeQRCode" },
                 new[] { "Nop.Plugin.Payments.WeiXinPay.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
