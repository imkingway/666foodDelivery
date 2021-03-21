using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendProcessingJob
{
    [Serializable]
    class Order
    {
        public int ID { get; set; }

        public string CustomerName { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string FoodName { get; set; }

        public DateTime DeliverTime { get; set; }

        public int Quantity { get; set; }
    }
}
