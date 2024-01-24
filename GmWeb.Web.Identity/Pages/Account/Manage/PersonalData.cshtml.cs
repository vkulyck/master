﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GmWeb.Web.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModelBase
    {
        private readonly ApiUserManager _userManager;
        private readonly ILogger<PersonalDataModel> _logger;

        public PersonalDataModel(
            ApiUserManager userManager,
            ILogger<PersonalDataModel> logger)
        {
            this._userManager = userManager;
            this._logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
            }

            return this.Page();
        }
    }
}