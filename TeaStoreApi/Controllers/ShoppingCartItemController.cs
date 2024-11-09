using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeaStoreApi.Interfaces;
using TeaStoreApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeaStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShoppingCartItemController : ControllerBase
    {
        private IShoppingCartRepository shoppingCartRepository;
        public ShoppingCartItemController(IShoppingCartRepository shoppingCartRepository)
        {
            this.shoppingCartRepository = shoppingCartRepository;
        }

        // GET: api/<ShoppingCartItemController>
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var shoppingCartItem = await shoppingCartRepository.GetShoppingCartItems(userId);
            if(shoppingCartItem.Any())
            {
                return Ok(shoppingCartItem);
            }
            return NotFound();
        }



        // POST api/<ShoppingCartItemController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ShoppingCartItem shoppingCartItem)
        {
            var isAdded = await shoppingCartRepository.AddToCart(shoppingCartItem);
            if(isAdded) 
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest("something went wrong");
        }

        // PUT api/<ShoppingCartItemController>/5
        [HttpPut]
        public async Task<IActionResult> Put(int productId, int userId,string action)
        {
            var isUpdated = await shoppingCartRepository.UpdateCart(productId, userId, action);
            if( isUpdated)
            {
                return Ok("cart updated");
            }
            return BadRequest("something went wrong");
        }


    }
}
