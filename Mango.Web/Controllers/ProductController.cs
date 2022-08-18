using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            List<ProductDto> products = new();
            var responce = await _productService.GetAllProductsAsync<ResponceDto>(token);
            if (responce != null && responce.IsSuccess) 
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responce.Result));
            }

            return View(products);
        }

        [HttpGet]
        public IActionResult ProductCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid) 
            {
                var token = await HttpContext.GetTokenAsync("access_token");

                var responce = await _productService.CreateProductAsync<ResponceDto>(model, token);

                if (responce != null && responce.IsSuccess)
                {
                    return RedirectToAction("ProductIndex");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProductEdit(int productId)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var responce = await _productService.GetProductByIdAsync<ResponceDto>(productId,token);
            if (responce != null && responce.IsSuccess) 
            {
                var product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responce.Result));
                return View(product);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var token = await HttpContext.GetTokenAsync("access_token");

                var responce = await _productService.UpdateProductAsync<ResponceDto>(model, token);

                if (responce != null && responce.IsSuccess)
                {
                    return RedirectToAction("ProductIndex");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProductDelete(int productId)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var result = await _productService.GetProductByIdAsync<ResponceDto>(productId, token);
            if (result != null && result.IsSuccess) 
            {
                var product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(result.Result));
                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var responce = await _productService.DeleteProductAsync<ResponceDto>(model.ProductId, token);

            if (responce.IsSuccess)
            {
                return RedirectToAction("ProductIndex");
            }

            return NotFound();
        }

    }
}
