using AutoMapper;
using Ketabi.Application.Common;
using Ketabi.Application.DTOs.Users;
using Ketabi.Application.Interfaces;
using Ketabi.Web.ViewModels.Profile;
using Ketabi.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ketabi.Web.Controllers;

public class ProfileController : BaseController
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IUserService userService, IMapper mapper, IFileService fileService, ILogger<ProfileController> logger)
    {
        _userService = userService;
        _mapper = mapper;
        _fileService = fileService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid id)
    {
        try
        {
            Guid? currentUserId = null;
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(idClaim) && Guid.TryParse(idClaim, out var parsed)) currentUserId = parsed;

            if (id == Guid.Empty) id = currentUserId ?? Guid.Empty;
            var profileDto = await _userService.GetUserProfileAsync(id, currentUserId);
            var vm = _mapper.Map<ProfileViewModel>(profileDto);

            return View(vm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load profile {ProfileId}", id);
            if (ex is Ketabi.Application.Exceptions.NotFoundException) return NotFound();
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        try
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId)) return RedirectToAction("Login", "Account");

            var profileDto = await _userService.GetUserProfileAsync(userId, userId);
            var editVm = _mapper.Map<EditProfileViewModel>(profileDto);
            editVm.UserId = profileDto.UserId;

            return View(editVm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load edit profile");
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId)) return RedirectToAction("Login", "Account");

            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                var saved = await _fileService.UploadFileAsync(model.ProfilePicture, AppConstants.Folders.UserUploads);
                var url = $"/uploads/{AppConstants.Folders.UserUploads}/{saved}";
                // Map to DTO and set ProfilePictureUrl explicitly
                var mapped = _mapper.Map<UpdateUserProfileDto>(model);
                var updateDtoWithPic = new UpdateUserProfileDto
                {
                    FirstName = mapped.FirstName,
                    LastName = mapped.LastName,
                    Bio = mapped.Bio,
                    City = mapped.City,
                    Governorate = mapped.Governorate,
                    ProfilePictureUrl = url
                };

                await _userService.UpdateUserProfileAsync(userId, updateDtoWithPic);
            }
            else
            {
                var updateDto = _mapper.Map<UpdateUserProfileDto>(model);
                await _userService.UpdateUserProfileAsync(userId, updateDto);
            }

            TempData[AppConstants.SuccessMessageKey] = "Profile updated successfully!";
            return RedirectToAction("Index", new { id = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update profile for user {UserId}", model.UserId);
            TempData[AppConstants.ErrorMessageKey] = "Failed to update profile.";
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        try
        {
            var summaryDto = await _userService.GetUserByIdAsync(id);
            var vm = _mapper.Map<UserSummaryViewModel>(summaryDto);
            return PartialView("_UserSummary", vm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load user summary {UserId}", id);
            return NotFound();
        }
    }
}
