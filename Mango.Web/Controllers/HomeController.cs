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
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
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

        [Authorize]
        public ActionResult ginAsync()
        {
            return RedirectToAction("Index");
        }
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}