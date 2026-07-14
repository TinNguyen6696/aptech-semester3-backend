using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Data.Seeders
{
    public static class AdminSeeder
    {
        // Demo/grading-only account. Hash is for "Admin@123", generated once with
        // BCrypt.Net.BCrypt.HashPassword (work factor 11, this package's default) and
        // hardcoded here on purpose — regenerating it on every build would make EF think
        // the seed data changed and want a new migration each time.
        private const string AdminPasswordHash = "$2a$11$U1DkzARGayfW6Z8NWlKRmOotkT4qnhWfP1jW2dHuPStbOjgaNC2c2";

        public static User GetUser()
        {
            var seed = new DateTime(2024, 1, 1);

            return new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@talentshowcase.com",
                PasswordHash = AdminPasswordHash,
                Role = UserRole.Admin,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = seed,
                UpdatedAt = seed
            };
        }

        public static UserProfile GetProfile()
        {
            var seed = new DateTime(2024, 1, 1);

            return new UserProfile
            {
                Id = 1,
                UserId = 1,
                FirstName = "Admin",
                LastName = "User",
                SkillLevel = SkillLevel.Advanced,
                PrimaryCategory = TalentCategory.Coder,
                ProvinceId = 1,
                CreatedAt = seed,
                UpdatedAt = seed
            };
        }
    }
}
