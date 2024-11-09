using API.Models;
using BLL.Infrasructure;
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

        [HttpPatch("Restock")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateStoreAssortment([FromBody] StoreAssortment storeAssortment)
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

            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred on the server.");
            }
        }

        
    }
}
