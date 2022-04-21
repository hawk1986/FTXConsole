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
            result = new Market();
        }

        public bool success { get; set; }
        public Market result { get; set; }
        
    }

    public class Market
    {
        public string name { get; set; }
        public bool enabled { get; set; }
        public bool postOnly { get; set; }
        public double? priceIncrement { get; set; }
        public double? sizeIncrement { get; set; }
        public double? minProvideSize { get; set; }
		public double? last { get; set; }
		public double? bid { get; set; }
		public double? ask { get; set; }
		public double? price { get; set; }
		public string type { get; set; }
		public string baseCurrency { get; set; }
		public string quoteCurrency { get; set; }
		public string underlying { get; set; }
		public bool restricted { get; set; }
		public bool highLeverageFeeExempt { get; set; }
		public double? largeOrderThreshold { get; set; }
		public double? change1h { get; set; }
		public double? change24h { get; set; }
		public double? changeBod { get; set; }
		public double? quoteVolume24h { get; set; }
		public double? volumeUsd24h { get; set; }
        public double? priceHigh24h { get; set; }
        public double? priceLow24h { get; set; }
    }
}
