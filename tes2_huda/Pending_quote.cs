using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tes2_huda
{
    class Pending_quote
    {
        
        public class Pending_quote_data
        {
            public int cust_id { set; get; }
            public string cust_name { set; get; }
            public string sfrom { set; get; }
            public string sto { set; get; }
            public decimal amount { set; get; }
            public string paid { set; get; }
        }
    }
}
