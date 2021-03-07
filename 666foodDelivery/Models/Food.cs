using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _666foodDelivery.Models
{
    public class Food
    {
        public int ID { get; set; }

        [Display(Name = "Food Name")]
        public string FoodName { get; set; }

        [Display(Name = "Food Added Date")]
        public DateTime FoodProducedDate { get; set; }

        public string Type { get; set; }

        public decimal Price { get; set; }

        public string Ingredient { get; set; }

        public string BlobURL { get; set; }
    }
}
