using Ketabi.Application.DTOs.Books;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Enums;
using Ketabi.Web.Models;
using Ketabi.Web.ViewModels.Home;
using Ketabi.Web.ViewModels.Shared;
using Ketabi.Web.ViewModels.Books;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Ketabi.Web.ViewModels.Books;

namespace Ketabi.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController>  _logger;
    private readonly IBookListingService      _bookListingService;

    // Static category list — replaces ICategoryService until that service is wired.
    private static readonly string[] _categories =
    [
        "Fiction", "Philosophy", "Science", "History", "Psychology",
        "Mathematics", "Literature", "Self-Help", "Economics",
        "Business", "Science Fiction"
    ];

    public HomeController(
        ILogger<HomeController> logger,
        IBookListingService     bookListingService)
    {
<<<<<<< HEAD
        _logger             = logger;
        _bookListingService = bookListingService;
=======
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


    public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
>>>>>>> e1641b1 (Book wazard controller and views)
    }

    // GET: /  or  /Home/Index?q=...&mode=...&categoryId=...&page=...
    public async Task<IActionResult> Index(
        string? q,
        int?    mode,
        Guid?   categoryId,
        int     page = 1)
    {
        // ── 1. Build filter state ──────────────────────────────────────
        var filter = new BookFilterViewModel
        {
            SearchQuery = q,
            SharingMode = mode.HasValue ? (SharingMode)mode.Value : null,
            CategoryId  = categoryId,
            Page        = page,
            PageSize    = 20
        };

        // ── 2. Fetch books ─────────────────────────────────────────────
        IEnumerable<BookSummaryDto> dtos;
        try
        {
            dtos = await _bookListingService.GetAllBooksAsync(page, filter.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load books for Explorer feed");
            dtos = [];
        }

        // ── 3. Client-side filtering (until server-side filtering is ready) ──
        if (!string.IsNullOrWhiteSpace(q))
        {
            var lq = q.ToLowerInvariant();
            dtos = dtos.Where(b =>
                b.Title.Contains(lq,  StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(lq, StringComparison.OrdinalIgnoreCase));
        }

        if (mode.HasValue)
        {
            var sharingModeStr = ((SharingMode)mode.Value).ToString().ToLowerInvariant();
            dtos = dtos.Where(b =>
                b.SharingMode.Equals(sharingModeStr, StringComparison.OrdinalIgnoreCase) ||
                b.SharingMode.Equals("both",          StringComparison.OrdinalIgnoreCase));
        }

        // ── 4. Map DTOs → BookCardViewModels ──────────────────────────
        var cards = dtos.Select(dto => new BookCardViewModel
        {
            BookId          = dto.Id,
            Title           = dto.Title,
            Author          = dto.Author,
            ImageUrl        = dto.ImageUrl ?? string.Empty,
            Condition       = dto.Condition,
            SharingMode     = dto.SharingMode.ToLowerInvariant() switch
            {
                "exchange" => SharingMode.Exchange,
                "both"     => SharingMode.Both,
                _          => SharingMode.Borrow
            },
            OwnerName       = dto.OwnerName,
            OwnerAvatarUrl  = dto.OwnerImageUrl ?? string.Empty,
            OwnerReputation = dto.OwnerRating,
            DistanceInKm    = dto.DistanceInKm,
            IsAvailable     = true
        }).ToList();

        // ── 5. Build category filter items ────────────────────────────
        var categories = _categories
            .Select(name => new CategoryFilterItemViewModel
            {
                CategoryId = null,           // real Guid lookup added when ICategoryService is wired
                Name       = name,
                IsActive   = false           // no match by name until CategoryId is used
            })
            .ToList();

        // ── 6. Assemble ViewModel ─────────────────────────────────────
        var vm = new ExplorerViewModel
        {
            Books      = cards,
            Filter     = filter,
            Categories = categories,
            Pager      = new PagerViewModel
            {
                CurrentPage = page,
                TotalCount  = cards.Count,   // replace with total from service when available
                TotalPages  = 1              // update when paginated API returns total pages
            }
        };

        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
