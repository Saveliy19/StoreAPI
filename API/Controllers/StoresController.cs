﻿using API.Models;
using BLL.Exceptions;
using BLL.Infrasructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {

        private IStoreService _storeService;

        public StoresController(IStoreService storeService) { _storeService = storeService; }


        [HttpGet]
        [ProducesResponseType(500)]
        [ProducesResponseType(typeof(List<Models.Store>), 200)]

        public IActionResult GetStores() 
        {
            try
            {
                var stores = new List<Store>();

                var bllStores = _storeService.GetStores();

                foreach (var store in bllStores)
                {
                    var new_store = new Store() { Id = store.Id, Name = store.Name, Address = store.Address };
                    stores.Add(new_store);
                }

                return Ok(stores);
            }
            catch (Exception) { return StatusCode(500, "An error occurred on the server."); }
        }

        [HttpGet("{storeId}/Assortment")]
        [ProducesResponseType(typeof(Models.StoreAssortment), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetStoreAssortment(int storeId)
        {
            try
            {
                var bllAssortment = _storeService.GetStoreAssortment(new BLL.DTO.Store() { Id = storeId });

                var products = new List<Product>();


                foreach (var bllProduct in bllAssortment.Products)
                {
                    var product = new Product() { Name = bllProduct.Name, Cost = bllProduct.Cost, Quantity = bllProduct.Quantity };
                    products.Add(product);
                }

                return Ok(new StoreAssortment() { Products = products, Id = bllAssortment.Id });
            }

            catch (StoreNotExistException ex) { return NotFound(ex.Message); }

            catch (Exception) { return StatusCode(500, "An error occurred on the server."); }
        }


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

            catch (AlreadyExistException ex) { return BadRequest(ex.Message); }

            catch (Exception) { return StatusCode(500, "An error occurred on the server."); }

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
                var bllStore = _storeService.CalculateAffordableItems(new BLL.DTO.Store() { Id = storeId }, cache);
                var products = new List<Product>();
                foreach (var item in bllStore.Products)
                {
                    var product = new Product() { Name = item.Name, Cost = item.Cost, Quantity = item.Quantity };
                    products.Add(product);
                }
                return Ok(new AffordableProducts() { Products = products, StoreId = bllStore.Id });
            }

            catch (StoreNotExistException ex) { return NotFound(ex.Message); }

            catch (Exception ex) { return StatusCode(500, "An error occurred on the server."); }
        }

        [HttpPatch("{storeId}/Assortment/Restock")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AddStoreAssortment(int storeId, [FromBody] AddStoreAssortment storeAssortment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (storeId < 0) { return BadRequest("storeId must be a natural number"); }

            try
            {
                var bllProducts = new List<BLL.DTO.Product>();
                foreach (var product in storeAssortment.Products)
                {
                    var bllProduct = new BLL.DTO.Product() { Name = product.Name, Cost = product.Cost, Quantity = product.Quantity };
                    bllProducts.Add(bllProduct);
                }
                _storeService.AddProductsToStore(new BLL.DTO.Store() { Id = storeId, Products = bllProducts });
                return Ok(storeAssortment);
            }

            catch (Exception ex) when (ex is StoreNotExistException || ex is ProductNotExistException)
            {
                return NotFound(ex.Message);
            }

            catch (Exception) { return StatusCode(500, "An error occurred on the server."); }
        }


        [HttpPatch("{storeId}/Assortment/Purchase")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult RemoveStoreAssortment(int storeId, [FromBody] DeleteStoreAssortment storeAssortment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (storeId < 0) { return BadRequest("storeId must be a natural number"); }

            try
            {
                var bllProducts = new List<BLL.DTO.Product>();
                foreach (var product in storeAssortment.Products)
                {
                    var bllProduct = new BLL.DTO.Product() { Name = product.Name, Quantity = product.Quantity };
                    bllProducts.Add(bllProduct);
                }
                int summ = _storeService.DeleteProductsFromStore(new BLL.DTO.Store() { Id = storeId, Products = bllProducts });
                return Ok(summ);
            }

            catch (Exception ex) 
            when (ex is StoreNotExistException || ex is ProductUnavailableException || ex is ProductNotExistException)
            {
                return NotFound(ex.Message);
            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred on the server.");
            }
        }

    }
}
