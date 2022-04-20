using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtxApi.Model
{
    public class BalanceResult
    {
        public BalanceResult()
        {
            result = new List<Balance>();
        }

        public bool success { get; set; }
        public List<Balance> result { get; set; }
        
    }

    public class Balance
    {
        public string coin { get; set; }
        public decimal? total { get; set; }
        public decimal? free { get; set; }
        public decimal? availableWithoutBorrow { get; set; }
        public decimal? usdValue { get; set; }
        public decimal? spotBorrow { get; set; }
    }
}
