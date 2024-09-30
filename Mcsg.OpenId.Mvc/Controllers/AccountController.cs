using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace Mcsg.OpenId.Mvc.Controllers;

using Common.Domain;
using Common.SeedWork;
using Interfaces;
using Models;

/// <summary>
/// Account controller
/// </summary>
public class AccountController : Controller
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="setting">Setting</param>
    /// <param name="userManager">User manager</param>
    /// <param name="appManager">App manager</param>
    public AccountController(ISetting setting, ApplicationUserManager userManager, IOpenIddictApplicationManager appManager)
    {
        _setting = setting;
        _aes = new SecurityAes(_setting.EncryptKey);
        _userManager = userManager;
        _appManager = appManager;
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="returnUrl">Return URL</param>
    /// <returns>Returns the result</returns>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="vm">View model</param>
    /// <returns>Returns the result</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        ViewData["ReturnUrl"] = vm.ReturnUrl;

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var encryptedEmail = _aes.EncryptText(vm.Username);

        if (string.IsNullOrEmpty(encryptedEmail))
        {
            ModelState.AddModelError(string.Empty, "Invalid username.");
            return View(vm);
        }

        var user = await _userManager.FindByEmailAsync(encryptedEmail);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(vm);
        }

        var signinResult = await _userManager.CheckPasswordAsync(user, vm.Password);
        if (!signinResult)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(vm);
        }

        var claims = new List<Claim> { new(ClaimTypes.Name, user.UserName + "") };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

        if (Url.IsLocalUrl(vm.ReturnUrl))
        {
            return Redirect(vm.ReturnUrl);
        }

        return View(vm);
    }

    /// <summary>
    /// Logout
    /// </summary>
    /// <returns>Returns the result</returns>
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;

    /// <summary>
    /// User manager
    /// </summary>
    private readonly ApplicationUserManager _userManager;

    /// <summary>
    /// Application manager
    /// </summary>
    private readonly IOpenIddictApplicationManager _appManager;

    #endregion
}
