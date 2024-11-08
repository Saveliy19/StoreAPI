using API.Models;
using BLL.Infrasructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IStoreService _storeService;

        public ProductController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
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
                return StatusCode(500, "An error occurred on the server.");
            }
        }


        [HttpGet("cheapest-store")]
        [ProducesResponseType(typeof(Models.CheapestLocation), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetCheapestStore([FromQuery] Dictionary<string, int> products)
        {
            if (products == null || products.Count == 0)
            {
                return BadRequest("Parameter 'items' is required.");
            }

            try
            {
                var bllProducts = products.Select(product =>
                    new BLL.DTO.Product { Name = product.Key, Quantity = product.Value }).ToList();

                var cheapestStore = _storeService.GetBestPriceLocation(bllProducts);

                if (cheapestStore == null)
                {
                    return NotFound("No store has all requested products.");
                }

                var response = new Models.CheapestLocation
                {
                    PriceSumm = cheapestStore.PriceSumm,
                    Store = new Models.Store
                    {
                        Id = cheapestStore.Store.Id,
                        Name = cheapestStore.Store.Name,
                        Address = cheapestStore.Store.Address
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while fetching the cheapest store.");
                return StatusCode(500, "An error occurred on the server.");
            }
        }


    }
}
