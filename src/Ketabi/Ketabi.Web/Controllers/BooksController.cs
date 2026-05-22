using System.Collections.Generic;
using Ketabi.Application.Common;
using AutoMapper;
using Ketabi.Application.DTOs.Books;
using Ketabi.Application.DTOs.Category;
using Ketabi.Application.DTOs.Requests;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Enums;
using Ketabi.Web.ViewModels.Books;
using Ketabi.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Ketabi.Web.Controllers;

[Authorize]
public class BooksController : BaseController
{
    private readonly IBookListingService _bookListingService;
    private readonly ICategoryService _categoryService;
    private readonly IRequestService _requestService;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;


    public BooksController(IBookListingService bookListingService, ICategoryService categoryService, IFileService fileService, IMapper mapper, IRequestService requestService)
    {
        _bookListingService = bookListingService;
        _categoryService = categoryService;
        _requestService = requestService;
        _mapper = mapper;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var viewModel = new CreateBookViewModel();
        await PopulateReferenceDataAsync(viewModel);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateReferenceDataAsync(model);
            return View(model);
        }

        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Upload cover image and build the public URL
            if (model.CoverImageFile == null || model.CoverImageFile.Length == 0)
            {
                ModelState.AddModelError(nameof(model.CoverImageFile), "Book cover image is required.");
                await PopulateReferenceDataAsync(model);
                return View(model);
            }
            var savedFileName = await _fileService.UploadFileAsync(model.CoverImageFile, AppConstants.Folders.BookCovers);
            model.CoverImageUrl = $"/uploads/{AppConstants.Folders.BookCovers}/{savedFileName}";

            var createDto = new CreateBookDto
            {
                Title = model.Title,
                Author = model.Author,
                ISBN = model.ISBN,
                Description = model.Description,
                Language = model.Language,
                Publisher = model.Publisher,
                CategoryId = model.CategoryId,
                Condition = model.Condition,
                SharingMode = model.SharingMode,
                ImageUrl = model.CoverImageUrl,
                LocationNote = model.LocationNote
            };

            await _bookListingService.CreateBookAsync(createDto, userId);

            TempData["SuccessMessage"] = "Book listed successfully!";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while creating the listing: " + ex.Message);
            await PopulateReferenceDataAsync(model);
            return View(model);
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            Guid? currentUserId = null;
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
            {
                currentUserId = parsedUserId;
            }

            // Fetch book details
            var bookDetailDto = await _bookListingService.GetBookByIdAsync(id, currentUserId);
            if (bookDetailDto == null)
            {
                return NotFound("Book not found");
            }

            var viewModel = await BuildBookDetailViewModelAsync(bookDetailDto, currentUserId);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the book details: " + ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            await _bookListingService.DeleteBookAsync(id, userId);
            TempData["SuccessMessage"] = "Listing deleted successfully.";
            return RedirectToAction("Index", "Home");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Book not found");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred while deleting the listing: " + ex.Message;
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBorrowRequest(BorrowRequestFormViewModel BorrowRequest)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            try
            {
                var bookDetailDto = await _bookListingService.GetBookByIdAsync(BorrowRequest.BookId, userId);
                var viewModel = await BuildBookDetailViewModelAsync(bookDetailDto, userId);
                viewModel.BorrowRequest = BorrowRequest;
                return View("Details", viewModel);
            }
            catch
            {
                return NotFound("Book not found");
            }
        }

        try
        {
            await _requestService.CreateBorrowRequestAsync(userId, new CreateBorrowRequestDto
            {
                ListingId = BorrowRequest.BookId,
                ReturnDate = BorrowRequest.ReturnDate,
                Note = BorrowRequest.Note
            });

            TempData["SuccessMessage"] = "Borrow request sent successfully.";
            return RedirectToAction("Index", "Requests", new { tab = "outgoing" });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            try
            {
                var bookDetailDto = await _bookListingService.GetBookByIdAsync(BorrowRequest.BookId, userId);
                var viewModel = await BuildBookDetailViewModelAsync(bookDetailDto, userId);
                viewModel.BorrowRequest = BorrowRequest;
                return View("Details", viewModel);
            }
            catch
            {
                return NotFound("Book not found");
            }
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExchangeRequest(ExchangeRequestFormViewModel ExchangeRequest)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            try
            {
                var bookDetailDto = await _bookListingService.GetBookByIdAsync(ExchangeRequest.BookId, userId);
                var viewModel = await BuildBookDetailViewModelAsync(bookDetailDto, userId);
                viewModel.ExchangeRequest = ExchangeRequest;
                return View("Details", viewModel);
            }
            catch
            {
                return NotFound("Book not found");
            }
        }

        try
        {
            await _requestService.CreateExchangeRequestAsync(userId, new CreateExchangeRequestDto
            {
                ListingId = ExchangeRequest.BookId,
                OfferedListingId = ExchangeRequest.OfferedBookId,
                Note = ExchangeRequest.Note
            });

            TempData["SuccessMessage"] = "Exchange request sent successfully.";
            return RedirectToAction("Index", "Requests", new { tab = "outgoing" });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            try
            {
                var bookDetailDto = await _bookListingService.GetBookByIdAsync(ExchangeRequest.BookId, userId);
                var viewModel = await BuildBookDetailViewModelAsync(bookDetailDto, userId);
                viewModel.ExchangeRequest = ExchangeRequest;
                return View("Details", viewModel);
            }
            catch
            {
                return NotFound("Book not found");
            }
        }
    }

    private async Task<BookDetailViewModel> BuildBookDetailViewModelAsync(BookDetailDto bookDetailDto, Guid? currentUserId)
    {
        var viewModel = new BookDetailViewModel
        {
            BookId = bookDetailDto.BookId,
            Title = bookDetailDto.Title,
            Author = bookDetailDto.Author,
            ISBN = bookDetailDto.ISBN,
            Description = bookDetailDto.Description,
            Language = bookDetailDto.Language,
            Publisher = bookDetailDto.Publisher,
            Category = bookDetailDto.Category,
            Condition = bookDetailDto.Condition,
            SharingMode = bookDetailDto.SharingMode,
            IsAvailable = bookDetailDto.IsAvailable,
            ImageUrl = bookDetailDto.ImageUrl,
            LocationNote = bookDetailDto.LocationNote,
            Owner = _mapper.Map<UserSummaryViewModel>(bookDetailDto.Owner),
            IsOwner = currentUserId.HasValue && bookDetailDto.Owner.UserId == currentUserId.Value,
            BorrowRequest = new BorrowRequestFormViewModel
            {
                BookId = bookDetailDto.BookId,
                ReturnDate = DateTime.UtcNow.AddDays(7)
            },
            ExchangeRequest = new ExchangeRequestFormViewModel
            {
                BookId = bookDetailDto.BookId
            }
        };

        var relatedBooks = await _bookListingService.GetRelatedBooksAsync(bookDetailDto.BookId, 1, 4);
        viewModel.RelatedBooks = relatedBooks
            .Select(rb => new BookCardViewModel
            {
                BookId = rb.Id,
                Title = rb.Title,
                Author = rb.Author ?? string.Empty,
                Category = rb.Category,
                ImageUrl = rb.ImageUrl ?? string.Empty,
                Condition = rb.Condition,
                SharingMode = Enum.TryParse<SharingMode>(rb.SharingMode, out var sharingMode) ? sharingMode : SharingMode.Both,
                IsAvailable = true,
                DistanceInKm = rb.DistanceInKm,
                OwnerId = rb.OwnerId,
                OwnerName = rb.OwnerName,
                OwnerAvatarUrl = rb.OwnerAvatarUrl ?? string.Empty,
                OwnerReputation = rb.OwnerReputation
            })
            .ToList();

        viewModel.BorrowDurationOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "3", Text = "3 Days" },
            new SelectListItem { Value = "7", Text = "1 Week" },
            new SelectListItem { Value = "14", Text = "2 Weeks" },
            new SelectListItem { Value = "30", Text = "1 Month" }
        };

        if (currentUserId.HasValue)
        {
            var viewerBooks = await _bookListingService.GetBooksByUserIdAsync(currentUserId.Value, 1, 100);
            viewModel.ViewerAvailableBooks = viewerBooks
                .Where(b => b.Id != bookDetailDto.BookId)
                .Select(rb => new BookCardViewModel
                {
                    BookId = rb.Id,
                    Title = rb.Title,
                    Author = rb.Author ?? string.Empty,
                    Category = rb.Category,
                    ImageUrl = rb.ImageUrl ?? string.Empty,
                    Condition = rb.Condition,
                    SharingMode = Enum.TryParse<SharingMode>(rb.SharingMode, out var sharingMode) ? sharingMode : SharingMode.Both,
                    IsAvailable = true,
                    OwnerId = rb.OwnerId,
                    OwnerName = rb.OwnerName,
                    OwnerAvatarUrl = rb.OwnerAvatarUrl ?? string.Empty,
                    OwnerReputation = rb.OwnerReputation
                })
                .ToList();
        }

        return viewModel;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var bookDetailDto = await _bookListingService.GetBookByIdAsync(id, userId);
        if (bookDetailDto == null)
        {
            return NotFound("Book not found");
        }

        var model = new EditBookViewModel
        {
            BookId = bookDetailDto.BookId,
            Title = bookDetailDto.Title,
            Author = bookDetailDto.Author,
            ISBN = bookDetailDto.ISBN,
            Description = bookDetailDto.Description,
            Language = bookDetailDto.Language,
            Publisher = bookDetailDto.Publisher,
            CategoryId = bookDetailDto.CategoryId,
            Condition = bookDetailDto.Condition,
            SharingMode = bookDetailDto.SharingMode,
            SharingDurationInDays = bookDetailDto.SharingDurationInDays,
            LocationNote = bookDetailDto.LocationNote,
            IsAvailable = bookDetailDto.IsAvailable,
            ExistingImageUrl = bookDetailDto.ImageUrl
        };

        await PopulateEditReferenceDataAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditBookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateEditReferenceDataAsync(model);
            return View(model);
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            string? uploadedImageUrl = null;
            if (model.CoverImage != null && model.CoverImage.Length > 0)
            {
                var savedFileName = await _fileService.UploadFileAsync(model.CoverImage, AppConstants.Folders.BookCovers);
                uploadedImageUrl = $"/uploads/{AppConstants.Folders.BookCovers}/{savedFileName}";
            }

            var updateDto = new UpdateBookDto
            {
                Title = model.Title,
                Author = model.Author,
                ISBN = model.ISBN,
                Description = model.Description,
                Language = model.Language,
                Publisher = model.Publisher,
                CategoryId = model.CategoryId,
                Condition = model.Condition,
                SharingMode = model.SharingMode,
                SharingDurationInDays = model.SharingDurationInDays,
                LocationNote = model.LocationNote,
                IsAvailable = model.IsAvailable,
                ImageUrl = uploadedImageUrl
            };

            await _bookListingService.UpdateBookAsync(model.BookId, updateDto, userId);
            TempData["SuccessMessage"] = "Book listing updated successfully.";
            return RedirectToAction("Details", new { id = model.BookId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while updating the listing: " + ex.Message);
            await PopulateEditReferenceDataAsync(model);
            return View(model);
        }
    }

    private async Task PopulateEditReferenceDataAsync(EditBookViewModel model)
    {
        model.CategoryOptions = await GetCategoryOptionsAsync();
    }

    private async Task<IList<SelectListItem>> GetCategoryOptionsAsync()
    {
        var categories = (await _categoryService.GetAllCategoriesAsync()).ToList();

        if (!categories.Any())
        {
            var defaultCategories = new[]
            {
                new CreateCategoryDto { Name = "Fiction", Description = "Fiction books and stories" },
                new CreateCategoryDto { Name = "Philosophy", Description = "Philosophy and self-reflection" },
                new CreateCategoryDto { Name = "Science", Description = "Science and technology books" },
                new CreateCategoryDto { Name = "History", Description = "History and biographies" }
            };

            foreach (var defaultCategory in defaultCategories)
            {
                await _categoryService.CreateCategoryAsync(defaultCategory);
            }

            categories = (await _categoryService.GetAllCategoriesAsync()).ToList();
        }

        var options = categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = string.IsNullOrEmpty(c.IconUrl) ? c.Name : $"{c.IconUrl} {c.Name}"
        }).ToList();

        if (!options.Any())
        {
            options = new List<SelectListItem>
            {
                new SelectListItem { Value = Guid.NewGuid().ToString(), Text = "Fiction" },
                new SelectListItem { Value = Guid.NewGuid().ToString(), Text = "Philosophy" },
                new SelectListItem { Value = Guid.NewGuid().ToString(), Text = "Science" },
                new SelectListItem { Value = Guid.NewGuid().ToString(), Text = "History" }
            };
        }

        return options;
    }

    private async Task PopulateReferenceDataAsync(CreateBookViewModel model)
    {
        var categories = (await _categoryService.GetAllCategoriesAsync()).ToList();

        if (!categories.Any())
        {
            var defaultCategories = new[]
            {
                new CreateCategoryDto { Name = "Fiction", Description = "Fiction books and stories" },
                new CreateCategoryDto { Name = "Philosophy", Description = "Philosophy and self-reflection" },
                new CreateCategoryDto { Name = "Science", Description = "Science and technology books" },
                new CreateCategoryDto { Name = "History", Description = "History and biographies" }
            };

            foreach (var defaultCategory in defaultCategories)
            {
                await _categoryService.CreateCategoryAsync(defaultCategory);
            }

            categories = (await _categoryService.GetAllCategoriesAsync()).ToList();
        }

        model.CategoryOptions = categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = string.IsNullOrEmpty(c.IconUrl) ? c.Name : $"{c.IconUrl} {c.Name}"
        }).ToList();

        
        if (!model.CategoryOptions.Any())
        {
            model.CategoryOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = Guid.NewGuid().ToString(), Text = "Fiction" },
                new SelectListItem { Value = Guid.NewGuid().ToString(), Text = "Philosophy" },
                new SelectListItem { Value = Guid.NewGuid().ToString(), Text = "Science" },
                new SelectListItem { Value = Guid.NewGuid().ToString(), Text = "History" }
            };
        }
    }
}
