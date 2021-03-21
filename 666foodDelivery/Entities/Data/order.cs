using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _666foodDelivery.Entities.Data
{
    public class order
    {
        
        public string CustomerName { get; set; }

        
        public string Address { get; set; }

        
        public string PhoneNumber { get; set; }

        
        public string FoodName { get; set; }

        
        public DateTime DeliverTime { get; set; }

       
        public int Quantity { get; set; }
    }
}
