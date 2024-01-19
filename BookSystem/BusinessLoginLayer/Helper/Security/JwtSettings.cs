
namespace BusinessLayer.Helper.Security
{
    public class JwtSettings
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public double DurationExpiredInDay { get; set; }
    }
}
