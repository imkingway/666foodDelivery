using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using _666foodDelivery.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _666foodDelivery.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<_666foodDeliveryUser> _userManager;
        private readonly SignInManager<_666foodDeliveryUser> _signInManager;

        public IndexModel(
            UserManager<_666foodDeliveryUser> userManager,
            SignInManager<_666foodDeliveryUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
           
            [DataType(DataType.Text)]
            [Display(Name = "Full Name")]
            [StringLength(100, ErrorMessage = "The number should be 6 - 100", MinimumLength = 6)]
            public string Name { get; set; }

            [Required(ErrorMessage = "Please key in your Date of Birth")]
            [DataType(DataType.Date)]
            [Display(Name = "Date-of-Birth")]
            public DateTime DOB { get; set; }

            [StringLength(100, ErrorMessage = "Address shout not be more than 100 words")]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [Display(Name = "Identification Number")]
            public string IC_Passport_No { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(_666foodDeliveryUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Name = user.Name,
                DOB = user.DOB,
                Address = user.Address,
                IC_Passport_No = user.IC_Passport_No
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.DOB != user.DOB)
            {
                user.DOB = Input.DOB;
            }

            if (Input.Address != user.Address)
            {
                user.Address = Input.Address;
            }

            if (Input.IC_Passport_No != user.IC_Passport_No)
            {
                user.IC_Passport_No = Input.IC_Passport_No;
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
