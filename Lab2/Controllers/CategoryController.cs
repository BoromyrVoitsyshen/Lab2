using AutoMapper;
using Lab2.Dto;
using Lab2.Interfaces;
using Lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryInterface _categoryInterface;
        private readonly IWareInterface _wareInterface;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryInterface categoryInterface, IMapper mapper, IWareInterface wareInterface)
        {
            _categoryInterface = categoryInterface;
            _mapper = mapper;
            _wareInterface = wareInterface;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]

        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryInterface.GetCategories());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryInterface.CategoryExists(categoryId))
                return NotFound();

            var category = _mapper.Map<CategoryDto>(_categoryInterface.GetCategory(categoryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(category);
        }

        [HttpGet("ware/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Ware))]
        [ProducesResponseType(400)]
        public IActionResult GetWareByCategory(int categoryId)
        {
            var wares = _mapper.Map<List<WareDto>>(_categoryInterface.GetWareByCategory(categoryId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(wares);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            if (categoryCreate == null)
                return BadRequest(ModelState);

            var category = _categoryInterface.GetCategories()
                .Where(c=> c.Name == categoryCreate.Name.TrimEnd().ToUpper()).FirstOrDefault();
            if (category != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryMap = _mapper.Map<Category>(categoryCreate);

            if (!_categoryInterface.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updatedCategory)
        {
            if(updatedCategory == null)
                return BadRequest(ModelState);

            if(categoryId != updatedCategory.Id)
                return BadRequest(ModelState);

            if (!_categoryInterface.CategoryExists(categoryId))
                return NotFound();

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryMap = _mapper.Map<Category>(updatedCategory);

            if (!_categoryInterface.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryInterface.CategoryExists(categoryId))
                return NotFound();

            var categoryToDelete = _categoryInterface.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var waresOfACategory = _categoryInterface.GetWareByCategory(categoryId).ToList();

            if (!_wareInterface.DeleteWares(waresOfACategory))
                ModelState.AddModelError("", "Something went wrong deleting wares of a category");

            if (!_categoryInterface.DeleteCategory(categoryToDelete))
                ModelState.AddModelError("", "Something went wrong deleting category");

            return Ok("Successfully deleted");
        }
    }
}
