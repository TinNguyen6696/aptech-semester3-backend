using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class JwtDenylistRepository : GenericRepository<JwtDenylist>, IJwtDenylistRepository
    {
        public JwtDenylistRepository(AppDbContext context) : base(context) { }

        public async Task<bool> ExistsAsync(string jti) =>
            await _dbSet.AnyAsync(j => j.Jti == jti);
    }
}