using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPI_POC1.ShippmentModel
{
    public class OrderItem
    {
        public string id { get; set; }
        public bool fragile { get; set; }
        public bool large { get; set; }
        public bool dangerous { get; set; }
    }

    public class DeliveryAddress
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string addressLine1 { get; set; }
        public object addressLine2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public string stateCode { get; set; }
        public string stateName { get; set; }
        public object landmark { get; set; }
    }

    public class BillingAddress
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string addressLine1 { get; set; }
        public object addressLine2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public string stateCode { get; set; }
        public string stateName { get; set; }
        public object landmark { get; set; }
    }

    public class BuyerDetails
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
    }

    public class SellerAddress
    {
        public object firstName { get; set; }
        public object lastName { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public object stateCode { get; set; }
        public object stateName { get; set; }
        public object landmark { get; set; }
    }

    public class CourierDetails
    {
        public object pickupDetails { get; set; }
        public object deliveryDetails { get; set; }
    }

    public class Shipment
    {
        public string orderId { get; set; }
        public object shipmentId { get; set; }
        public List<OrderItem> orderItems { get; set; }
        public bool weighingRequired { get; set; }
        public DeliveryAddress deliveryAddress { get; set; }
        public BillingAddress billingAddress { get; set; }
        public BuyerDetails buyerDetails { get; set; }
        public SellerAddress sellerAddress { get; set; }
        public object returnAddress { get; set; }
        public CourierDetails courierDetails { get; set; }
    }

    public class RootObject
    {
        public List<Shipment> shipments { get; set; }
    }
}
