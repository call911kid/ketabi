using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.Category;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Interfaces;

namespace Ketabi.Application.Services
{
    internal class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            
            if (category == null) 
                return null;

            return MapToDto(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            // تحقق من عدم تكرار اسم القسم
            var existingCategory = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Name == createCategoryDto.Name);
            if (existingCategory != null)
            {
                throw new InvalidOperationException($"Category with the name '{createCategoryDto.Name}' already exists.");
            }

            var category = new Category(Guid.NewGuid())
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                IconUrl = createCategoryDto.IconUrl,
                Emoji = string.Empty,
                Color = createCategoryDto.Color
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<bool> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(updateCategoryDto.Id);
            if (category == null) 
                return false;

            
            var existingCategoryWithSameName = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Name == updateCategoryDto.Name && c.Id != updateCategoryDto.Id);
            if (existingCategoryWithSameName != null)
            {
                throw new InvalidOperationException($"Another category with the name '{updateCategoryDto.Name}' already exists.");
            }

            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;
            category.IconUrl = updateCategoryDto.IconUrl;
            category.Emoji = string.Empty;
            category.Color = updateCategoryDto.Color;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) 
                return false;

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        
        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IconUrl = category.IconUrl,
                Color = category.Color,
                BookCount = category.BookListings?.Count ?? 0,
                CreatedAt = category.CreatedAt
            };
        }
    }
}
