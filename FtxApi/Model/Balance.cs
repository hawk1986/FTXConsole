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
        public double? total { get; set; }
        public double? free { get; set; }
        public double? availableWithoutBorrow { get; set; }
        public double? usdValue { get; set; }
        public double? spotBorrow { get; set; }
    }
}
