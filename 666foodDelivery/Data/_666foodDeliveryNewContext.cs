using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using _666foodDelivery.Models;

namespace _666foodDelivery.Data
{
    public class _666foodDeliveryNewContext : DbContext
    {
        public _666foodDeliveryNewContext (DbContextOptions<_666foodDeliveryNewContext> options)
            : base(options)
        {
        }

        public DbSet<_666foodDelivery.Models.Food> Food { get; set; }
    }
}
