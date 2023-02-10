// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
 
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ElseForty.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Microsoft.Identity.Web.UI.Areas.MicrosoftIdentity.Controllers
{
    /// <summary>
    /// Controller used in web apps to manage accounts.
    /// </summary>
 
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
 
        /// <summary>
        /// Constructor of <see cref="AccountController"/> from <see cref="MicrosoftIdentityOptions"/>
        /// This constructor is used by dependency injection.
        /// </summary>
        /// <param name="microsoftIdentityOptionsMonitor">Configuration options.</param>
        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public UserManager<IdentityUser> UserManager { get; }
        public SignInManager<IdentityUser> SignInManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        [Route("Account/SignOut")]
        public async  Task  <IActionResult> SignOut()
        {
            await SignInManager.SignOutAsync();

            return RedirectToAction("index", "Home");
        }


        [Route("Account/SignIn")]
        public IActionResult SignIn(string returnUrl = null)
        {
            var provider = "Google";
 
            var redirectUrl = Url.Action("SignInCallback", "Account",
                               new { ReturnUrl = returnUrl });
            var properties = SignInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }


        [Route("signin-google")]
        public async Task<IActionResult> SignInCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return RedirectToAction( "index", "Home");
            }

            var xternalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("index", "Home");
            }

            // Sign in the user
            var result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }

            // If the user does not have an account, create a new account
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new IdentityUser { UserName = email, Email = email };
            user.EmailConfirmed = true;

            var createResult = await UserManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                return RedirectToAction("index", "Home");
            }

            // Link the external login to the new account
            var linkResult = await UserManager.AddLoginAsync(user, info);
            if (!linkResult.Succeeded)
            {
                return RedirectToAction("index", "Home");
            }

            // Sign in the new user
            await SignInManager.SignInAsync(user, isPersistent: false);


        
            return RedirectToLocal(returnUrl);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

    }
}
 