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
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateStore([FromBody] NewStore newStore)
        {
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
    }
}
