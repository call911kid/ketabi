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
    private readonly IBookListingService _bookListingService;
    private readonly IReviewService _reviewService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IUserService userService, IMapper mapper, IFileService fileService, IBookListingService bookListingService, IReviewService reviewService, ILogger<ProfileController> logger)
    {
        _userService = userService;
        _mapper = mapper;
        _fileService = fileService;
        _bookListingService = bookListingService;
        _reviewService = reviewService;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        try
        {
            if (file is null || file.Length == 0)
                return Json(new { success = false, error = "No file provided." });

            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
                return Json(new { success = false, error = "Not authenticated." });

            _fileService.DeleteFile(file.FileName, AppConstants.Folders.UserUploads);
            var savedFileName = await _fileService.UploadFileAsync(file, AppConstants.Folders.UserUploads);
            var newImageUrl = $"{savedFileName}";

            await _userService.UpdateUserProfileAsync(userId, new UpdateUserProfileDto
            {
                ProfilePictureUrl = newImageUrl
            });

            //return Json(new { success = true, newImageUrl });
            return RedirectToAction("Index", new { id = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Avatar upload failed");
            return Json(new { success = false, error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid id, int booksPage = 1, int reviewsPage = 1)
    {
        try
        {
            Guid? currentUserId = null;
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(idClaim) && Guid.TryParse(idClaim, out var parsed)) currentUserId = parsed;

            if (id == Guid.Empty) id = currentUserId ?? Guid.Empty;

            var profileDto = await _userService.GetUserProfileAsync(id, currentUserId);
            var vm = _mapper.Map<ProfileViewModel>(profileDto);

            // Normalize identity and presentation fields
            vm.Email = profileDto.Email ?? string.Empty;
            vm.FirstName = profileDto.FirstName ?? string.Empty;
            vm.LastName = profileDto.LastName ?? string.Empty;
            vm.FullName = string.Join(' ', new[] { vm.FirstName, vm.LastName }.Where(s => !string.IsNullOrWhiteSpace(s)));
            vm.Bio = profileDto.Bio;
            vm.UserName = string.IsNullOrWhiteSpace(profileDto.Email) ? string.Empty : profileDto.Email.Split('@').FirstOrDefault() ?? string.Empty;
            vm.IsOwnProfile = profileDto.IsOwnProfile;

            // Avatar: DTO stores filename only; resolve to public URL for views
            vm.AvatarUrl = string.IsNullOrWhiteSpace(profileDto.AvatarUrl)
                ? AppConstants.DefaultProfilePic
                : profileDto.AvatarUrl;

            // Stats
            vm.ReputationScore = profileDto.Stats?.ReputationScore ?? 0.0;
            vm.ReviewCount = profileDto.Stats?.ReviewCount ?? 0;
            vm.BooksListed = profileDto.Stats?.BooksListed ?? 0;
            vm.ActiveListings = profileDto.Stats?.BooksListed ?? 0;
            vm.CompletedBorrows = profileDto.Stats?.CompletedTransactions ?? 0;
            vm.CompletedExchanges = 0;
            vm.MemberSince = profileDto.MemberSince ?? string.Empty;

            // Load user's books for listings tab (paginated)
            var booksDto = await _bookListingService.GetBooksByUserIdAsync(id, booksPage, 6);
            var books = booksDto.Select(b => new Ketabi.Web.ViewModels.Shared.BookCardViewModel
            {
                BookId = b.Id,
                Title = b.Title,
                Author = b.Author ?? string.Empty,
                ImageUrl = string.IsNullOrWhiteSpace(b.ImageUrl) ? "/img/cover-placeholder.svg" : b.ImageUrl,
                Condition = b.Condition,
                SharingMode = Enum.TryParse<Ketabi.Core.Domain.Enums.SharingMode>(b.SharingMode, true, out var sm) ? sm : Ketabi.Core.Domain.Enums.SharingMode.Borrow,
                IsAvailable = true,
                OwnerName = b.OwnerName,
                OwnerAvatarUrl = string.IsNullOrWhiteSpace(b.OwnerAvatarUrl) ? AppConstants.DefaultProfilePic : b.OwnerAvatarUrl,
                OwnerReputation = b.OwnerRating,
                ShowOwnerActions = currentUserId.HasValue && currentUserId.Value == id,
                DistanceInKm = b.DistanceInKm
            }).ToList();

            // Load paged reviews and map to view models
            try
            {
                var reviewsResult = await _reviewService.GetReviewsForUserAsync(id, new Ketabi.Application.DTOs.Common.PagedRequestDto { Page = reviewsPage, PageSize = 5 });
                if (reviewsResult != null && reviewsResult.Success && reviewsResult.Data != null)
                {
                    vm.Reviews = reviewsResult.Data.Items.Select(r => new Ketabi.Web.ViewModels.Profile.ReviewItemViewModel
                    {
                        ReviewId = r.ReviewId,
                        ReviewerName = r.Reviewer?.FullName ?? string.Empty,
                        ReviewerAvatar = string.IsNullOrWhiteSpace(r.Reviewer?.AvatarUrl) ? "/uploads/users/profile-picture.png" : $"/uploads/users/{r.Reviewer?.AvatarUrl}",
                        Rating = r.Rating,
                        Comment = r.Comment,
                        RelatedBookTitle = r.RelatedBookTitle ?? string.Empty,
                        TimeAgo = r.TimeAgo ?? string.Empty
                    }).ToList();

                    vm.ReviewsPager = new PagerViewModel
                    {
                        CurrentPage = reviewsResult.Data.Page,
                        TotalCount = reviewsResult.Data.TotalCount,
                        TotalPages = reviewsResult.Data.TotalPages
                    };
                }
                else
                {
                    vm.Reviews = new List<ReviewItemViewModel>();
                    vm.ReviewsPager = new PagerViewModel { CurrentPage = reviewsPage, TotalCount = 0, TotalPages = 0 };
                    if (reviewsResult != null)
                        _logger.LogWarning("Failed to load reviews for user {UserId}: {Errors}", id, string.Join(';', reviewsResult.Errors ?? new List<string>()));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reviews for user {UserId}", id);
                vm.Reviews = new List<ReviewItemViewModel>();
                vm.ReviewsPager = new PagerViewModel { CurrentPage = reviewsPage, TotalCount = 0, TotalPages = 0 };
            }

            vm.Books = books;

            vm.BooksPager = new PagerViewModel
            {
                CurrentPage = booksPage,
                TotalCount = profileDto.Stats?.BooksListed ?? 0,
                TotalPages = profileDto.Stats != null && profileDto.Stats.BooksListed > 0 ? (int)Math.Ceiling((double)profileDto.Stats.BooksListed / 6) : 0
            };

            // Prepare edit form when viewing own profile
            if (vm.IsOwnProfile)
            {
                vm.EditForm = _mapper.Map<EditProfileViewModel>(profileDto);
                vm.EditForm.UserId = profileDto.UserId;
            }

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
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
                return Challenge();

            var profileDto = await _userService.GetUserProfileAsync(userId, userId);
            var editVm = _mapper.Map<EditProfileViewModel>(profileDto);
            editVm.UserId = userId;

            return PartialView("_EditProfileModal", editVm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load edit profile modal");
            return BadRequest();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_EditProfileModal", model);
        }

        try
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
                return Json(new { success = false, message = "Unauthorized access." });

            var updateDto = _mapper.Map<UpdateUserProfileDto>(model);

            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                var savedFileName = await _fileService.UploadFileAsync(model.ProfilePicture, AppConstants.Folders.UserUploads);
                updateDto.ProfilePictureUrl = savedFileName;
            }

            await _userService.UpdateUserProfileAsync(userId, updateDto);

            TempData[AppConstants.SuccessMessageKey] = "Profile updated successfully!";
            return Json(new { success = true, redirectUrl = Url.Action("Index", new { id = userId }) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update profile for user {UserId}", model.UserId);
            ModelState.AddModelError(string.Empty, "An unexpected error occurred while saving your profile.");
            return PartialView("_EditProfileModal", model);
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
