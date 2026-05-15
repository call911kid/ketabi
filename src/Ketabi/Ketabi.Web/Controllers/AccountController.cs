using AutoMapper;
using Ketabi.Application.Common;
using Ketabi.Application.DTOs.Auth;
using Ketabi.Application.Interfaces;
using Ketabi.Web.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace Ketabi.Web.Controllers
{
    public class AccountController : Controller
    {
        private IAuthService _authService;
        private IMapper _mapper;

        public AccountController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        #region Register

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var request = _mapper.Map<RegisterRequest>(model);
            request.ProfilePictureUrl = AppConstants.DefaultProfilePic;

            try
            {
                await _authService.RegisterAsync(request);

                TempData[AppConstants.SuccessMessageKey] = Messages.Auth.RegisterSuccess;
                return RedirectToAction(nameof(Login));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        #endregion

        #region Login

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
            => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var request = _mapper.Map<LoginRequest>(model);

            try
            {
                var response = await _authService.LoginAsync(request);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = model.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddDays(1)
                };

                Response.Cookies.Append(AppConstants.AuthCookieName, response.Token, cookieOptions);

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError(string.Empty, Messages.Auth.LoginFailed);
                return View(model);
            }
        }

        #endregion

        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(AppConstants.AuthCookieName);
            return RedirectToAction(nameof(Login));
        }

        #endregion

        #region Unauthorized

        [HttpGet("/Account/Unauthorized")]
        public IActionResult UnauthorizedPage()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            ViewData["Title"] = "Unauthorized";
            return View("Unauthorized");
        }

        #endregion
    }
}