using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPI_POC1.OrderModel
{
    public class PriceComponents
    {
        public double sellingPrice { get; set; }
        public double customerPrice { get; set; }
        public double shippingCharge { get; set; }
        public double totalPrice { get; set; }
    }

    public class OrderItem
    {
        public long orderItemId { get; set; }
        public string orderId { get; set; }
        public string status { get; set; }
        public bool hold { get; set; }
        public string orderDate { get; set; }
        public string dispatchByDate { get; set; }
        public string updatedAt { get; set; }
        public int sla { get; set; }
        public int quantity { get; set; }
        public string title { get; set; }
        public string listingId { get; set; }
        public string fsn { get; set; }
        public string sku { get; set; }
        public double price { get; set; }
        public double shippingFee { get; set; }
        public PriceComponents priceComponents { get; set; }
        public List<string> stateDocuments { get; set; }
        public List<object> subItems { get; set; }
    }

    public class RootObject
    {
        public bool hasMore { get; set; }
        public List<OrderItem> orderItems { get; set; }
    }

}

