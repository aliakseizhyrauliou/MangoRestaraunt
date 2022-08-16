using Mango.Web.Models;
using Mango.Web.Services.IServices;
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
            List<ProductDto> products = new();
            var responce = await _productService.GetAllProductsAsync<ResponceDto>();
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
                var responce = await _productService.CreateProductAsync<ResponceDto>(model);

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
            var responce = await _productService.GetProductByIdAsync<ResponceDto>(productId);
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
                var responce = await _productService.UpdateProductAsync<ResponceDto>(model);

                if (responce != null && responce.IsSuccess)
                {
                    return RedirectToAction("ProductIndex");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDelete(int productId)
        {
            var result = await _productService.GetProductByIdAsync<ResponceDto>(productId);
            if (result != null && result.IsSuccess) 
            {
                var product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(result.Result));
                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            
            var responce = await _productService.DeleteProductAsync<ResponceDto>(model.ProductId);

            if (responce.IsSuccess)
            {
                return RedirectToAction("ProductIndex");
            }

            return NotFound();
        }

    }
}
