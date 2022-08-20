using Mango.Services.CouponAPI.Models.Dtos;
using Mango.Services.CouponAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/coupon")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        protected ResponceDto _responce;
        public CouponController(ICouponRepository cartRepository)
        {
            _couponRepository = cartRepository;
            this._responce = new ResponceDto();
        }

        [HttpGet("{code}")]
        public async Task<object> GetDiscoutForCode(string code)
        {
            try
            {
                var coupon = await _couponRepository.GetCouponByCode(code);
                _responce.Result = coupon;
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
