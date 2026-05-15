using Ketabi.Application.DTOs.Category;
using Ketabi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ketabi.Web.Controllers
{
    // You can add [Authorize(Roles = "Admin")] here when authentication is set up
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("/Admin/Categories")]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryService.CreateCategoryAsync(model);
                    TempData["SuccessMessage"] = "Category created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please fill all required fields correctly.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCategoryDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _categoryService.UpdateCategoryAsync(model);
                    if (result)
                    {
                        TempData["SuccessMessage"] = "Category updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Category not found.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please fill all required fields correctly.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Category deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Category not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
