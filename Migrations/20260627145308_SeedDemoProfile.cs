using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaLentShowcase.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedDemoProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                SET NOCOUNT ON;
                DECLARE @Now datetime2 = SYSUTCDATETIME();
                DECLARE @PasswordHash nvarchar(500) = N'PBKDF2$210000$mu5luZUTn74StzSdQq2Fbw==$ynl88nXrWwKuPXLi0iaDKUaRxzL93TdV1GyZi7DzrXk=';

                IF NOT EXISTS (SELECT 1 FROM provinces WHERE Id = 1)
                BEGIN
                    SET IDENTITY_INSERT provinces ON;
                    INSERT INTO provinces (Id, Name, CreatedAt, UpdatedAt)
                    VALUES (1, N'Ha Noi', @Now, @Now);
                    SET IDENTITY_INSERT provinces OFF;
                END;

                IF NOT EXISTS (SELECT 1 FROM users WHERE Id = 2 OR Username = N'johnsmith')
                BEGIN
                    SET IDENTITY_INSERT users ON;
                    INSERT INTO users
                        (Id, Username, Email, PasswordHash, FirstName, LastName, Bio,
                         ProfileImageUrl, ProvinceId, Role, CreatedAt, UpdatedAt)
                    VALUES
                        (2, N'johnsmith', N'john.smith@gmail.com', @PasswordHash,
                         N'John', N'Smith',
                         N'Singer and dancer passionate about live performance and creative collaboration.',
                         N'https://example.com/images/user-2.jpg', 1, N'Member', @Now, @Now);
                    SET IDENTITY_INSERT users OFF;
                END;

                UPDATE users
                SET PasswordHash = @PasswordHash,
                    Bio = COALESCE(Bio, N'Singer and dancer passionate about live performance and creative collaboration.'),
                    ProfileImageUrl = COALESCE(ProfileImageUrl, N'https://example.com/images/user-2.jpg'),
                    UpdatedAt = @Now
                WHERE Id = 2 AND Username = N'johnsmith';

                IF NOT EXISTS (SELECT 1 FROM talents WHERE Name = N'Singing')
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM talents WHERE Id = 2)
                    BEGIN
                        SET IDENTITY_INSERT talents ON;
                        INSERT INTO talents (Id, Name, CreatedAt, UpdatedAt)
                        VALUES (2, N'Singing', @Now, @Now);
                        SET IDENTITY_INSERT talents OFF;
                    END
                    ELSE
                        INSERT INTO talents (Name, CreatedAt, UpdatedAt)
                        VALUES (N'Singing', @Now, @Now);
                END;

                IF NOT EXISTS (SELECT 1 FROM talents WHERE Name = N'Dancing')
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM talents WHERE Id = 3)
                    BEGIN
                        SET IDENTITY_INSERT talents ON;
                        INSERT INTO talents (Id, Name, CreatedAt, UpdatedAt)
                        VALUES (3, N'Dancing', @Now, @Now);
                        SET IDENTITY_INSERT talents OFF;
                    END
                    ELSE
                        INSERT INTO talents (Name, CreatedAt, UpdatedAt)
                        VALUES (N'Dancing', @Now, @Now);
                END;

                DECLARE @UserId int = (SELECT TOP 1 Id FROM users WHERE Id = 2 AND Username = N'johnsmith');
                DECLARE @SingingId int = (SELECT TOP 1 Id FROM talents WHERE Name = N'Singing');
                DECLARE @DancingId int = (SELECT TOP 1 Id FROM talents WHERE Name = N'Dancing');

                IF @UserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM UserProfiles WHERE UserId = @UserId)
                    INSERT INTO UserProfiles
                        (UserId, Phone, Website, Facebook, Youtube, Instagram, Tiktok,
                         Address, Headline, Experience, CreatedAt, UpdatedAt)
                    VALUES
                        (@UserId, N'+84901234567', N'https://example.com/tin-nguyen',
                         N'https://facebook.com/tinnguyen', N'https://youtube.com/@tinnguyen',
                         N'https://instagram.com/tinnguyen', N'https://tiktok.com/@tinnguyen',
                         N'Ho Chi Minh City, Vietnam', N'Singer, dancer and creative performer',
                         N'Five years of stage performance, vocal practice and community events.',
                         @Now, @Now);

                IF @UserId IS NOT NULL AND @SingingId IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM user_talents WHERE UserId = @UserId AND TalentId = @SingingId)
                    INSERT INTO user_talents
                        (UserId, TalentId, IsPrimary, YearsExperience, Level, CreatedAt, UpdatedAt)
                    VALUES (@UserId, @SingingId, 0, 5, N'Advanced', @Now, @Now);

                IF @UserId IS NOT NULL AND @DancingId IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM user_talents WHERE UserId = @UserId AND TalentId = @DancingId)
                    INSERT INTO user_talents
                        (UserId, TalentId, IsPrimary, YearsExperience, Level, CreatedAt, UpdatedAt)
                    VALUES (@UserId, @DancingId, 1, 2, N'Intermediate', @Now, @Now);

                IF @UserId IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM achievements WHERE UserId = @UserId AND Title = N'Top 10 University Talent Showcase')
                    INSERT INTO achievements
                        (UserId, Title, Description, AchievementDate, CreatedAt, UpdatedAt)
                    VALUES
                        (@UserId, N'Top 10 University Talent Showcase',
                         N'Reached the final round with a live vocal performance.',
                         '2025-11-20', @Now, @Now);

                IF @UserId IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM awards WHERE UserId = @UserId AND AwardName = N'Best Vocal Performance')
                    INSERT INTO awards
                        (UserId, AwardName, Organization, AwardDate, CreatedAt, UpdatedAt)
                    VALUES
                        (@UserId, N'Best Vocal Performance', N'Aptech Talent Showcase',
                         '2025-12-15', @Now, @Now);

                IF @UserId IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM certifications WHERE UserId = @UserId AND Name = N'Professional Vocal Training')
                    INSERT INTO certifications
                        (UserId, Name, IssuedBy, IssueDate, ExpiredDate, CertificateUrl,
                         CreatedAt, UpdatedAt)
                    VALUES
                        (@UserId, N'Professional Vocal Training', N'Vietnam Music Academy',
                         '2025-01-10', '2027-01-10',
                         N'https://example.com/certificates/vocal-training-2', @Now, @Now);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM users WHERE Id = 2 AND Username = N'johnsmith';
                DELETE FROM talents
                WHERE Name IN (N'Singing', N'Dancing')
                  AND NOT EXISTS (SELECT 1 FROM user_talents WHERE TalentId = talents.Id)
                  AND NOT EXISTS (SELECT 1 FROM videos WHERE TalentId = talents.Id)
                  AND NOT EXISTS (SELECT 1 FROM communities WHERE TalentId = talents.Id)
                  AND NOT EXISTS (SELECT 1 FROM contests WHERE TalentId = talents.Id)
                  AND NOT EXISTS (SELECT 1 FROM opportunities WHERE TalentId = talents.Id);
                DELETE FROM provinces
                WHERE Id = 1
                  AND NOT EXISTS (SELECT 1 FROM users WHERE ProvinceId = provinces.Id)
                  AND NOT EXISTS (SELECT 1 FROM opportunities WHERE ProvinceId = provinces.Id);
                """);
        }
    }
}
