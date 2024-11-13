﻿using API.Models;
using BLL.Infrasructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IStoreService _storeService;

        public ProductsController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateProduct([FromBody] NewProduct product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

        [HttpGet]
        [ProducesResponseType(typeof(List<Models.NewProduct>), 200)]
        [ProducesResponseType(500)]
        public IActionResult GetAllProducts() 
        {
            try
            {
                var products = new List<Models.NewProduct>();

                var bllProducts = _storeService.GetProducts();

                foreach (var product in bllProducts) 
                {
                    var new_product = new Models.NewProduct { Name = product.Name };
                    products.Add(new_product);
                }

                return Ok(products);
            }

            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred on the server.");
            }
        }


        [HttpGet("Stores/Cheapest")]
        [ProducesResponseType(typeof(Models.CheapestLocation), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetCheapestStore([FromQuery] Dictionary<string, int> products)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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