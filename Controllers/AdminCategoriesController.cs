using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/admin/categories")]
    [AllowAnonymous]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public AdminCategoriesController(ECommerceDbContext context)
        {
            _context = context;
        }

        //////////////////////////////////////////////////////
        // GET ALL CATEGORIES
        //////////////////////////////////////////////////////

        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _context.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Slug,
                    c.Description,
                    c.ParentId,
                    c.ImageUrl
                })
                .ToList();

            return Ok(categories);
        }

        //////////////////////////////////////////////////////
        // GET CATEGORY BY ID
        //////////////////////////////////////////////////////

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var category = _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Slug,
                    c.Description,
                    c.ParentId,
                    c.ImageUrl
                })
                .FirstOrDefault();

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        //////////////////////////////////////////////////////
        // GET CATEGORY TREE
        //////////////////////////////////////////////////////

        [HttpGet("tree")]
        public IActionResult GetTree()
        {
            var categories = _context.Categories.ToList();

            var tree = categories
                .Where(c => c.ParentId == null)
                .Select(parent => new
                {
                    parent.Id,
                    parent.Name,
                    parent.Slug,
                    parent.Description,
                    parent.ImageUrl,

                    Children = categories
                        .Where(c => c.ParentId == parent.Id)
                        .Select(child => new
                        {
                            child.Id,
                            child.Name,
                            child.Slug,
                            child.Description,
                            child.ImageUrl
                        })
                        .ToList()
                })
                .ToList();

            return Ok(tree);
        }

        //////////////////////////////////////////////////////
        // CREATE CATEGORY
        //////////////////////////////////////////////////////

        [HttpPost]
        public IActionResult Create(CategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Slug = dto.Slug,
                Description = dto.Description,
                ParentId = dto.ParentId,
                ImageUrl = dto.ImageUrl
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return Ok(category);
        }

        //////////////////////////////////////////////////////
        // UPDATE CATEGORY
        //////////////////////////////////////////////////////

        [HttpPut("{id}")]
        public IActionResult Update(int id, CategoryDto dto)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
                return NotFound();

            category.Name = dto.Name;
            category.Slug = dto.Slug;
            category.Description = dto.Description;
            category.ParentId = dto.ParentId;
            category.ImageUrl = dto.ImageUrl;

            _context.SaveChanges();

            return Ok(category);
        }

        //////////////////////////////////////////////////////
        // DELETE CATEGORY
        //////////////////////////////////////////////////////

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok(new { message = "Category deleted successfully" });
        }
    }
}