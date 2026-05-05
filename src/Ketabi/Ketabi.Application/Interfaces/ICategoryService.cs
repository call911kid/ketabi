using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.Category;

namespace Ketabi.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<bool> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
