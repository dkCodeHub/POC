using MPI_POC1.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPI_POC1
{
    class Program
    {


        {
            List<OrderCSV> _OrderCSVDetails = new List<OrderCSV>();
            OrderProcess _Process = new OrderProcess();

            string strAccessToken = _Process.GetAUTHToken("LIVE");
            if (!(String.IsNullOrEmpty(strAccessToken)))
            {
                bool HasMore = false;
                do
                {
                    object details = _Process.GetOrderDetails("LIVE", strAccessToken, out HasMore);
                } while (HasMore != false);
            }
            Console.Read();
        }
    }
}
