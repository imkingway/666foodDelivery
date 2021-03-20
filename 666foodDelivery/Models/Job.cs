using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _666foodDelivery.Models
{
    public class Job
    {

        [Required]
        [Display(Name = "Driver ID")]
        [Key]
        public string DriverID { get; set; }

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
        public string DeliverTime { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The order must be more than 1")]
        [Display(Name = "Number of Order")]
        public string quantity { get; set; }
    }
}
