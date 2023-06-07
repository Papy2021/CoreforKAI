using Core.ViewModeles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Core.Controllers
{
    public class AccountController : Controller
    {
        #pragma warning disable CS8604 // Possible null reference return.

        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> looger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = looger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var userResult = await userManager.CreateAsync(user, model.Password);
                if (userResult.Succeeded)
                {
                    var uToken=await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = uToken }, Request.Scheme);
                    logger.Log(LogLevel.Warning, confirmationLink);


                    if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    ViewBag.ErrorTitle = "Registration Successful";
                    ViewBag.ErrorMessage = "Before to continue! please confirm your email by clicking on the link we have emailed you!";
                    return View("Error");
                }
                else
                {
                    foreach(var error in userResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            
            return View(model);
        }



        [AllowAnonymous]
        [HttpPost]
        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Welcome", "Home"); 
        }



        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            LoginViewModel model = new()
            {

                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };
            return View(model);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var currentUser= await userManager.FindByEmailAsync(model.Email);

                if (currentUser != null && !currentUser.EmailConfirmed 
                    && (await userManager.CheckPasswordAsync(currentUser, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(model);
                }

                var signInResult= await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if(signInResult.Succeeded)
                {
                    if (returnUrl != null)
                    {
                        LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Welcome", "Home");
                    }
                }

                if (signInResult.IsLockedOut)
                {
                    return View("AccountLocked");
                }
               ModelState.AddModelError(string.Empty, "Invalid user name and/or Password");
            }
                   
            return View(model);
        }


        [AllowAnonymous]
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailUsed(string email)
        {
            var EmailChecked = await userManager.FindByEmailAsync(email);
            if (EmailChecked==null) 
            {
                return Json(true);
            }
            else
            {
                return Json("This email address is in use");
            }
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
    
        }


        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");
            var loginModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins=(await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };


            if(remoteError != null)
            {
                ModelState.AddModelError(string.Empty,$"Error from external provider { remoteError}");
                return View(loginModel);
            }

            var info=await signInManager.GetExternalLoginInfoAsync();
            
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error loading external info");
                return View(loginModel);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            IdentityUser? currentUser = null;

            if(email!=null)
            {
                currentUser=await userManager.FindByEmailAsync(email);
                if (currentUser != null && !currentUser.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View("Login", loginModel);
                }
            }


            var signInResult=await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false,
                bypassTwoFactor:true);

            if(signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
               
                if (email != null)
                {
                    if (currentUser == null)
                    {
                        currentUser = new IdentityUser
                        {
                            UserName=info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        };
                        await userManager.CreateAsync(currentUser);

                        var uToken = await userManager.GenerateEmailConfirmationTokenAsync(currentUser);
                        var confirmationLink=Url.Action("ConfirmEmail", "Account", 
                            new {userId=currentUser.Id, token=uToken}, Request.Scheme);

                        logger.Log(LogLevel.Warning, confirmationLink);
                        ViewBag.ErrorTitle = "Registration Successfull";
                        ViewBag.ErrorMessage = "Before you can login, please confirm your email, by clicking on the confirmation link we have emailed you";
                        return View("Error");
                    }
                    await userManager.AddLoginAsync(currentUser, info);
                    await signInManager.SignInAsync(currentUser, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"No Email received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on papytshokengele@gmail.com";

                return View("Error");
            }

           
        }


        [AllowAnonymous]
        public  async Task<IActionResult> ConfirmEmail(string? userId, string token)
        {
            if(userId== null || token == null)
            {
                return RedirectToAction("Welcome", "Home");
            }
            var currentUser= await userManager.FindByIdAsync(userId);
            if(currentUser == null)
            {
                ViewBag.ErrorMessage = $"The user ID {userId} is invalid";
                return View("NotFound");
            }

            var confirResult = await userManager.ConfirmEmailAsync(currentUser, token);

            if (confirResult.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await userManager.FindByEmailAsync(model.Email);
                if (currentUser != null && await userManager.IsEmailConfirmedAsync(currentUser))
                {
                    var uToken = await userManager.GeneratePasswordResetTokenAsync(currentUser);
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email,token = uToken },Request.Scheme);
                    logger.Log(LogLevel.Warning, passwordResetLink);
                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string? email, string? token)
        {
            if (email == null || token==null)
            {
                ModelState.AddModelError(string.Empty, "Invalid password reset token");  
                return View();
            }
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser= await userManager.FindByEmailAsync(model.Email);
                if (currentUser != null)
                {
                    var resetResult=await userManager.ResetPasswordAsync(currentUser,model.Token,model.Password);
                    if (resetResult.Succeeded)
                    {

                        if(await userManager.IsLockedOutAsync(currentUser))
                        {
                            await userManager.SetLockoutEndDateAsync(currentUser, DateTimeOffset.UtcNow);
                        }
                        return View("ResetPasswordConfirmation");
                    }
                    foreach(var error in resetResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var currentUser = await userManager.GetUserAsync(User);

            var userHasPW = await userManager.HasPasswordAsync(currentUser);
            if (!userHasPW)
            {
                return RedirectToAction("AddPassword");
            }
            return View();
        }


        [HttpPost]
        public async  Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currenUser =await  userManager.GetUserAsync(User);
                if (currenUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                var changeResult = await userManager.ChangePasswordAsync(currenUser, model.CurrentPassword, model.NewPassword);
                if (!changeResult.Succeeded)
                {
                    foreach (var error in changeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        
                    }
                    return View();
                }
                await signInManager.RefreshSignInAsync(currenUser);
                return View("ChangePasswordConfirmation");

            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var currentUser=await userManager.GetUserAsync(User);

            var userHasPW=await userManager.HasPasswordAsync(currentUser);
            if (userHasPW)
            {
                return RedirectToAction("ChangePassword");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await userManager.GetUserAsync(User);
                var addResult = await userManager.AddPasswordAsync(currentUser, model.NewPassword);

                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }
                await signInManager.RefreshSignInAsync(currentUser);
                return View("AddPasswordConfirmation");
               
            } 
            return View();
        }
    }
}
