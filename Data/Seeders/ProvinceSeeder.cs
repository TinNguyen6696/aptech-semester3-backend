using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Seeders
{
    public static class ProvinceSeeder
    {
        public static Province[] GetProvinces()
        {
            var seed = new DateTime(2024, 1, 1);

            return new[]
            {
                // 6 centrally-run cities
                new Province { Id = 1,  Name = "Hanoi",            CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 2,  Name = "Ho Chi Minh City", CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 3,  Name = "Hai Phong",        CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 4,  Name = "Da Nang",          CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 5,  Name = "Can Tho",          CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 6,  Name = "Hue",              CreatedAt = seed, UpdatedAt = seed },

                // 28 provinces
                new Province { Id = 7,  Name = "Cao Bang",     CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 8,  Name = "Dien Bien",    CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 9,  Name = "Ha Tinh",      CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 10, Name = "Lai Chau",     CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 11, Name = "Lang Son",     CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 12, Name = "Nghe An",      CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 13, Name = "Quang Ninh",   CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 14, Name = "Thanh Hoa",    CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 15, Name = "Son La",       CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 16, Name = "Tuyen Quang",  CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 17, Name = "Lao Cai",      CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 18, Name = "Thai Nguyen",  CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 19, Name = "Phu Tho",      CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 20, Name = "Bac Ninh",     CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 21, Name = "Hung Yen",     CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 22, Name = "Ninh Binh",    CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 23, Name = "Quang Tri",    CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 24, Name = "Quang Ngai",   CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 25, Name = "Gia Lai",      CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 26, Name = "Khanh Hoa",    CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 27, Name = "Lam Dong",     CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 28, Name = "Dak Lak",      CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 29, Name = "Dong Nai",     CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 30, Name = "Tay Ninh",     CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 31, Name = "Vinh Long",    CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 32, Name = "Dong Thap",    CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 33, Name = "Ca Mau",       CreatedAt = seed, UpdatedAt = seed },
                new Province { Id = 34, Name = "An Giang",     CreatedAt = seed, UpdatedAt = seed }
            };
        }
    }
}
