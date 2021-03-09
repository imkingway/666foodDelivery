using System;
using _666foodDelivery.Areas.Identity.Data;
using _666foodDelivery.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(_666foodDelivery.Areas.Identity.IdentityHostingStartup))]
namespace _666foodDelivery.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<_666foodDeliveryContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("_666foodDeliveryContextConnection")));

                services.AddDefaultIdentity<_666foodDeliveryUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<_666foodDeliveryContext>();
            });
        }
    }
}