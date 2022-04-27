using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtxApi.Model
{
    public class HistoryPrice
    {
        public HistoryPrice()
        {
            result = new List<Price>();
        }

        public bool success { get; set; }
        public List<Price> result { get; set; }
    }

    public class Price
    {
        public string startTime { get; set; }
        public double time { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
		public double? volume { get; set; }
        public string color { get; set; }
    }
}
