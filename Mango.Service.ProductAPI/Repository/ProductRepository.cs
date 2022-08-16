using AutoMapper;
using Mango.Service.ProductAPI.DbContexts;
using Mango.Service.ProductAPI.Models;
using Mango.Service.ProductAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Service.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContex _dbContext;
        private IMapper _mapper;

        public ProductRepository(ApplicationDbContex contex,
            IMapper mapper)
        {
            _dbContext = contex; 
            _mapper = mapper;
        }
        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            if (product.ProductId > 0)
            {
                _dbContext.Products.Update(product);
            }
            else 
            {
                _dbContext.Products.Add(product);
            }
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(product => product.ProductId == productId);
                if (product is null)
                {
                    return false;
                }
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception) 
            {
                return false;
            }
        }

        public async Task<ProductDto> GetProductById(int productId)
        {
            var product = await _dbContext.Products
                .SingleOrDefaultAsync(product => product.ProductId == productId);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var productsList = await _dbContext.Products.ToListAsync();

            return _mapper.Map<List<ProductDto>>(productsList);
        }
    }
}
