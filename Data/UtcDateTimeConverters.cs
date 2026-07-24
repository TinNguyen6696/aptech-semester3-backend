using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TalentShowcase.Api.Data
{
    // SQL Server's datetime2 has no timezone concept, so EF Core always reads DateTime values
    // back with Kind=Unspecified — even though every value in this app is written as UTC
    // (DateTime.UtcNow). System.Text.Json only appends the "Z" suffix when Kind is Utc, so
    // without this, every DateTime field in every API response comes back as a naive string
    // and gets misinterpreted as LOCAL time by JS `new Date(...)` on the client — a silent
    // timezone shift equal to the client's UTC offset. This forces Kind=Utc back on read.
    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter() : base(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }

    public class NullableUtcDateTimeConverter : ValueConverter<DateTime?, DateTime?>
    {
        public NullableUtcDateTimeConverter() : base(
            v => v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)
        {
        }
    }
}
