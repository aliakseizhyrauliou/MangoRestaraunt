using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService,
            IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public async Task<IActionResult> CartIndex()
        {
            var cart = await LoadCartDtoBasedOnLoggedInUser();
            return View(cart);
        }

        public async Task<IActionResult> Remove(int cartDetailsId) 
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault().Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var _responce = await _cartService.RemoveFromCartAsync<ResponceDto>(cartDetailsId, accessToken);

            if (_responce != null && _responce.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser() 
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault().Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var _responce = await _cartService.GetCartByUserIdAsync<ResponceDto>(userId, accessToken);
            CartDto cartDto = new();
            if (_responce != null && _responce.IsSuccess) 
            {
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(_responce.Result));
            }

            if (cartDto.CartHeader != null) 
            {
                foreach (var product in cartDto.CartDetails) 
                {
                    cartDto.CartHeader.OrderTotal += (product.Product.Price * product.Count);
                }
            }

            return cartDto;
        }
    }
}
