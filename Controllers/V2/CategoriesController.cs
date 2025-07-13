using Asp.Versioning;
using AutoMapper;
using cs_apiEcommerce.Constants;
using cs_apiEcommerce.Models.Dtos;
using cs_apiEcommerce.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace cs_apiEcommerce.Controllers.V2
{
//* Add API versioning in route
    //* Before [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //* Add API versions
    [ApiVersion("2.0")]
    [ApiController]
    //? Enable CORS at Controller level
    // [EnableCors(PolicyNames.AllowSpecificOrigin)]
    //* Authorization
    [Authorize(Roles = "Admin")]
    public class CategoriesController(ICategoryRepository categoryRepository, IMapper mapper) : ControllerBase
    {
        private readonly ICategoryRepository _categoryRespository = categoryRepository;

        private readonly IMapper _mapper = mapper;

       
        //* Duplicate of Getcategories and renamed to getcategoriesbyId implemented in V2
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //? Enable CORS at method level
        // [EnableCors(PolicyNames.AllowSpecificOrigin)]
        //* We can indicate to which API version the method correspond
        // [MapToApiVersion("2.0")]
        public IActionResult GetCategoriesById()
        {
            ICollection<Category> categories = [.. _categoryRespository.GetCategories().OrderBy(cat => cat.Id)];

            List<CategoryDto> categoriesDto = [];
            foreach (var c in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(c));
            }

            return Ok(categoriesDto);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetCategory")]
        //? Add cache response by Duration and by Profile name
        // [ResponseCache(Duration = 10)]
        [ResponseCache(CacheProfileName = CacheProfiles.Default10)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategory(int id)
        {
            //* Verify Cache implementatoin
            System.Console.WriteLine($"Category with the ID: {id} at {DateTime.Now}");
            Category? category = _categoryRespository.GetCategory(id);
            System.Console.WriteLine($"Response with the ID: {id}");

            if (category == null) return NotFound($"The category with Id {id} doesn't exist.");

            CategoryDto categoryDto = _mapper.Map<CategoryDto>(category);

            return Ok(categoryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null) return BadRequest(ModelState);
            if (_categoryRespository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "The Category already exists");
                return BadRequest(ModelState);
            }

            Category category = _mapper.Map<Category>(createCategoryDto);

            if (!_categoryRespository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Somehting went wrong when creating category {category.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }

        [HttpPatch("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
        {
            if (!_categoryRespository.CategoryExists(id))
            {
                return NotFound($"The category with Id {id} doesn't exist.");
            }

            if (updateCategoryDto == null) return BadRequest(ModelState);
            if (_categoryRespository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError("Custom Error", "The Category already exists");
                return BadRequest(ModelState);
            }
            Category category = _mapper.Map<Category>(updateCategoryDto);
            category.Id = id;
            if (!_categoryRespository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when updating the registry {category.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult DeleteCategory(int id)
        {
            if (!_categoryRespository.CategoryExists(id))
            {
                return NotFound($"The category with Id {id} doesn't exist.");
            }

            Category? category = _categoryRespository.GetCategory(id);
            if (category == null)
            {
                return NotFound($"The category with Id {id} doesn't exist.");
            }

            if (!_categoryRespository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when deleting registry {category.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
