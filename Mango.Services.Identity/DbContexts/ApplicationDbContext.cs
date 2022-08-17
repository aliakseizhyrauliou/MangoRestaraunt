using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.Identity.DbContexts
{
    public class ApplicationDbContex : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContex(DbContextOptions<ApplicationDbContex> options) : base(options)
        {

        }
    }
}
