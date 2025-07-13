using Asp.Versioning;
using AutoMapper;
using cs_apiEcommerce.Models;
using cs_apiEcommerce.Models.Dtos;
using cs_apiEcommerce.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cs_apiEcommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    //* Add API versioning in route
    //* Before [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    // [ApiVersion("1.0")]
    // [ApiVersion("2.0")]
    //* If controller will be neutral there's no need to show the version and can be specified as neutral
    [ApiVersionNeutral]
    public class ProductsController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper) : ControllerBase
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProducts()
        {
            ICollection<Product> products = _productRepository.GetProducts();
            List<ProductDto> productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [AllowAnonymous]
        [HttpGet("{productId:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProduct(int productId)
        {
            Product? product = _productRepository.GetProduct(productId);
            if (product == null) return NotFound($"The product with the id {productId} does not exist");

            ProductDto productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);

        }
        [HttpGet("searchProductByCategory/{categoryId:int}", Name = "GetProductsByCategory")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            ICollection<Product> products = _productRepository.GetProductsByCategory(categoryId);
            if (products.Count == 0) return NotFound($"The products with the category {categoryId} doesn't exist.");

            List<ProductDto> productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }
        [HttpGet("searchProductByName/{searchTerm}", Name = "SearchProducts")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult SearchProducts(string searchTerm)
        {
            ICollection<Product> products = _productRepository.SearchProducts(searchTerm);
            if (products.Count == 0) return NotFound($"The products with the name '{searchTerm}' doesn't exist.");

            List<ProductDto> productDtos = _mapper.Map<List<ProductDto>>(products);
            return Ok(productDtos);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (createProductDto == null)
                return BadRequest(ModelState);

            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", $"The preoduct already exists");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"The category with {createProductDto.CategoryId} does not exist");
                return BadRequest(ModelState);
            }

            Product product = _mapper.Map<Product>(createProductDto);

            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("Custom Error", $"Something went wrong when saving registry {product.Name}");
                return StatusCode(500, ModelState);
            }

            Product? createdProduct = _productRepository.GetProduct(product.ProductId);

            ProductDto productDto = _mapper.Map<ProductDto>(createdProduct);
            return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, productDto);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BuyProduct(string name, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
            {
                return BadRequest("The product name or quantity are is valid.");
            }

            bool foundProduct = _productRepository.ProductExists(name);

            if (!foundProduct) return NotFound($"The product with the name {name} doesn't exist.");

            if (!_productRepository.BuyProduct(name, quantity))
            {
                ModelState.AddModelError("Custom Error", $"Operation to buy product {name} couldn't be completed check stock.");
                return BadRequest(ModelState);
            }

            var units = quantity == 1 ? "unit" : "units";
            return Ok($"{quantity} {units} of product {name} purchased.");
        }

        [HttpPut("{productId:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProduct(int productId, [FromBody] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null) return BadRequest(ModelState);

            if (!_productRepository.ProductExists(productId))
            {
                ModelState.AddModelError("CustomError", "The product doesn't exist");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"The category with the product {updateProductDto.CategoryId} doesn't exist.");
                return BadRequest(ModelState);
            }

            Product product = _mapper.Map<Product>(updateProductDto);
            product.ProductId = productId;

            if (!_productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong whne trying to update registry {product.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{productId:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteProduct(int productId)
        {
            if (productId == 0) return BadRequest(ModelState);

            Product? product = _productRepository.GetProduct(productId);

            if (product == null)
            {
                return NotFound($"The product with Id {productId} doesn't exist");
            }

            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when trying to delete registry {product.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}
