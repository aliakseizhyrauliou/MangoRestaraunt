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
        private readonly ICouponService _couponService;


        public CartController(ICartService cartService,
            IProductService productService,
            ICouponService couponService)
        {
            _couponService = couponService;
            _cartService = cartService;
            _productService = productService;
        }

        public async Task<IActionResult> CartIndex()
        {
            var cart = await LoadCartDtoBasedOnLoggedInUser();
            return View(cart);
        }
        [HttpGet]
        public async Task<IActionResult> Checkout() 
        {
            var cart = await LoadCartDtoBasedOnLoggedInUser();
            return View(cart);
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto) 
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var responce = await _cartService.ApplyCoupon<ResponceDto>(cartDto, token);

            if (responce != null && responce.IsSuccess) 
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var responce = await _cartService.RemoveCoupon<ResponceDto>(cartDto.CartHeader.UserId, token);

            if (responce != null && responce.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
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
                if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode)) 
                {
                    var couponResp = await _couponService.GetCoupon<ResponceDto>(cartDto.CartHeader.CouponCode, accessToken);
                    if (couponResp != null && couponResp.IsSuccess) 
                    {
                        var coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(couponResp.Result));
                        cartDto.CartHeader.DiscountTotal = coupon.DiscountAmount;
                    }
                }
                foreach (var product in cartDto.CartDetails) 
                {
                    cartDto.CartHeader.OrderTotal += (product.Product.Price * product.Count);
                }

                cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
            }

            return cartDto;
        }
    }
}
