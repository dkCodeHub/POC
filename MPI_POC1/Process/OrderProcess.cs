using MPI_POC1.Config;
using MPI_POC1.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MPI_POC1.Process
{
    public class OrderProcess
    {
        public String GetAUTHToken(string strEnvironment)
        {
            string strToken = string.Empty;
            Provider _Provider = new Provider();
            Newtonsoft.Json.Linq.JObject jobject = Newtonsoft.Json.Linq.JObject.Parse(_Provider.ExecuteAuth(strEnvironment).Content);
            strToken = jobject["access_token"].ToString();
            return strToken;
        }


        //public string FetchOrderDetails(string strToken, string strEnvironment, string nextPage)
        //{

        //    Provider _Provider = new Provider();
        //    _Provider.GetOrderDetails(strEnvironment, strToken, nextPage);
        //    return string.Empty;
        //}

        internal List<OrderCSV> GetOrderDetails(string strEnvironment, string strAccessToken, out bool HasMore)
        {
            Provider _Provider = new Provider();
            return _Provider.GetOrderDetails(strEnvironment, strAccessToken, out HasMore);
            
        }
    }
}
