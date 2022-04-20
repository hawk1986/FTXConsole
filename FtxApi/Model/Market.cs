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
        public decimal? priceIncrement { get; set; }
        public decimal? sizeIncrement { get; set; }
        public decimal? minProvideSize { get; set; }
		public decimal? last { get; set; }
		public decimal? bid { get; set; }
		public decimal? ask { get; set; }
		public decimal? price { get; set; }
		public string type { get; set; }
		public string baseCurrency { get; set; }
		public string quoteCurrency { get; set; }
		public string underlying { get; set; }
		public bool restricted { get; set; }
		public bool highLeverageFeeExempt { get; set; }
		public decimal? largeOrderThreshold { get; set; }
		public decimal? change1h { get; set; }
		public decimal? change24h { get; set; }
		public decimal? changeBod { get; set; }
		public decimal? quoteVolume24h { get; set; }
		public decimal? volumeUsd24h { get; set; }
        public decimal? priceHigh24h { get; set; }
        public decimal? priceLow24h { get; set; }
    }
}
