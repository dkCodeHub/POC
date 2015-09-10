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


        static void Main(string[] args)
        {
            OrderProcess _Process = new OrderProcess();
            string strAccessToken = _Process.GetAUTHToken("LIVE");
            if (strAccessToken != null && !(string.IsNullOrEmpty(strAccessToken)))
            {
                Console.Write("Authenticated.");
                string str = _Process.FetchOrderDetails(strAccessToken, "LIVE", "");
            }
            else {
                Console.Write("Not Authenticated.");
            }
            //

            Console.Read();
        }
    }
}
