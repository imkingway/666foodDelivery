using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace _666foodDelivery.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the _666foodDeliveryUser class
    public class _666foodDeliveryUser : IdentityUser
    {

        [PersonalData]
        public string Name { get; set; }

        [PersonalData]
        public DateTime DOB {  get; set; }

        [PersonalData]
        public string Address { get; set; }

        [PersonalData]
        public string IC_Passport_No { get; set; }

        [PersonalData]
        public string User_Role { get; set; }
    }
}
