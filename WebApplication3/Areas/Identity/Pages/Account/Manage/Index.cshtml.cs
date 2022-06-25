// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using WebApplication3.Models;

namespace WebApplication3.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment HostingEnvironment;
        private readonly ApplicationDbContext db;

        public IndexModel(
            IWebHostEnvironment HostingEnvironment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext _db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            db = _db;
            this.HostingEnvironment = HostingEnvironment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }
        public string url { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone")]
            public string PhoneNumber { get; set; }

            [Display(Name = "FirstName")]
            public string FirstName { get; set; }
            [Display(Name = "Lastname")]
            public string Lastname { get; set; }

            [Display(Name = "Street")]
            public string Street { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }
            
            [Display(Name = "Province")]
            public string Province { get; set; }
            [Display(Name = "Image")]
            public IFormFile img { get; set; }
        }
        

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var street = user.Street;
            var city = user.City;
            var province = user.Province;
            var firstname = user.Firstname;
            var lastname = user.Lastname;
            Username = userName;
            url = user.image == null ? "defaultimg.png" : user.image;
            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Street = street,
                FirstName = firstname,
                Lastname = lastname,
                City = city,
                Province = province,
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
            if (Input.FirstName != user.Firstname)
            {
                user.Firstname = Input.FirstName;
            }
            if (Input.Lastname != user.Lastname)
            {
                user.Lastname = Input.Lastname;
            }
            if (Input.Street != user.Street)
            {
                user.Street = Input.Street;
            }
            if (Input.City != user.City)
            {
                user.City = Input.City;
            }
            if (Input.Province != user.Province)
            {
                user.Province = Input.Province;
            }

            
        

            
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
