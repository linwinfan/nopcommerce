using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.WeiXinPay
{
    public class WeiXinPayPaymentSettings : ISettings
    {
        /// <summary>
        /// 第三方用户唯一凭证ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 第三方用户唯一凭证密钥，即appsecret
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// 商户支付密钥Key。登录微信商户后台，进入栏目【账户设置】【密码安全】【API 安全】【API 密钥】
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 支付完成后的回调处理页面
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 证书路径，仅在退款撤消订单时需要
        /// </summary>
        public string SSLCertPath { get; set; }

        /// <summary>
        /// 证书密码，仅在退款撤消订单时需要
        /// </summary>
        public string SSLCertPassword { get; set; }

        /// <summary>
        /// 商户系统后台机器IP
        /// </summary>
        public string IP { get; set; }

        //=======【代理服务器设置】===================================
        /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
        */
        public static string PROXY_URL = "http://10.152.18.220:8080";

        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        public static int REPORT_LEVENL = 0;

    }
    
}
