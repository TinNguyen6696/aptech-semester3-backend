using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Data.Seeders
{
    public static class CommunitySeeder
    {
        public static Community[] GetCommunities()
        {
            var seed = new DateTime(2024, 1, 1);
            const int adminUserId = 1;

            return new[]
            {
                new Community { Id = 1, Name = "Singers",      Category = TalentCategory.Singer,      CreatedByUserId = adminUserId, CreatedAt = seed, UpdatedAt = seed },
                new Community { Id = 2, Name = "Dancers",      Category = TalentCategory.Dancer,      CreatedByUserId = adminUserId, CreatedAt = seed, UpdatedAt = seed },
                new Community { Id = 3, Name = "Artists",      Category = TalentCategory.Artist,      CreatedByUserId = adminUserId, CreatedAt = seed, UpdatedAt = seed },
                new Community { Id = 4, Name = "Designers",    Category = TalentCategory.Designer,    CreatedByUserId = adminUserId, CreatedAt = seed, UpdatedAt = seed },
                new Community { Id = 5, Name = "Coders",       Category = TalentCategory.Coder,       CreatedByUserId = adminUserId, CreatedAt = seed, UpdatedAt = seed },
                new Community { Id = 6, Name = "Photographers", Category = TalentCategory.Photographer, CreatedByUserId = adminUserId, CreatedAt = seed, UpdatedAt = seed }
            };
        }
    }
}