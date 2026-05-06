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
    private readonly ILogger<HomeController> _logger;
    private readonly IBookListingService _bookListingService;

    private static readonly string[] _categories =
    [
        "Fiction", "Philosophy", "Science", "History", "Psychology",
        "Mathematics", "Literature", "Self-Help", "Economics",
        "Business", "Science Fiction"
    ];

    public HomeController(
        ILogger<HomeController> logger,
        IBookListingService bookListingService)
    {
        _logger = logger;
        _bookListingService = bookListingService;
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

        if (!string.IsNullOrWhiteSpace(q))
        {
            var lq = q.ToLowerInvariant();
            dtos = dtos.Where(b =>
                b.Title.Contains(lq, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(lq, StringComparison.OrdinalIgnoreCase));
        }

        if (mode.HasValue)
        {
            var sharingModeStr = ((SharingMode)mode.Value).ToString().ToLowerInvariant();
            dtos = dtos.Where(b =>
                b.SharingMode.Equals(sharingModeStr, StringComparison.OrdinalIgnoreCase) ||
                b.SharingMode.Equals("both", StringComparison.OrdinalIgnoreCase));
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

        var categories = _categories
            .Select(name => new CategoryFilterItemViewModel
            {
                CategoryId = null,
                Name = name,
                IsActive = false
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
                TotalPages = 1
            }
        };

        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
}
