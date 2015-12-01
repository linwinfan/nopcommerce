using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.WeiXinPay.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Payments.WeiXinPay.API;
using Nop.Core.Domain.Orders;
using System.Linq;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Nop.Plugin.Payments.WeiXinPay.Controllers
{
    public class PaymentWeiXinPayController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly WeiXinPayPaymentSettings _weixinPayPaymentSettings;
        private readonly PaymentSettings _paymentSettings;

        public PaymentWeiXinPayController(ISettingService settingService, 
            IPaymentService paymentService, IOrderService orderService, 
            IOrderProcessingService orderProcessingService, 
            ILogger logger, IWebHelper webHelper,
            WeiXinPayPaymentSettings weixinPayPaymentSettings,
            PaymentSettings paymentSettings,
            IWorkContext workContext,
            IStoreContext storeContext)
        {
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._logger = logger;
            this._webHelper = webHelper;
            this._weixinPayPaymentSettings = weixinPayPaymentSettings;
            this._paymentSettings = paymentSettings;
            this._workContext = workContext;
            this._storeContext = storeContext;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.AppId = _weixinPayPaymentSettings.AppId;
            model.Key = _weixinPayPaymentSettings.Key;
            model.AppSecret = _weixinPayPaymentSettings.AppSecret;
            model.MchId = _weixinPayPaymentSettings.MchId;
            model.ReturnUrl = _weixinPayPaymentSettings.ReturnUrl;
            model.SSLCertPath = _weixinPayPaymentSettings.SSLCertPath;
            model.SSLCertPassword = _weixinPayPaymentSettings.SSLCertPassword;
            model.IP = _weixinPayPaymentSettings.IP;

            return View("~/Plugins/Payments.WeiXinPay/Views/PaymentWeiXinPay/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _weixinPayPaymentSettings.AppId = model.AppId;
            _weixinPayPaymentSettings.AppSecret = model.AppSecret;
            _weixinPayPaymentSettings.MchId = model.MchId;
            _weixinPayPaymentSettings.Key = model.Key;
            _weixinPayPaymentSettings.ReturnUrl = model.ReturnUrl;
            _weixinPayPaymentSettings.SSLCertPath = model.SSLCertPath;
            _weixinPayPaymentSettings.SSLCertPassword = model.SSLCertPassword;
            _weixinPayPaymentSettings.IP = model.IP;

            _settingService.SaveSetting(_weixinPayPaymentSettings);
            
            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();
            
            WxPayData data = new WxPayData(_weixinPayPaymentSettings.Key);
            Order order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
            if (order != null)
            {
                data.SetValue("body", _storeContext.CurrentStore.Name);//商品描述
                data.SetValue("attach", "weixincodepay");//附加数据
                data.SetValue("out_trade_no", order.Id);//随机字符串
                data.SetValue("total_fee", order.OrderTotal);//总金额
                data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
                data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
                data.SetValue("goods_tag", order.OrderItems.FirstOrDefault().Product.Name);//商品标记
                data.SetValue("trade_type", "NATIVE");//交易类型
                data.SetValue("product_id", order.OrderItems.FirstOrDefault().Product.Id);//商品ID

                WxPayData result = WxPayApi.UnifiedOrder(_weixinPayPaymentSettings.AppId, _weixinPayPaymentSettings.MchId, _weixinPayPaymentSettings.Key,_weixinPayPaymentSettings.IP,WeiXinPayPaymentSettings.PROXY_URL, data);//调用统一下单接口
                model.URL = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接
                
                return View("~/Plugins/Payments.WeiXinPay/Views/PaymentWeiXinPay/PaymentInfo.cshtml", model);
            }
            throw new Exception("未找到待支持的订单");
        }

        public ActionResult MakeQRCode(string data)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeScale = 4;

            //将字符串生成二维码图片
            Bitmap image = qrCodeEncoder.Encode(data, Encoding.Default);

            //保存为PNG到内存流  
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);
            return File(ms.GetBuffer(), "image/jpeg");
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            
            return paymentInfo;
        }

        [ValidateInput(false)]
        public ActionResult Notify(FormCollection form)
        {
            throw new NotFiniteNumberException();
        }

        [ValidateInput(false)]
        public ActionResult Return()
        {

            throw new NotFiniteNumberException();

        }
    }
}