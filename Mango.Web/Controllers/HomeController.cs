using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService,
            ICartService cartService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();

            var responce = await _productService.GetAllProductsAsync<ResponceDto>("");

            if(responce != null && responce.IsSuccess) 
            {
                 list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responce.Result));
                return View(list);
            }

            return NotFound();

        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            ProductDto product = new();

            var responce = await _productService.GetProductByIdAsync<ResponceDto>(productId, token);

            if (responce != null && responce.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responce.Result));
                return View(product);
            }

            return NotFound();

        }

        [HttpPost]
        [Authorize]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            CartDto cartDto = new()
            {
                CartHeader = new CartHeaderDto()
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault().Value
                }
            };

            CartDetailsDto cartDetails = new CartDetailsDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };

            var resp = await _productService.GetProductByIdAsync<ResponceDto>(productDto.ProductId, "");

            if (resp != null && resp.IsSuccess) 
            {
                cartDetails.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(resp.Result));
            }
            List<CartDetailsDto> cartDetailsDtos = new();
            cartDetailsDtos.Add(cartDetails);
            cartDto.CartDetails = cartDetailsDtos;

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var addToCartResp = await _cartService.AddToCartAsync<ResponceDto>(cartDto, accessToken);
            if (addToCartResp != null && addToCartResp.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(productDto);
        }

        [Authorize]
        public ActionResult LoginAsync()
        {
            return RedirectToAction("Index");
        }
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}