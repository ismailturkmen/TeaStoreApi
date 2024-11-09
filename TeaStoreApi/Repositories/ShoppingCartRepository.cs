using Microsoft.EntityFrameworkCore;
using TeaStoreApi.Data;
using TeaStoreApi.Interfaces;
using TeaStoreApi.Models;

namespace TeaStoreApi.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private ApiDbContext dbcontext;
        public ShoppingCartRepository(ApiDbContext dbContext)
        {
            this.dbcontext = dbContext;
        }

        public async Task<bool> AddToCart(ShoppingCartItem shoppingCartItem)
        {
            var product = await dbcontext.Products.FindAsync(shoppingCartItem.ProductId);
            if (product == null)
            {
                return false;
            }

            //check if item is already in the cart 
            var existingCartItem = await dbcontext.ShoppingCartItems.FirstOrDefaultAsync(s => s.ProductId == shoppingCartItem.ProductId && s.UserId == shoppingCartItem.UserId);
            if (existingCartItem != null)
            {
                //item already exists, update quantity and total amount
                existingCartItem.Qty = shoppingCartItem.Qty;
                existingCartItem.TotalAmount = shoppingCartItem.Price * existingCartItem.Qty;
            }
            else
            {
                // item doesnt exist add new items to the cart
                shoppingCartItem.Price = product.Price;
                shoppingCartItem.TotalAmount = product.Price * shoppingCartItem.Qty;
                await dbcontext.ShoppingCartItems.AddAsync(shoppingCartItem);
            }
            await dbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<object>> GetShoppingCartItems(int userId)
        {
            var shoppingCart = await (from shoppingCartItems in dbcontext.ShoppingCartItems.Where(x => x.UserId == userId)
                                      join product in dbcontext.Products on shoppingCartItems.ProductId equals product.Id
                                      select new
                                      {
                                          ProductId = product.Id,
                                          ProductName = product.Name,
                                          ImageUrl = product.ImageUrl,
                                          Price = shoppingCartItems.Price,
                                          TotalAmount = shoppingCartItems.TotalAmount,
                                          Qty = shoppingCartItems.Qty,
                                      }).ToListAsync();
            return shoppingCart;
        }

        public async Task<bool> UpdateCart(int productId, int userId, string action)
        {
            var existingCartItem = await dbcontext.ShoppingCartItems.FirstOrDefaultAsync(s => s.ProductId == productId && s.UserId == userId);
            if (existingCartItem == null)
            {
                return false;
            }
            switch (action)
            {
                case "increase":
                    existingCartItem.Qty += 1;
                    existingCartItem.TotalAmount = existingCartItem.Price *existingCartItem.Qty;
                    break;
                case "decrease":
                    if(existingCartItem.Qty > 0)
                    {
                        existingCartItem.Qty-= 1;
                        existingCartItem.TotalAmount = existingCartItem.Price*existingCartItem.Qty;
                    }
                    else
                    {
                        dbcontext.ShoppingCartItems.Remove(existingCartItem);
                    }
                    break;
                case "delete":
                    dbcontext.ShoppingCartItems.Remove(existingCartItem);
                    break;
                    default: return false;

            }
            await dbcontext.SaveChangesAsync();
            return true;

        }
    }
}
