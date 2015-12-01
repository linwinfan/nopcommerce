using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.WeiXinPay.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        /// <summary>
        /// 第三方用户唯一凭证ID
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeiXinPay.AppId")]
        public string AppId { get; set; }

        /// <summary>
        /// 第三方用户唯一凭证密钥，即appsecret
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeiXinPay.AppSecret")]
        public string AppSecret { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeiXinPay.MchId")]
        public string MchId { get; set; }

        /// <summary>
        /// 商户支付密钥Key。登录微信商户后台，进入栏目【账户设置】【密码安全】【API 安全】【API 密钥】
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeiXinPay.Key")]
        public string Key { get; set; }

        /// <summary>
        /// 支付完成后的回调处理页面
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeiXinPay.ReturnUrl")]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 证书路径，仅在退款撤消订单时需要
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeiXinPay.SSLCertPath")]
        public string SSLCertPath { get; set; }

        /// <summary>
        /// 证书密码，仅在退款撤消订单时需要
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeiXinPay.SSLCertPassword")]
        public string SSLCertPassword { get; set; }

        /// <summary>
        /// 商户系统后台机器IP
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeiXinPay.IP")]
        public string IP { get; set; }

    }
}