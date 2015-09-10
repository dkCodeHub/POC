using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPI_POC1.Models
{
    public abstract class BaseInfo
    {
        public string BaseURL { get; set; }
        public Tuple<string, string> EnvCredentials { get; set; }

    }

    public class AUTH : BaseInfo
    {
        public string Resource { get; set; }


    }



    public class OrderAPIS
    {
        public string searchURL { get; set; }
    }
}
