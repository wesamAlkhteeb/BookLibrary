using BusinessLayer.Helper.Security;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BusinessLayer.DependencyInjectionBusinessLayer
{
    public static class DependencyInjection
    {
        public static void DIBusinessLayer(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IAccountService, AccountService>();
            services.DISecurity(configuration);
        }
        public static IServiceCollection DISecurity(this IServiceCollection services, IConfiguration configuration)
        {
            var keyJwt = configuration["Security:JWT_Key"]!;
            var issuerJwt = configuration["Security:JWT_Issuer"]!;
            var audienceJwt = configuration["Security:JWT_Audience"]!;
            var durationExpiredInDay_JWT = configuration["Security:JWT_DurationExpiredInDay"]!;

            services.Configure<JwtSettings>(jwt => {
                jwt.Key = keyJwt;
                jwt.Audience = audienceJwt;
                jwt.Issuer = issuerJwt;
                jwt.DurationExpiredInDay = int.Parse(durationExpiredInDay_JWT);
            });
            services.AddAuthentication(option => {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(
                op =>
                {
                    op.RequireHttpsMetadata = false;
                    op.SaveToken = false;
                    op.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = issuerJwt,
                        ValidAudience = audienceJwt,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyJwt)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            return services;
        }
    }
}
