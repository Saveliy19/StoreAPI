using API.Models;
using BLL.Infrasructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IStoreService _storeService;

        public ProductController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] NewProduct product)
        {
            if (product == null || string.IsNullOrEmpty(product.Name))
            {
                return BadRequest("Необходмо название продукта.");
            }

            try
            {
                _storeService.CreateProduct(new BLL.DTO.Product { Name = product.Name });
                return Ok(product);
            }

            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при создании продукта.");
            }
        }


        [HttpGet("cheapest-store")]

        public IActionResult GetCheapestStore([FromQuery] Dictionary<string, int> items)
        {
            if (items == null || items.Count == 0)
            {
                return BadRequest("'items' required.");
            }

            return Ok(new
            {
                
            });
        }



    }
}
