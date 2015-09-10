using MPI_POC1.Models;
using Newtonsoft.Json;
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

        /// <summary>
        /// GET ALL ORDERS
        /// </summary>
        /// <param name="strEnvironment"></param>
        /// <param name="strToken"></param>
        /// <param name="HasMore"></param>
        /// <returns></returns>
        public IRestResponse GetAllOrderDetails(string strEnvironment, string strToken, string HasMore)
        {
            DateTime currdate = DateTime.Now;
            string fromdate = string.Format("{0:yyyy-MM-dd}", currdate.AddDays(-40));
            string todate = string.Format("{0:yyyy-MM-dd}", currdate);
            string json = "{\"filter\":{ \"orderDate\": {\"fromDate\": \"" + fromdate + "\",\"toDate\": \"" + todate + "\"}}}";
            RestClient _clnt = new RestClient();
            _clnt.BaseUrl = new Uri(GetBaseURL(strEnvironment));
            var request = new RestSharp.RestRequest("sellers/orders/search", RestSharp.Method.POST) { RequestFormat = RestSharp.DataFormat.Json }
            .AddParameter("application/json", json, ParameterType.RequestBody);

            request.AddHeader("Authorization", "Bearer " + Convert.ToString(strToken));
            var _Response = _clnt.Execute(request);
            string strValue = _Response.Content;
            int idx = strValue.IndexOf("url", 1);
            string json_result = strValue.Substring(0, (idx - 2)) + "}";
            MPI_POC1.OrderModel.RootObject obj = JsonConvert.DeserializeObject<MPI_POC1.OrderModel.RootObject>(json_result);
            List<ShippmentModel.RootObject> _ShippmentList = new List<ShippmentModel.RootObject>();
            string strList = string.Empty;
            /*CALL TO SHIPPMENT DETAILS*/

            foreach (var Order in obj.orderItems)
            {
                strList = strList + Order.orderItemId + ",";
            }
            ShippmentModel.RootObject strShippmentDetails = JsonConvert.DeserializeObject<ShippmentModel.RootObject>(GetShippmentDetails(strList.TrimEnd(',').ToString(), strEnvironment, strToken));

            /*CALL TO Get by OrderItemId API */
            /*
            List<MPI_POC1.OrderItemDetailsModels.RootObject> _OrderItemDetailsList = new List<OrderItemDetailsModels.RootObject>();
            foreach (var Order in obj.orderItems)
            {
                MPI_POC1.OrderItemDetailsModels.RootObject objOrderItemDetails = JsonConvert.DeserializeObject<MPI_POC1.OrderItemDetailsModels.RootObject>(GetOrderByItemId(Order.orderItemId, strEnvironment, strToken));
                _OrderItemDetailsList.Add(objOrderItemDetails);
            }
            */

            return _Response;

        }

        /// <summary>
        /// GET SHIPPMENT DETAILS
        /// </summary>
        /// <param name="OrderID"></param>
        /// <param name="strEnvironment"></param>
        /// <param name="strToken"></param>
        /// <returns></returns>
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


        private string GetOrderByItemId(long OrderItemID, string strEnvironment, string strToken)
        {

            RestClient _clnt = new RestClient();
            _clnt.BaseUrl = new Uri(GetBaseURL(strEnvironment));
            var request = new RestSharp.RestRequest("sellers/orders/{orderItemId}", RestSharp.Method.GET) { RequestFormat = RestSharp.DataFormat.Json }
                .AddParameter("orderItemId", OrderItemID, ParameterType.UrlSegment);
            request.AddHeader("Authorization", "Bearer " + Convert.ToString(strToken));

            var _Response = _clnt.Execute(request);
            return _Response.Content;
        }


    }
}
