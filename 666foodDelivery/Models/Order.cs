using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace _666foodDelivery.Models
{
    public class Order
    {

        public int ID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string CustomerName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Food Name")]
        public string FoodName { get; set; }
        [Required]
        [Display(Name = "Delivery Time")]
        public DateTime DeliverTime { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The order must be more than 1")]
        [Display(Name = "Number of Order")]
        public int quantity { get; set; }

        [Display(Name = "Notes")]
        public string notes { get; set; }




    }
}
