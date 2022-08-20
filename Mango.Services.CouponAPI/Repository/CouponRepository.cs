using Mango.Services.CouponAPI.Models.Dtos;
using Mango.Services.CouponAPI.DbContexts;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public CouponRepository(ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public async Task<CouponDto> GetCouponByCode(string code)
        {
            var couponFromDb = await _dbContext.Coupons
                .SingleOrDefaultAsync(c => c.CouponCode == code);

            var couponDto = _mapper.Map<CouponDto>(couponFromDb);

            return couponDto;
        }
    }
}
