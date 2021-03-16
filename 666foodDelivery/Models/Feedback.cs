using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _666foodDelivery.Models
{
    public class Feedback : TableEntity
    {
        public Feedback() { }

        public Feedback(string CustomerFeedback, string Feedback)
        {
            this.PartitionKey = CustomerFeedback;
            this.RowKey = Feedback;
        }

        public string CustomerName { get; set; }

        public string FeedbackCategory { get; set; }
    }
}
