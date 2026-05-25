using Ketabi.Application.DTOs.Books;
using Ketabi.Application.DTOs.Category;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Enums;
using Ketabi.Web.Models;
using Ketabi.Web.ViewModels.Home;
using Ketabi.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;
using System.Text;

namespace Ketabi.Web.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBookListingService _bookListingService;
    private readonly ICategoryService _categoryService;

    public HomeController(
        ILogger<HomeController> logger,
        IBookListingService bookListingService,
        ICategoryService categoryService)
    {
        _logger = logger;
        _bookListingService = bookListingService;
        _categoryService = categoryService;
    }

    // GET: /  or  /Home/Index?q=...&mode=...&categoryId=...&page=...
    public async Task<IActionResult> Index(
        string? q,
        int? mode,
        Guid? categoryId,
        int page = 1)
    {
        var filter = new BookFilterViewModel
        {
            SearchQuery = q,
            SharingMode = mode.HasValue ? (SharingMode)mode.Value : null,
            CategoryId = categoryId,
            Page = page,
            PageSize = 20
        };

        // Build the filter DTO for server-side filtering
        var bookFilterDto = new BookFilterDto
        {
            Query = q,
            CategoryId = categoryId,
            SharingMode = mode.HasValue ? (SharingMode)mode.Value : null,
            PageNumber = page,
            PageSize = 20
        };

        IEnumerable<BookSummaryDto> dtos;
        try
        {
            dtos = await _bookListingService.GetFilteredBooksAsync(bookFilterDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load books for Explorer feed");
            dtos = [];
        }

        var cards = dtos.Select(dto => new BookCardViewModel
        {
            BookId = dto.Id,
            Title = dto.Title,
            Author = dto.Author,
            ImageUrl = dto.ImageUrl ?? string.Empty,
            Condition = dto.Condition,
            SharingMode = dto.SharingMode.ToLowerInvariant() switch
            {
                "exchange" => SharingMode.Exchange,
                "both" => SharingMode.Both,
                _ => SharingMode.Borrow
            },
            OwnerName = dto.OwnerName,
            OwnerAvatarUrl = dto.OwnerImageUrl ?? string.Empty,
            OwnerReputation = dto.OwnerRating,
            DistanceInKm = dto.DistanceInKm,
            IsAvailable = true
        }).ToList();

        // Fetch all categories from the database
        IEnumerable<CategoryDto> categoryDtos;
        try
        {
            categoryDtos = await _categoryService.GetAllCategoriesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load categories");
            categoryDtos = [];
        }

        var categories = categoryDtos
            .Select(cat => new CategoryFilterItemViewModel
            {
                CategoryId = cat.Id,
                Name = cat.Name,
                IconUrl = cat.IconUrl,
                IsActive = categoryId == cat.Id
            })
            .ToList();

        var vm = new ExplorerViewModel
        {
            Books = cards,
            Filter = filter,
            Categories = categories,
            Pager = new PagerViewModel
            {
                CurrentPage = page,
                TotalCount = cards.Count,
                TotalPages = (cards.Count + filter.PageSize - 1) / filter.PageSize
            }
        };

        return View(vm);
    }

    // API endpoint for AJAX filtering
    [HttpGet("/api/books/filter")]
    public async Task<IActionResult> FilterBooks(
        string? q,
        int? mode,
        Guid? categoryId,
        int page = 1)
    {
        var filter = new BookFilterViewModel
        {
            SearchQuery = q,
            SharingMode = mode.HasValue ? (SharingMode)mode.Value : null,
            CategoryId = categoryId,
            Page = page,
            PageSize = 20
        };

        // Build the filter DTO for server-side filtering
        var bookFilterDto = new BookFilterDto
        {
            Query = q,
            CategoryId = categoryId,
            SharingMode = mode.HasValue ? (SharingMode)mode.Value : null,
            PageNumber = page,
            PageSize = 20
        };

        IEnumerable<BookSummaryDto> dtos;
        try
        {
            dtos = await _bookListingService.GetFilteredBooksAsync(bookFilterDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load books for Explorer feed");
            dtos = [];
        }

        var cards = dtos.Select(dto => new BookCardViewModel
        {
            BookId = dto.Id,
            Title = dto.Title,
            Author = dto.Author,
            ImageUrl = dto.ImageUrl ?? string.Empty,
            Condition = dto.Condition,
            SharingMode = dto.SharingMode.ToLowerInvariant() switch
            {
                "exchange" => SharingMode.Exchange,
                "both" => SharingMode.Both,
                _ => SharingMode.Borrow
            },
            OwnerName = dto.OwnerName,
            OwnerAvatarUrl = dto.OwnerImageUrl ?? string.Empty,
            OwnerReputation = dto.OwnerRating,
            DistanceInKm = dto.DistanceInKm,
            IsAvailable = true
        }).ToList();

        // Fetch all categories from the database
        IEnumerable<CategoryDto> categoryDtos;
        try
        {
            categoryDtos = await _categoryService.GetAllCategoriesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load categories");
            categoryDtos = [];
        }

        var categories = categoryDtos
            .Select(cat => new CategoryFilterItemViewModel
            {
                CategoryId = cat.Id,
                Name = cat.Name,
                IconUrl = cat.IconUrl,
                IsActive = categoryId == cat.Id
            })
            .ToList();

        var vm = new ExplorerViewModel
        {
            Books = cards,
            Filter = filter,
            Categories = categories,
            Pager = new PagerViewModel
            {
                CurrentPage = page,
                TotalCount = cards.Count,
                TotalPages = (cards.Count + filter.PageSize - 1) / filter.PageSize
            }
        };

        // Return JSON response with HTML partials and pagination info
        return Json(new
        {
            filterBar = await RenderViewToString("_FilterBar", vm),
            bookGrid = await RenderViewToString("_BookGrid", vm),
            bookCards = await RenderBookCardsToString(cards),
            pagination = new
            {
                currentPage = page,
                pageSize = filter.PageSize,
                totalCount = cards.Count,
                hasMore = page * filter.PageSize < (vm.Pager.TotalCount > 0 ? vm.Pager.TotalCount : cards.Count)
            },
            filter = new
            {
                searchQuery = filter.SearchQuery,
                sharingMode = filter.SharingMode.HasValue ? (int?)filter.SharingMode : null,
                categoryId = filter.CategoryId
            }
        });
    }

    // Helper method to render book cards to string (for infinite scroll)
    private async Task<string> RenderBookCardsToString(List<BookCardViewModel> cards)
    {
        var output = new StringBuilder();
        foreach (var card in cards)
        {
            output.Append(await RenderViewToString("Shared/_BookCard", card));
        }
        return output.ToString();
    }

    // Helper method to render a partial view to string
    private async Task<string> RenderViewToString(string viewName, object model)
    {
        var viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
        var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor);

        var viewResult = viewEngine?.FindView(actionContext, viewName, false);
        if (viewResult?.View == null)
        {
            return string.Empty;
        }

        var view = viewResult.View;
        using (var output = new StringWriter())
        {
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var tempDataDictionary = new TempDataDictionary(HttpContext, HttpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider);

            var viewContext = new ViewContext(
                actionContext,
                view,
                viewDataDictionary,
                tempDataDictionary,
                output,
                new HtmlHelperOptions());

            await view.RenderAsync(viewContext);
            return output.ToString();
        }
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode = null)
    {
        if (statusCode.HasValue)
        {
            ViewData["StatusCode"] = statusCode.Value;
        }

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
