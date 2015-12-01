using System;
using System.Web.Mvc;
using Nop.Admin.Models.Common;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Admin.Models.Affiliates
{
    public partial class AffiliateModel : BaseNopEntityModel
    {
        public AffiliateModel()
        {
            Address = new AddressModel();
            this.AvailableLevels = new List<SelectListItem>();
            this.AvailableAffiliates = new List<SelectListItem>();
            this.AvailableLevels.Add(new SelectListItem { Text = "一级",Value = "1" });
            this.AvailableLevels.Add(new SelectListItem { Text = "二级", Value = "2" });
            this.AvailableLevels.Add(new SelectListItem { Text = "三级", Value = "3" });
        }

        [NopResourceDisplayName("Admin.Affiliates.Fields.ID")]
        public override int Id { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.Fields.URL")]
        public string Url { get; set; }


        [NopResourceDisplayName("Admin.Affiliates.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.Fields.FriendlyUrlName")]
        [AllowHtml]
        public string FriendlyUrlName { get; set; }
        
        [NopResourceDisplayName("Admin.Affiliates.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.Fields.Level")]
        public int Level { get; set; }

        public IList<SelectListItem> AvailableLevels { get; set; }

        public int? ParentAffiliateId { get; set; }

        public IList<SelectListItem> AvailableAffiliates { get; set; }

        public AddressModel Address { get; set; }

        #region Nested classes
        
        public partial class AffiliatedOrderModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Affiliates.Orders.Order")]
            public override int Id { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.OrderStatus")]
            public string OrderStatus { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.PaymentStatus")]
            public string PaymentStatus { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.ShippingStatus")]
            public string ShippingStatus { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.OrderTotal")]
            public string OrderTotal { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class AffiliatedCustomerModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Affiliates.Customers.Name")]
            public string Name { get; set; }
        }

        #endregion
    }
}