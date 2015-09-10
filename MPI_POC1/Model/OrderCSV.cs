using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROCVIEW.Model
{
    public class OrderCSV
    {
        public long OrderNumer { get; set; }
        public string PurchaseNumber { get; set; }
        public string PurchaseStatus { get; set; }
        public string Purchaseinstructions { get; set; }
        public string confirmedAt { get; set; }
        public string GuestCheckout { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingPincode { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingPincode { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string ShippmentNumber { get; set; }
        public string ShippmentCreatedate { get; set; }
        public string ShippmentStatus { get; set; }
        public string TrackingCode { get; set; }
        public string ShippingAgentCode { get; set; }
        public string ProductTitle { get; set; }
        public string stockNotes { get; set; }
        public string PromotionCodes { get; set; }
        public string GiftWrap { get; set; }
        public string GiftMessage { get; set; }
        public string Sku { get; set; }
        public int Qty { get; set; }
        public double MRP { get; set; }
        public string PromotionAmt { get; set; }
        public double ShippingCharge { get; set; }
        public double ItemTotal { get; set; }
        public string PaymentMethod { get; set; }
        public string PgTransactionId { get; set; }
        public string PaymentTransactionId { get; set; }
        public string SiteGroup { get; set; }
        public string CustomerGroup { get; set; }
        public string InvoiceDate { get; set; }

    }
}
