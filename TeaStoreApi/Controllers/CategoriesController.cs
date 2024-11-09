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
    public class CategoriesController : ControllerBase
    {

        private ICategoryRepository categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
          var categories= await categoryRepository.GetCategories();
            if (categories.Any())
            {
                return Ok(categories);
            }
            return NotFound();
        }


        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm]Category category)
        {
            var isAdded =await categoryRepository.AddCategory(category);
            if(isAdded)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest("Someting went wrong");

        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await categoryRepository.DeleteCategory(id);
            if(isDeleted)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }
    }
}
