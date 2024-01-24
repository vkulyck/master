using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using GmWeb.Logic.Data.Context;
using GmWeb.Web.Common.Utility;
using GmWeb.Web.Common.Utility.Extensions;
using GmWeb.Web.Identity.Models;
using GmWeb.Web.Identity.Models.ProviderConfiguration;
using GmWeb.Web.Common.Identity;
using GmWeb.Common;
using System.Configuration;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Utility.Email;
using GmWeb.Logic.Utility.Redis;
using GmWeb.Web.Common;
using GmWeb.Web.Common.Controllers;
using AccountAPI = GmWeb.Web.Common.Identity.IdentitySettings;
using LegacyUser = GmWeb.Logic.Data.Models.Profile.BaseAccount;

namespace GmWeb.Web.Identity.Controllers
{
    using BaseProvider = TotpSecurityStampBasedTokenProvider<GmIdentity, string>;
    public class AccountController : AccountController<GmIdentity> { }

    [Authorize]
    public abstract partial class AccountController<TUser> : BaseController<TUser>
        where TUser : GmIdentity, new()
    {
        public string DefaultUsername => this.ClassConfig("AutoLoginUser");
        public string DefaultPassword => this.ClassConfig("AutoLoginPassword");
        protected virtual AppIdentityType IdentityType => this.RouteVariable<AppIdentityType>("accountType");
        protected virtual bool RequireRegistrationPassword => true;
        protected string AgencyInfoUriPath => ConfigurationManager.AppSettings["GmWeb.Web.Legacy.AgencyInfo.UriPath"];
        protected ITokenCache TokenCache => OwinContext.Get<ITokenCache>();
        protected ITwoFactorManager<TUser> TwoFactorManager => OwinContext.Get<ITwoFactorManager<TUser>>();
        protected TwoFactorViewModelFactory<GmIdentity> ViewModelFactory => OwinContext.Get<TwoFactorViewModelFactory<GmIdentity>>();
        protected override GmManager<TUser> UserManager => OwinContext.Get<GmManager<TUser>>().WithAppIdentityType(this.IdentityType);
        protected EmailClient EmailClient => OwinContext.Get<EmailClient>();

#if SECURITY_BYPASS
        protected string GeneratedParameter => "invoke-generator";
#endif

        protected virtual async Task<ActionResult> SignInAsync(LoginViewModel model, string returnUrl)
        {
            this.CurrentUser = null;
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectOrDefault(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("TwoFactorSelection", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        protected virtual async Task<ActionResult> TwoFactorSignInAsync(TwoFactorVerificationViewModel model)
        {
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectOrDefault(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        #region Login
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Login(string ReturnUrl, bool? Automate)
        {
            if (this.IsAuthenticated)
            {
                return RedirectOrDefault(ReturnUrl);
            }
#if AUTOLOGIN
            Automate ??= true;
#else
            Automate ??= false;
#endif
            ViewBag.ReturnUrl = ReturnUrl;
            var model = new LoginViewModel();
#if SECURITY_BYPASS
            model = new LoginViewModel { UserName = DefaultUsername, Password = DefaultPassword, IdentityType = this.IdentityType };
            if (Automate.Value)
            {
                return await this.SignInAsync(model, ReturnUrl);
            }
#endif
            return await ViewAsync(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
                else
                {
                    user.AuthenticationDate = DateTime.Now;
                    return await SignInAsync(model, ReturnUrl);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        [ActionAliases("Logoff")]
        public ActionResult Logout(string ReturnUrl = null)
        {
            this.CurrentUser = null;
            this.HttpContext.RemoveCookies(Cookies.SwitcherLogonData);
            System.Web.Security.FormsAuthentication.SignOut();
            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            if (string.IsNullOrEmpty(ReturnUrl))
                return View();
            return Redirect(ReturnUrl);
        }

#endregion

#region Registration
        protected RedirectToRouteResult RegForbidden() => this.RedirectToAction("ForbiddenRegistration", "AuthError");
        protected RedirectToRouteResult RegUnauthorized() => this.RedirectToAction("UnauthorizedRegistration", "AuthError");

        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Allows administrators to request an account registration token for a provided email address.
        /// </summary>
        public ActionResult RequestRegistration()
        {
            if (!AccountAPI.EnableRegistration)
                return this.RegForbidden();
#if SECURITY_BYPASS
            var email = GmWeb.Logic.Utility.Datagen.GetEmail();
            var model = new RequestTokenViewModel { Email = email };
            return View(model);
#else
            var model = new RequestTokenViewModel();
            return View(model);
#endif
        }
        /// <summary>
        /// Generates a registration token for the specified email address and sends it to the user with further instructions.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RequestRegistration(RequestTokenViewModel model)
        {
            if(!AccountAPI.EnableRegistration)
                return this.RegForbidden();
            string token = await TokenCache.GenerateRegistrationTokenAsync(model.Email);
            string registerUrl = Url.Action("Register", this.ControllerName, new { Token = token }, Request.Url.Scheme);
            await this.EmailClient.SendRegistrationTokenAsync(model.Email, token, registerUrl);
            model.IsEmailSent = true;
#if SECURITY_BYPASS
            return Redirect(registerUrl);
#else
            return View(model);
#endif
        }

        [HttpGet]
        public virtual async Task<ActionResult> MigrateLegacyUser()
        {
            if (!AccountAPI.EnableLegacyMigration)
                return this.RegForbidden();
            ViewBag.RequireRegistrationPassword = this.RequireRegistrationPassword;
            var model = new MigrateViewModel 
            { 
                IdentityType = this.IdentityType
            };
            return await this.ViewAsync(model);
        }

        [HttpPost]
        public virtual async Task<ActionResult> MigrateLegacyUser(MigrateViewModel model)
        {
            if (!AccountAPI.EnableLegacyMigration)
                return this.RegForbidden();
            if (!ModelState.IsValid)
                return View(model);
            var accounts = this.OwinContext.Get<AccountCache>();
            LegacyUser legacyUser = null;
            if (model.IdentityType == AppIdentityType.User)
                legacyUser = accounts.UserAccounts.SingleOrDefault(x => x.Email == model.Email);
            else if(model.IdentityType == AppIdentityType.Client)
                legacyUser = accounts.ClientAccounts.SingleOrDefault(x => x.Email == model.Email);
            if(legacyUser == null)
            {
                this.ModelState.AddModelError("Email", "Email address does not exist in legacy account tables.");
                return await this.ViewAsync(model);
            }
            var migUser = this.Mapper.Map<TUser>(legacyUser);
            var regResult = await this.UserManager.CreateAsync(migUser);
            if (regResult.Succeeded)
            {
                var dbUser = await this.UserManager.FindByEmailAsync(migUser.Email);
                await this.RefreshSignInAsync(dbUser);
                return this.RedirectToDefault();
            }
            else
            {
                AddErrors(regResult);
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Register(string Token, string ReturnUrl = null)
        {
            if (!AccountAPI.EnableRegistration)
                return this.RegForbidden();
            if (string.IsNullOrWhiteSpace(Token))
                return this.RegUnauthorized();
            ViewBag.RequireRegistrationPassword = this.RequireRegistrationPassword;
            var model = new RegisterViewModel 
            { 
                ReturnUrl = ReturnUrl, 
                IdentityType = this.IdentityType,
                Token = Token
            };
#if SECURITY_BYPASS
            if (model.Email == GeneratedParameter)
                model.Email = GmWeb.Logic.Utility.Datagen.GetEmail();
            if (model.Token == GeneratedParameter)
                model.Token = await this.RegistrationCache.GenerateRegistrationTokenAsync(model.Email);
#endif
            ViewBag.RequireRegistrationPassword = this.RequireRegistrationPassword;
            var regData = await this.TokenCache.GetRegistrationDataAsync<UserRegistrationData>(model.Token);
            var isTokenValid = regData != null;
            if (isTokenValid)
                model.Email = regData.Email;
            else
                this.ModelState.AddModelError("Token", $"Invalid registration token provided.");
            return View(model);
        }

        /// <summary>
        /// Submits the registration form data to the UserManager to complete the registration process.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!AccountAPI.EnableRegistration)
                return this.RegForbidden();
            ViewBag.RequireRegistrationPassword = this.RequireRegistrationPassword;
            if (!ModelState.IsValid)
                return View(model);
            var result = await this.TokenCache.VerifyRegistrationTokenAsync(model.Token);
            if (result.Succeeded)
            {
                var regUser = this.Mapper.Map<TUser>(new GmIdentity { Email = model.Email, UserName = model.Email });
                var regResult = await this.UserManager.CreateAsync(regUser, model.Password);
                if (regResult.Succeeded)
                {
                    var dbUser = await this.UserManager.FindByNameAsync(regUser.Email);
                    // TODO: confirm email
                    //await this.SignInAsync(model, false);
                    return this.RedirectOrDefault(model.ReturnUrl);
                }
                else
                {
                    AddErrors(regResult);
                }
            }
            else
            {
                this.ModelState.AddModelError("Token", "Registration token could not be verified.");
            }
            return View(model);
        }

#endregion

#region Password
        /// <summary>
        /// Allows visitors to submit their email address and request a password reset.
        /// </summary>
        public ActionResult RequestPasswordReset()
        {
            var model = new RequestPasswordResetViewModel
            {
                IsEmailSent = false
            };
            return View(model);
        }

        /// <summary>
        /// Generates a password reset request for the specified email address.
        /// Accepts the email address posted via the RequestPasswordResetViewModel.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> RequestPasswordReset(RequestPasswordResetViewModel model)
        {
            // Gets the user entity for the specified email address
            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "Email does not match an existing user.");
            }
            else
            {
                // Generates a password reset token for the user
                string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                if(string.IsNullOrWhiteSpace(token))
                {
                    ModelState.AddModelError("Token", "Error generating reset token.");
                }
                else
                {
                    // Prepares the URL of the password reset link (targets the "ManagePasswordReset" action)
                    // Fill in the name of your controller
                    string resetUrl = Url.Action("ManagePasswordReset", this.ControllerName, new { model.Email, Token = token }, Request.Url.Scheme);

                    // Creates and sends the password reset email to the user's address
                    try
                    {
                        await this.EmailClient.SendPasswordResetTokenAsync(user.Email, token, resetUrl);
                        ViewBag.StatusMessage = $"Email sent to {model.Email}";
                    }
                    catch
                    {
                        // TODO: Log exception
                        ModelState.AddModelError("Email", "Error sending reset token.");
                    }
                }
            }

            // Displays a view asking the visitor to check their email and click the password reset link
            return View(model);
        }

        #region Phone Management

        [HttpGet]
        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> ManagePhoneUpdate()
        {
            ViewBag.Title = $"Phone Update Form";
            ViewBag.Description = $"Update your phone here.";
            var model = new PhoneConfigurationViewModel { ManagementTask = ManagementTask.Update, InformationType = InformationType.Phone };
            return await this.ViewAsync(model);
        }

        [HttpPost]
        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> ManagePhoneUpdate(PhoneConfigurationViewModel model)
        {
            ViewBag.Title = $"Phone Update Form";
            ViewBag.Description = $"Update your phone here.";
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var token = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Phone);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Phone,
                    Body = "Your security code is: " + token
                };
                await UserManager.SmsService.SendAsync(message);

                // Uncomment to debug locally 
                // TempData["ViewBagCode"] = message.Body.ToString();
            }
            return RedirectToAction("ManagePhoneVerify", new { Phone = model.Phone });
        }

        [HttpGet]
        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> ManagePhoneVerify(string phone)
        {
            ViewBag.Title = $"Phone Verification Form";
            ViewBag.Description = $"Verify your phone number.";
            var model = new PhoneConfigurationViewModel
            {
                ManagementTask = ManagementTask.Verify,
                InformationType = InformationType.Phone,
                Phone = phone
            };
            if (string.IsNullOrWhiteSpace(phone))
                return await this.ErrorViewAsync(model, "No phone number provided for verification.");
            return await ViewAsync(model);
        }

        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> ManagePhoneVerify(PhoneConfigurationViewModel model)
        {
            ViewBag.Title = $"Phone Verification Form";
            ViewBag.Description = $"Verify your phone number.";
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.Phone, model.Token);
            if (result.Succeeded)
            {
                return RedirectToAction("TwoFactorDetails", new { Message = ManageMessageId.UpdatePhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        #endregion

        #region Email Management
        [HttpGet]
        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> ManageEmailUpdate()
        {
            ViewBag.Title = $"Email Update Form";
            ViewBag.Description = $"Update your email here.";
            var model = new EmailConfigurationViewModel { ManagementTask = ManagementTask.Update, InformationType = InformationType.Email };
            return await this.ViewAsync(model);
        }

        [HttpPost]
        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> ManageEmailUpdate(EmailConfigurationViewModel model)
        {
            ViewBag.Title = $"Email Update Form";
            ViewBag.Description = $"Update your email here.";
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var token = await TokenCache.GenerateRegistrationTokenAsync(model.Email);

            if (UserManager.EmailService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Email,
                    Body = "Your security code is: " + token
                };
                await UserManager.EmailService.SendAsync(message);

                // Uncomment to debug locally 
                // TempData["ViewBagCode"] = message.Body.ToString();
            }
            return RedirectToAction("ManageEmailVerify", new { Email = model.Email });
        }

        [HttpGet]
        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> ManageEmailVerify(string email, string token)
        {
            ViewBag.Title = $"Email Verification Form";
            ViewBag.Description = $"Verify your email address.";
            var model = new EmailConfigurationViewModel
            {
                ManagementTask = ManagementTask.Verify,
                InformationType = InformationType.Email,
                Email = email,
                Token = token
            };
            if (string.IsNullOrWhiteSpace(email))
                return await this.ErrorViewAsync(model, "No email address provided for verification.");
            return await ViewAsync(model);
        }

        [HttpPost]
        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> ManageEmailVerify(EmailConfigurationViewModel model)
        {
            ViewBag.Title = $"Email Verification Form";
            ViewBag.Description = $"Verify your email address.";
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await TokenCache.VerifyRegistrationTokenAsync(model.Token);
            if(result.Email != model.Email)
            {
                ModelState.AddModelError("", "Provided email does not match original update request.");
            }
            else if (result.Succeeded)
            {
                this.CurrentUser.Email = result.Email;
                await UserManager.UpdateAsync(this.CurrentUser);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Token verified, but email address was not updated.");
                }
                else
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user.Email != model.Email)
                    {
                        ModelState.AddModelError("", "Token verified, but email address was not updated.");
                    }
                    else
                    {
                        if (user != null)
                        {
                            await this.RefreshSignInAsync(user, isPersistent: false);
                        }
                        return RedirectToAction("Index", new { Message = ManageMessageId.UpdateEmailSuccess });
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify email");
            return View(model);
        }
        #endregion

        #region Two-Factor OTP Authentication

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> TwoFactorSelection(string ReturnUrl, bool RememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
#if AUTOLOGIN
            return RedirectToAction("TwoFactorVerification", new
            {
                Provider = "DevAutoLogin",
                ReturnUrl = ReturnUrl,
                RememberMe = RememberMe
            });
#endif
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var providers = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new TwoFactorSelectionViewModel { Providers = providers, ReturnUrl = ReturnUrl, RememberMe = RememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> TwoFactorSelection(TwoFactorSelectionViewModel model)
        {
            var errs = this.ModelState.SelectMany(x => x.Value.Errors).ToList();
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("TwoFactorVerification", new
            {
                Provider = model.SelectedProvider,
                ReturnUrl = model.ReturnUrl,
                RememberMe = model.RememberMe
            });
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> TwoFactorVerification(string Provider, string ReturnUrl, bool RememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var model = new TwoFactorVerificationViewModel { Provider = Provider, ReturnUrl = ReturnUrl, RememberMe = RememberMe };
#if SECURITY_BYPASS
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            model.Code = await UserManager.GenerateTwoFactorTokenAsync(userId, Provider);
#endif
#if AUTOLOGIN
            return await this.TwoFactorSignInAsync(model);
#else
            return View(model);
#endif
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> TwoFactorVerification(TwoFactorVerificationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            return await this.TwoFactorSignInAsync(model);
        }

        [HttpGet]
        public async Task<ActionResult> TwoFactorDetails()
        {
            var model = this.Mapper.Map<TwoFactorDetailsViewModel>(this.CurrentUser);
            ViewBag.Title = $"Account Security Details";
            ViewBag.Description = $"{model.InformationType.GetDisplayName()} {model.ManagementTask.ToString().ToLower()} was {model.TaskResult}.";
            model.ProviderConfiguration = await this.TwoFactorManager.GetProviderConfigurationAsync(this.CurrentUser);
            return this.View("TwoFactorDetails", model);
        }

        [HttpGet]
        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> TwoFactorConfiguration(ManagementTask ManagementTask, InformationType InformationType, string ReturnUrl = null)
        {
            var model = ViewModelFactory.CreateModel(this.CurrentUser, InformationType);
            model.ManagementTask = ManagementTask;
            model.InformationType = InformationType;
            model.ReturnUrl = ReturnUrl;
            if(string.IsNullOrWhiteSpace(model.ReturnUrl))
                model.ReturnUrl = this.Url.Action("TwoFactorDetails");
            if (model.ManagementTask == ManagementTask.Update)
            {
                // If we're enabling phone/email verification (which involves 3rd-party communication) 
                // then we need to confirm that the token was delivered successfully.
                if (InformationType.IsUserProperty())
                {
                    model.ManagementTask = ManagementTask.Verify;
                    model.SecretKey = this.CurrentUser.GetInformation(model.InformationType);
                    var token = await UserManager.GenerateTwoFactorTokenAsync(this.CurrentUser.Id, model.SelectedProvider);
                    var result = await this.UserManager.NotifyTwoFactorTokenAsync(this.CurrentUser.Id, model.SelectedProvider, token);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Error sending 2FA token.");
                    }
                }
                else if(InformationType.EmploysSharedTokenGenerators())
                {
                    model.SecretKey = await this.TwoFactorManager.ConfigureTwoFactorSecret(this.CurrentUser.Id, model.SelectedProvider, InformationType);
                }
            }
#if SECURITY_BYPASS
            model.Token = await UserManager.GenerateTwoFactorTokenAsync(this.CurrentUser.Id, model.SelectedProvider);
#endif
            return await this.ViewAsync(model);
        }

        [HttpPost]
        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> TwoFactorConfiguration(TwoFactorConfigurationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            
            if (model.ManagementTask == ManagementTask.Disable)
            {
                await this.UserManager.DisableUserTfaAsync(this.CurrentUser, model.InformationType);
                await this.UserManager.DeleteUserTfaSecretAsync(this.CurrentUser, model.InformationType);
                return RedirectOrDefault(model.ReturnUrl);
            }
            else if (string.IsNullOrWhiteSpace(model.Token))
            {
                ModelState.AddModelError("", "No 2FA token provided.");
            }
            else if(model.ManagementTask == ManagementTask.Update)
            {
                // TODO: this is getting confusing so refactor please
                if (!string.IsNullOrWhiteSpace(model.SecretKey))
                    await this.UserManager.SetUserTfaSecretAsync(this.CurrentUser, model.InformationType, model.SecretKey);
                var result = await UserManager.VerifyTwoFactorTokenAsync(this.CurrentUser.Id, model.SelectedProvider, model.Token);
                if (result)
                {
                    this.CurrentUser.AuthenticationDate = DateTime.Now;
                    await this.UserManager.EnableUserTfaAsync(this.CurrentUser, model.InformationType);
                    return RedirectOrDefault(model.ReturnUrl);
                }
            }
            var message = $"{model.InformationType.GetDisplayName()} token could not be validated; please try again.";
            return await this.ErrorViewAsync(model, message);
        }
        #endregion


        [HttpGet]
        [AllowAnonymous]
        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> ManagePasswordReset()
        {
            ViewBag.Title = $"Password Reset Form";
            ViewBag.Description = $"Reset your password here.";
            var model = new TwoFactorConfigurationViewModel { ManagementTask = ManagementTask.Reset, InformationType = InformationType.Password };
            return await this.ViewAsync(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ExplicitView("TwoFactorConfiguration")]
        public async Task<ActionResult> ManagePasswordReset(TwoFactorConfigurationViewModel model)
        {
            ViewBag.Title = $"Password Reset Form";
            ViewBag.Description = $"Reset your password here.";
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Email is not set.");
            }
            else if (string.IsNullOrEmpty(model.Token))
            {
                ModelState.AddModelError("Token", "Token is not set.");
            }
            else if (this.ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "Email does not match an existing user.");
                }
                else if (this.ModelState.IsValid)
                {
                    var result = UserManager.ResetPassword(user.Id, model.Token, model.NewPassword);
                    AddErrors(result);
                    if (ModelState.IsValid)
                        ViewBag.StatusMessage = "Password reset successfully.";
                }
            }
            else
            {
                this.ModelState.Clear();
                return await this.ErrorViewAsync(model, "Error resetting password.");
            }
            return View(model);
        }

        [HttpGet]
        [ExplicitView("PasswordConfiguration")]
        public async Task<ActionResult> ManagePasswordUpdate()
        {
            ViewBag.Title = $"Password Update Form";
            ViewBag.Description = $"Update your password here.";
            var model = new PasswordConfigurationViewModel { ManagementTask = ManagementTask.Update };
            return await this.ViewAsync(model);
        }

        [HttpPost]
        public async Task<ActionResult> ManagePasswordUpdate(PasswordConfigurationViewModel model)
        {
            ViewBag.Title = $"Password Update Form";
            ViewBag.Description = $"Update your password here.";
            if (string.IsNullOrEmpty(model.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "New password is not set.");
            }
            else if (string.IsNullOrEmpty(model.ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPassword", "Confirmation password is not set.");
            }
            else if (model.ConfirmPassword != model.NewPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Confirmation password does not match.");
            }
            if (string.IsNullOrEmpty(model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Current password is not set.");
            }
            else if (this.ModelState.IsValid)
            {
                IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("TwoFactorDetails", new { Message = ManageMessageId.UpdatePasswordSuccess });
                }
                else
                {
                    AddErrors(result);
                }
            }
            return View("PasswordConfiguration", model);
        }
#endregion

        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByIdAsync(this.User.Identity.GetUserId());
            var model = new IndexViewModel
            {
                IsLoggedIn = this.User.Identity.IsAuthenticated,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber,
            };
            return View("Index", model);
        }

#region Phone
        //
        // POST: /Account/EnableTwoFactorAuthentication
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await RefreshSignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Account/DisableTwoFactorAuthentication
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await RefreshSignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Account/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await RefreshSignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

#endregion
    }
}
