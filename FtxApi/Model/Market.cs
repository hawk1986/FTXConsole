using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtxApi.Model
{
    public class MarketResult
    {
        public MarketResult()
        {
            result = new List<Market>();
        }

        public bool success { get; set; }
        public List<Market> result { get; set; }
        
    }

    public class Market
    {
        public string coin { get; set; }
        public decimal total { get; set; }
        public decimal free { get; set; }
        public decimal availableWithoutBorrow { get; set; }
        public decimal usdValue { get; set; }
        public decimal spotBorrow { get; set; }
    }
}
