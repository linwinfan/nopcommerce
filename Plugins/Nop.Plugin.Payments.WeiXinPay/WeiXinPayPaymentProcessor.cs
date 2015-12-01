using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.WeiXinPay.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Framework;
using Nop.Plugin.Payments.WeiXinPay.API;

namespace Nop.Plugin.Payments.WeiXinPay
{
    /// <summary>
    /// WeiXinPay payment processor
    /// </summary>
    public class WeiXinPayPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly WeiXinPayPaymentSettings _weixinPayPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public WeiXinPayPaymentProcessor(WeiXinPayPaymentSettings weixinPayPaymentSettings,
            ISettingService settingService, IWebHelper webHelper,
            IStoreContext storeContext)
        {
            this._weixinPayPaymentSettings = weixinPayPaymentSettings;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._storeContext = storeContext;
        }

        #endregion
        

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //查询微信支付是否完成
            WxPayApi.OrderQuery()
            string service = "create_direct_pay_by_user";

            string seller_email = _weixinPayPaymentSettings.SellerEmail;
            string sign_type = "MD5";
            string key = _weixinPayPaymentSettings.Key;
            string partner = _weixinPayPaymentSettings.Partner;
            string input_charset = "utf-8";

            string show_url = _webHelper.GetStoreLocation(false);

            string out_trade_no = postProcessPaymentRequest.Order.OrderGuid.ToString();
            string subject = string.IsNullOrEmpty(_weixinPayPaymentSettings.InvoiceSubject) ? _storeContext.CurrentStore.Name : _weixinPayPaymentSettings.InvoiceSubject;
            //string body = _weixinPayPaymentSettings.InvoiceBody;
            string body = "";
            foreach (var item in postProcessPaymentRequest.Order.OrderItems)
            {
                if (item.Product.Name.Length > 100)
                    body += item.Product.Name.Substring(0, 100) + "...,";
                else
                    body += item.Product.Name + ",";

            }
            body = body.Substring(0, body.Length - 1);
            if (body.Length > 999)
            { 
                body = body.Substring(0, 996) + "...";
            }

            string total_fee = postProcessPaymentRequest.Order.OrderTotal.ToString("0.00", CultureInfo.InvariantCulture);

            string notify_url = _webHelper.GetStoreLocation(false) + "Plugins/PaymentWeiXinPay/Notify";
            string return_url = _webHelper.GetStoreLocation(false) + "Plugins/PaymentWeiXinPay/Return";

            var paymethod = "directPay"; //bankPay:银联支付； directPay:支付宝即时到账
            var defaultBank = "";

            var customInfo = postProcessPaymentRequest.Order.DeserializeCustomValues();
            if (customInfo != null && customInfo.Count > 0)
            {
                if (customInfo.ContainsKey(WeiXinPayPaymentSystemNames.SelectedAlipayPayMethod))
                {
                    paymethod = customInfo[WeiXinPayPaymentSystemNames.SelectedAlipayPayMethod].ToString();
                    if (!string.IsNullOrEmpty(paymethod) && paymethod != WeiXinPayPaymentSystemNames.DirectPay)
                    {
                        //默认网银
                        defaultBank = paymethod;//如果传递的paymethod不等于directPay,则表示使用银联支付，paymethod表示银行编码

                        //默认支付方式
                        paymethod = WeiXinPayPaymentSystemNames.BankPay;
                    }
                }
            }

            IList<string> paraList = new List<string>();
            paraList.Add("service=" + service);
            paraList.Add("partner=" + partner);
            paraList.Add("seller_email=" + seller_email);
            paraList.Add("out_trade_no=" + out_trade_no);
            paraList.Add("subject=" + subject);
            paraList.Add("body=" + body);
            paraList.Add("total_fee=" + total_fee);
            paraList.Add("show_url=" + show_url);
            paraList.Add("return_url=" + return_url);
            paraList.Add("notify_url=" + notify_url);
            paraList.Add("payment_type=1");
            paraList.Add("paymethod=" + paymethod);
            if (!string.IsNullOrEmpty(defaultBank))
            {
                paraList.Add("defaultbank=" + defaultBank);
            }
            paraList.Add("_input_charset=" + input_charset);

            string aliay_url = CreatUrl(
                paraList.ToArray(),
                input_charset,
                key
                );
            var post = new RemotePost();
            post.FormName = "weixinpaysubmit";
            post.Url = "https://mapi.weixinpay.com/gateway.do?_input_charset=utf-8";
            post.Method = "POST";

            post.Add("service", service);
            post.Add("partner", partner);
            post.Add("seller_email", seller_email);
            post.Add("out_trade_no", out_trade_no);
            post.Add("subject", subject);
            post.Add("body", body);
            post.Add("total_fee", total_fee);
            post.Add("show_url", show_url);
            post.Add("return_url", return_url);
            post.Add("notify_url", notify_url);
            post.Add("payment_type", "1");
            post.Add("paymethod", paymethod);
            if (!string.IsNullOrEmpty(defaultBank))
            {
                post.Add("defaultbank", defaultBank);
            }
            post.Add("sign", aliay_url);
            post.Add("sign_type", sign_type);

            post.Post();
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return _weixinPayPaymentSettings.AdditionalFee;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //WeiXinPay is the redirection payment method
            //It also validates whether order is also paid (after redirection) so customers will not be able to pay twice
            
            //payment status should be Pending
            if (order.PaymentStatus != PaymentStatus.Pending)
                return false;

            //let's ensure that at least 1 minute passed after order is placed
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes < 1)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentWeiXinPay";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.WeiXinPay.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentWeiXinPay";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.WeiXinPay.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentWeiXinPayController);
        }

        public override void Install()
        {
            //settings
            var settings = new WeiXinPayPaymentSettings()
            {
                SellerEmail = "",
                Key = "",
                Partner= "",
                AdditionalFee = 0,
                InvoiceSubject = "Order Subject",
                //InvoiceBody = "对一笔交易的具体描述信息",
                EnableBankPay = true
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.RedirectionTip", "Select pay method, you will be redirected to WeiXinPay site to complete the order.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.SellerEmail", "Seller email");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.SellerEmail.Hint", "Enter seller email.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.Key", "Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.Key.Hint", "Enter key.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.Partner", "Partner");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.Partner.Hint", "Enter partner.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.ReturnUrl", "Return Url (Please leave it empty, if you didn't have custom page)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.ReturnUrl.Hint", "Return Url.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.InvoiceSubject", "Subject");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.InvoiceSubject.Hint", "Enter name of product.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.InvoiceBody", "Body");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.InvoiceBody.Hint", "Enter description of product.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.EnableBankPay", "Enable Bank Pay");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeiXinPay.EnableBankPay.Hint", "Enable bank pay.");
            this.AddOrUpdatePluginLocaleResource("Payment.PayMethod", "Pay Method");
            this.AddOrUpdatePluginLocaleResource("Payment.PayMethod.Hint", "Select pay method.");
            this.AddOrUpdatePluginLocaleResource("Payment.DirectPayMethod", "Alipay direct pay");
            this.AddOrUpdatePluginLocaleResource("Payment.DirectPayMethod.Hint", "Alipay direct pay.");
            this.AddOrUpdatePluginLocaleResource("Payment.BankPayMethod", "Alipay bank pay");
            this.AddOrUpdatePluginLocaleResource("Payment.BankPayMethod.Hint", "Alipay bank pay.");
            base.Install();
        }


        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.SellerEmail.RedirectionTip");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.SellerEmail");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.SellerEmail.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.Key");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.Key.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.Partner");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.Partner.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.ReturnUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.ReturnUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.InvoiceSubject");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.InvoiceSubject.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.InvoiceBody");
            this.DeletePluginLocaleResource("Plugins.Payments.WeiXinPay.InvoiceBody.Hint");

            base.Uninstall();
        }
        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        #endregion
    }
}
