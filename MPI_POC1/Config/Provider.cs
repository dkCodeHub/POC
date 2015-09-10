using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PROCVIEW.Model;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MPI_POC1.Config
{

    public class Provider
    {
        private string GetBaseURL(string strEnvironment)
        {

            string strURL = string.Empty;

            switch (strEnvironment.ToUpper())
            {
                case "SANDBOX":
                    strURL = ConfigurationManager.AppSettings["SANDBOXBASEURL"].ToString();
                    break;
                case "LIVE":
                    strURL = ConfigurationManager.AppSettings["LIVEBOXBASEURL"].ToString();
                    break;
            }
            return strURL;
        }

        #region OAUTH PROPERTIES & METHODS
        private Tuple<string, string> GetCredntials(string strEnvironment)
        {
            Tuple<string, string> _ReturnTup = null;
            switch (strEnvironment.ToUpper())
            {
                case "SANDBOX":
                    _ReturnTup = Tuple.Create(ConfigurationManager.AppSettings["SANDBOXAPPID"].ToString(), ConfigurationManager.AppSettings["SANDBOXSECRET"].ToString());
                    break;
                case "LIVE":
                    _ReturnTup = Tuple.Create(ConfigurationManager.AppSettings["LIVEBOXAPPID"].ToString(), ConfigurationManager.AppSettings["LIVEBOXSECRET"].ToString());
                    break;
            }
            return _ReturnTup;
        }


        private RestClient OAUTHClient(string strEnvironment)
        {
            RestClient _clnt = new RestClient();
            _clnt.BaseUrl = new Uri(GetBaseURL(strEnvironment));
            dynamic d = GetCredntials(strEnvironment);
            _clnt.Authenticator = new HttpBasicAuthenticator(d.Item1, d.Item2);
            return _clnt;
        }

        private RestRequest OAUTHRequest(string strEnvironment)
        {
            RestRequest _Req = new RestRequest();
            _Req.Resource = ConfigurationManager.AppSettings["OAUTHRESOURCE"].ToString();
            _Req.Method = Method.GET;
            _Req.AddParameter("grant_type", "client_credentials");
            _Req.AddParameter("scope", "Seller_Api");
            return _Req;
        }

        #endregion

        /// <summary>
        /// FOR AUTHENTICATION TOKEN
        /// </summary>
        /// <param name="strEnvironment"></param>
        /// <returns></returns>
        public IRestResponse ExecuteAuth(string strEnvironment)
        {
            var client = OAUTHClient(strEnvironment);
            var request = OAUTHRequest(strEnvironment);
            return client.Execute(request);

        }


        public List<OrderCSV> GetOrderDetails(string strEnvironment, string strAccessToken, out bool HasMore)
        {
            DateTime currdate = DateTime.Now;
            string fromdate = string.Format("{0:yyyy-MM-dd}", currdate.AddDays(-40));
            string todate = string.Format("{0:yyyy-MM-dd}", currdate);
            string json = "{\"filter\":{ \"orderDate\": {\"fromDate\": \"" + fromdate + "\",\"toDate\": \"" + todate + "\"}}}";

            RestClient _clnt = new RestClient();
            _clnt.BaseUrl = new Uri(GetBaseURL(strEnvironment));

            var request = new RestSharp.RestRequest("sellers/orders/search", RestSharp.Method.POST) { RequestFormat = RestSharp.DataFormat.Json }
            .AddParameter("application/json", json, ParameterType.RequestBody);
            request.AddHeader("Authorization", "Bearer " + Convert.ToString(strAccessToken));

            var _Response = _clnt.Execute(request);
            string strValue = _Response.Content;

            int idx = strValue.IndexOf("url", 1);
            string json_result = strValue.Substring(0, (idx - 2)) + "}";

            MPI_POC1.OrderModel.RootObject obj = JsonConvert.DeserializeObject<MPI_POC1.OrderModel.RootObject>(json_result);

            List<PROCVIEW.Model.OrderCSV> _LstOrdrCsv = new List<PROCVIEW.Model.OrderCSV>();
            foreach (var Order in obj.orderItems)
            {
                _LstOrdrCsv.Add(new PROCVIEW.Model.OrderCSV()
                {
                    OrderNumer = Order.orderItemId,
                    PurchaseNumber = Order.orderId,
                    PurchaseStatus = Order.status,
                    confirmedAt = Order.orderDate,
                    ShippmentCreatedate = Order.dispatchByDate,
                    ProductTitle = Order.title,
                    Sku = Order.sku,
                    Qty = Order.quantity,
                    MRP = Order.price,
                    ShippingCharge = Order.priceComponents.shippingCharge,
                    ItemTotal = Order.priceComponents.totalPrice
                });
            }

            string strList = string.Empty;
            foreach (var OrderInfo in _LstOrdrCsv) { strList = strList + OrderInfo.OrderNumer + ","; }
            ShippmentModel.RootObject strShippmentDetails = JsonConvert.DeserializeObject<ShippmentModel.RootObject>(GetShippmentDetails(strList.TrimEnd(',').ToString(), strEnvironment, strAccessToken));


            var objFinal = from u in strShippmentDetails.shipments
                           join d in _LstOrdrCsv
                           on u.orderId equals d.PurchaseNumber
                           select new { Update = u, Details = d };

            foreach (var ShippmentDetails in objFinal)
            {
                ShippmentDetails.Details.ShippingAddress1 = ShippmentDetails.Update.deliveryAddress.addressLine1;
                ShippmentDetails.Details.ShippingAddress2 = ShippmentDetails.Update.deliveryAddress.addressLine2;
                ShippmentDetails.Details.ShippingPincode = ShippmentDetails.Update.deliveryAddress.pincode;
                ShippmentDetails.Details.ShippingCity = ShippmentDetails.Update.deliveryAddress.city;
                ShippmentDetails.Details.ShippingState = ShippmentDetails.Update.deliveryAddress.state;
                ShippmentDetails.Details.BillingAddress1 = ShippmentDetails.Update.billingAddress.addressLine1;
                ShippmentDetails.Details.BillingAddress2 = ShippmentDetails.Update.billingAddress.addressLine2;
                ShippmentDetails.Details.BillingPincode = ShippmentDetails.Update.billingAddress.pincode;
                ShippmentDetails.Details.BillingCity = ShippmentDetails.Update.billingAddress.city;
                ShippmentDetails.Details.BillingState = ShippmentDetails.Update.billingAddress.state;
                ShippmentDetails.Details.ShippmentNumber = ShippmentDetails.Update.shipmentId;
                JObject deliveryObject = (Newtonsoft.Json.Linq.JObject)ShippmentDetails.Update.courierDetails.deliveryDetails;
                ShippmentDetails.Details.TrackingCode = deliveryObject == null ? "" : deliveryObject["trackingId"].ToString();
                ShippmentDetails.Details.ShippingAgentCode = deliveryObject == null ? "" : deliveryObject["vendorName"].ToString();

                ShippmentDetails.Details.Purchaseinstructions = "Not Available";
                ShippmentDetails.Details.GuestCheckout = "Not Available";
                ShippmentDetails.Details.CustomerEmail = "Not Available";
                ShippmentDetails.Details.CustomerPhone = "Not Available";
                ShippmentDetails.Details.ShippmentStatus = "Not Available";
                ShippmentDetails.Details.stockNotes = "Not Available";
                ShippmentDetails.Details.PromotionCodes = "Not Available";
                ShippmentDetails.Details.GiftWrap = "Not Available";
                ShippmentDetails.Details.GiftMessage = "Not Available";
                ShippmentDetails.Details.PromotionAmt = "Not Available";
                ShippmentDetails.Details.PaymentMethod = "Not Available";
                ShippmentDetails.Details.PgTransactionId = "Not Available";
                ShippmentDetails.Details.PaymentTransactionId = "Not Available";


                ShippmentDetails.Details.SiteGroup = "7001";
                ShippmentDetails.Details.CustomerGroup = "NON-FA";
                ShippmentDetails.Details.InvoiceDate = "Not Available";

            }
            HasMore = obj.hasMore;
            return _LstOrdrCsv;
        }

        private string GetShippmentDetails(string OrderID, string strEnvironment, string strToken)
        {
            RestClient _clnt = new RestClient();
            var listOfOrderItemIDS = new List<string> { OrderID.ToString() };
            _clnt.BaseUrl = new Uri(GetBaseURL(strEnvironment));

            var request = new RestSharp.RestRequest("sellers/orders/shipments", RestSharp.Method.GET) { RequestFormat = RestSharp.DataFormat.Json }
                .AddParameter("orderItemIds", OrderID, "application/json", ParameterType.QueryString);
            request.AddHeader("Authorization", "Bearer " + Convert.ToString(strToken));
            var _Response = _clnt.Execute(request);
            return _Response.Content;
        }



    }
}
