using Mango.Services.ShoppingCart.API.Models.Dto;
using Mango.Services.ShoppingCart.API.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCart.API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        protected ResponceDto _responce;
        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            this._responce = new ResponceDto();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserId(userId);
                _responce.Result = cart;
            }
            catch(Exception ex) 
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string>() { ex.Message };
            }

            return _responce;
        }

        [HttpGet("ClearCart/{userId}")]
        public async Task<object> ClearCart(string userId) 
        {
            try 
            {
                var result = await _cartRepository.ClearCart(userId);
                _responce.Result = result;
            }
            catch(Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string>() { ex.Message };
            }

            return _responce;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart([FromBody]CartDto cartDto)
        {
            try
            {
                var cart = await _cartRepository.CreateUpdateCart(cartDto);
                _responce.Result = cart;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string>() { ex.Message };
            }

            return _responce;
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart([FromBody]CartDto cartDto)
        {
            try
            {
                var cart = await _cartRepository.CreateUpdateCart(cartDto);
                _responce.Result = cart;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string>() { ex.Message };
            }

            return _responce;
        }

        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody]int cardId)
        {
            try
            {
                var result = await _cartRepository.RemoveFromCart(cardId);
                _responce.Result = result;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string>() { ex.Message };
            }

            return _responce;
        }




    }
}
