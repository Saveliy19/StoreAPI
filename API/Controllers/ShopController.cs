using API.Models;
using BLL.Infrasructure;
using DAL.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        
        private IStoreService _storeService;

        public ShopController(IStoreService storeService) { _storeService = storeService; }


        [HttpPost]
        [ProducesResponseType(typeof(Models.NewStore), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateStore([FromBody] NewStore newStore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _storeService.CreateStore(new BLL.DTO.Store() { Name = newStore.Name, Address = newStore.Address });
                return Ok(newStore);
            }

            catch (Exception ex) 
            {
                return StatusCode(500, "An error occurred on the server.");
            }

        }

        [HttpGet("{storeId}/Assortment/{cache}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetAffordableAssortment(int storeId, int cache)
        {
            try
            {
                var bllStore= _storeService.CalculateAffordableItems(new BLL.DTO.Store() { Id = storeId }, cache);
                var products = new List<Product>();
                foreach (var item in bllStore.Products)
                {
                    var product = new Product() { Name = item.Name, Cost = item.Cost, Quantity = item.Quantity };
                    products.Add(product);
                }
                return Ok(new AffordableProducts() { Products = products, StoreId = bllStore.Id});
            }

            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred on the server.");
            }
        }

        [HttpPatch("Restock")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AddStoreAssortment([FromBody] AddStoreAssortment storeAssortment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var bllProducts = new List<BLL.DTO.Product>();
                foreach (var product in storeAssortment.Products) 
                {
                    var bllProduct = new BLL.DTO.Product() { Name = product.Name, Cost = product.Cost, Quantity = product.Quantity };
                    bllProducts.Add(bllProduct);
                }
                _storeService.AddProductsToStore(new BLL.DTO.Store() { Id = storeAssortment.StoreId, Products = bllProducts });
                return StatusCode(201);
            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred on the server.");
            }
        }


        [HttpPatch("Purchase")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult RemoveStoreAssortment([FromBody] DeleteStoreAssortment storeAssortment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var bllProducts = new List<BLL.DTO.Product>();
                foreach (var product in storeAssortment.Products)
                {
                    var bllProduct = new BLL.DTO.Product() { Name = product.Name, Quantity = product.Quantity };
                    bllProducts.Add(bllProduct);
                }
                int summ = _storeService.DeleteProductsFromStore(new BLL.DTO.Store() { Id = storeAssortment.StoreId, Products = bllProducts });
                return Ok(summ);
            }

            catch (ProductUnavailableException) 
            {
                return StatusCode(404, "Не все продукты имеются в достаточном количестве");
            }

            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred on the server.");
            }
        }

    }
}
