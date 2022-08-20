using AutoMapper;
using Mango.Services.ShoppingCart.API.DbContexts;
using Mango.Services.ShoppingCart.API.Models;
using Mango.Services.ShoppingCart.API.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCart.API.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public CartRepository(ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            var cardFromDb = await _dbContext.CartHeaders
                .FirstOrDefaultAsync(u => u.UserId == userId);
            cardFromDb.CouponCode = couponCode;
            _dbContext.CartHeaders.Update(cardFromDb);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await _dbContext.CartHeaders.FirstOrDefaultAsync(u =>
            u.UserId == userId);

            if (cartHeaderFromDb != null)
            {
                _dbContext.CartDetails.RemoveRange(
                    _dbContext.CartDetails.Where(u =>
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId));
                _dbContext.CartHeaders.Remove(cartHeaderFromDb);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto);

            var productInDb = await _dbContext.Products
                .FirstOrDefaultAsync(x =>
                x.ProductId == cartDto.CartDetails
                .FirstOrDefault().ProductId);

            if (productInDb == null) 
            {
                _dbContext.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _dbContext.SaveChangesAsync();
            }

            var cartHeaderFromDb = await _dbContext.CartHeaders.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == cart.CartHeader.UserId);

            if (cartHeaderFromDb == null)
            {
                _dbContext.CartHeaders.Add(cart.CartHeader);
                await _dbContext.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _dbContext.SaveChangesAsync();
            }
            else 
            {
                var CartDetailsFromDb = await _dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(u =>
                u.ProductId == cart.CartDetails.FirstOrDefault().ProductId && 
                u.CartHeaderId == cartHeaderFromDb.CartHeaderId);


                if (CartDetailsFromDb == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }
                else 
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += CartDetailsFromDb.Count;
                    _dbContext.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }
            }
            return _mapper.Map<CartDto>(cart);

        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _dbContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId)
            };

            cart.CartDetails = _dbContext.CartDetails
                .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId).Include(u => u.Product);

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cardFromDb = await _dbContext.CartHeaders
                .FirstOrDefaultAsync(u => u.UserId == userId);

            cardFromDb.CouponCode = "";
            _dbContext.CartHeaders.Update(cardFromDb);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _dbContext.CartDetails
                    .FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);

                int totalCountOfCartItems = _dbContext.CartDetails
                    .Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();

                _dbContext.CartDetails.Remove(cartDetails);
                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _dbContext.CartHeaders
                        .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _dbContext.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
