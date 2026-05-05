using AutoMapper;
using Ketabi.Application.DTOs.Auth;
using Ketabi.Application.Interfaces;
using Ketabi.Application.Services;
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

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.ProfilePicture != null && model.ProfilePicture.Length > 10 * 1024 * 1024)
            {
                ModelState.AddModelError("ProfilePicture", "Profile picture must be less than 10MB.");
                return View(model);
            }
            var request = _mapper.Map<RegisterRequest>(model);

            try
            {
                if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
                {
                    // TODO: must reimplement FileService
                    string fileName = await FileService.UploadFileAsync(model.ProfilePicture);
                    request.ProfilePictureUrl = $"/Uploads/{fileName}";
                }

                await _authService.RegisterAsync(request);

                TempData["SuccessMessage"] = "Account created successfully!";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex) // add specific exception type for better error handling
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

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
                    Expires = model.RememberMe ? DateTime.UtcNow.AddDays(7) : null
                };

                Response.Cookies.Append("AuthToken", response.Token, cookieOptions);

                return string.IsNullOrEmpty(model.ReturnUrl)
                    ? RedirectToAction("Index", "Home")
                    : Redirect(model.ReturnUrl);
            }
            catch (Exception) // add specific exception type for better error handling
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Login");
        }

    }
}