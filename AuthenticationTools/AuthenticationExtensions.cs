using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace SignalR_Tests
{
    public static class AuthenticationExtensions
    {
        public static void AddKeycloakAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization();
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.Authority = configuration["KeycloakAuthority"];
                o.Audience = "account";
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.IncludeErrorDetails = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    AuthenticationType = JwtBearerDefaults.AuthenticationScheme,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = false,
                    SignatureValidator = (token, parameters) =>
                    {
                        var jwtSecurityToken = new JwtSecurityToken(token);

                        return CleanClaims(jwtSecurityToken);
                    },
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,
                };
                o.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "text/plain";

                        await c.Response.WriteAsync(c.Exception?.Message ?? "An error occurred processing your authentication.");
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var authToken = context.Request.Headers["Authorization"].ToString();
                        var token = !string.IsNullOrEmpty(accessToken) ? accessToken.ToString() : !string.IsNullOrEmpty(authToken) ? authToken.Substring(7) : String.Empty;
                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(token) &&
                            (path.StartsWithSegments("/ServerHub")))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        private static JwtSecurityToken CleanClaims(JwtSecurityToken jwt)
        {
            const string AccountType = "account_type";

            jwt.Payload.Remove(AccountType);

            return jwt;
        }
    }
}
