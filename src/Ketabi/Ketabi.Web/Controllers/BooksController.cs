using Ketabi.Application.Common;
using Ketabi.Application.DTOs.Books;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Enums;
using Ketabi.Web.ViewModels.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Ketabi.Web.Controllers;

[Authorize]
public class BooksController : Controller
{
    private readonly IBookListingService _bookListingService;
    private readonly ICategoryService _categoryService;
    private readonly IFileService _fileService;

    public BooksController(IBookListingService bookListingService, ICategoryService categoryService, IFileService fileService)
    {
        _bookListingService = bookListingService;
        _categoryService = categoryService;
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

    private async Task PopulateReferenceDataAsync(CreateBookViewModel model)
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        model.CategoryOptions = categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
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
