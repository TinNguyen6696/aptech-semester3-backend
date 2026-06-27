using Microsoft.EntityFrameworkCore;
using TaLentShowcase.API.Infrastructure.Persistence;
using TaLentShowcase.API.Models.Entities;
using TaLentShowcase.API.Repositories.Interfaces;

namespace TaLentShowcase.API.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ApplicationDbContext _context;

    public ProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetProfileForReadAsync(int userId)
    {
        return await _context.Users
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Province)
            .Include(x => x.UserProfile)
            .Include(x => x.UserTalents)
                .ThenInclude(x => x.Talent)
            .Include(x => x.Achievements)
            .Include(x => x.Awards)
            .Include(x => x.Certifications)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<User?> GetUserWithProfileAsync(int userId)
    {
        return await _context.Users
            .Include(x => x.UserProfile)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<bool> UserExistsAsync(int userId)
    {
        return await _context.Users.AnyAsync(x => x.Id == userId);
    }

    public async Task<bool> UserTalentExistsAsync(int userId, int talentId)
    {
        return await _context.UserTalents
            .AnyAsync(x => x.UserId == userId && x.TalentId == talentId);
    }

    public async Task<Talent?> GetTalentAsync(int talentId)
    {
        return await _context.Talents.FirstOrDefaultAsync(x => x.Id == talentId);
    }

    public async Task<UserTalent?> GetUserTalentAsync(int userId, int userTalentId)
    {
        return await _context.UserTalents
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == userTalentId);
    }

    public async Task<Achievement?> GetAchievementAsync(int userId, int achievementId)
    {
        return await _context.Achievements
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == achievementId);
    }

    public async Task<Award?> GetAwardAsync(int userId, int awardId)
    {
        return await _context.Awards
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == awardId);
    }

    public async Task<Certification?> GetCertificationAsync(int userId, int certificationId)
    {
        return await _context.Certifications
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == certificationId);
    }

    public async Task AddUserTalentAsync(UserTalent userTalent)
    {
        await _context.UserTalents.AddAsync(userTalent);
    }

    public async Task AddAchievementAsync(Achievement achievement)
    {
        await _context.Achievements.AddAsync(achievement);
    }

    public async Task AddAwardAsync(Award award)
    {
        await _context.Awards.AddAsync(award);
    }

    public async Task AddCertificationAsync(Certification certification)
    {
        await _context.Certifications.AddAsync(certification);
    }

    public async Task ClearPrimaryUserTalentsAsync(int userId)
    {
        await _context.UserTalents
            .Where(x => x.UserId == userId && x.IsPrimary)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsPrimary, false));
    }

    public void RemoveUserTalent(UserTalent userTalent)
    {
        _context.UserTalents.Remove(userTalent);
    }

    public void RemoveAchievement(Achievement achievement)
    {
        _context.Achievements.Remove(achievement);
    }

    public void RemoveAward(Award award)
    {
        _context.Awards.Remove(award);
    }

    public void RemoveCertification(Certification certification)
    {
        _context.Certifications.Remove(certification);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
