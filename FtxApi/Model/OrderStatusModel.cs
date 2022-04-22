using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtxApi.Model
{
    public class OrderStatus
    {
        public OrderStatus()
        {
            result = new List<Status>();
        }

        public bool success { get; set; }
        public List<Status> result { get; set; }
    }

    public class Status
    {
        public string id { get; set; }
        public string clientId { get; set; }
        public string market { get; set; }
        public string type { get; set; }
        public string side { get; set; }
        public double? price { get; set; }
		public double? size { get; set; }
		public string status { get; set; }
		public double? filledSize { get; set; }
		public double? remainingSize { get; set; }
		public bool reduceOnly { get; set; }
		public string liquidation { get; set; }
		public double? avgFillPrice { get; set; }
		public bool postOnly { get; set; }
		public bool ioc { get; set; }
		public string createdAt { get; set; }
		public string future { get; set; }
    }
}
