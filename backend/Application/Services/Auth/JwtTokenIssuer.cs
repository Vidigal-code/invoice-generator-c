using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Infrastructure.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InvoiceGenerator.Api.Application.Services.Auth
{
    public sealed class JwtTokenIssuer : IJwtTokenIssuer
    {
        private readonly AppSettings _appSettings;

        public JwtTokenIssuer(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public string CreateEncryptedToken(string userId, string roleName, string username)
        {
            var jwtSecret = _appSettings.JwtSettings.Secret;
            var jweSecret = _appSettings.JwtSettings.JweSecret;

            if (string.IsNullOrEmpty(jwtSecret))
                throw new InvalidOperationException("JwtSettings:Secret não configurado.");
            if (string.IsNullOrEmpty(jweSecret))
                throw new InvalidOperationException("JwtSettings:JweSecret não configurado.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var jweKey = Encoding.ASCII.GetBytes(jweSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim(JwtCustomClaims.PermissionContracts, JwtCustomClaims.Write),
                    new Claim(JwtCustomClaims.PermissionDebts, JwtCustomClaims.Read)
                }),
                Expires = DateTime.UtcNow.AddHours(AuthCookieConstants.SessionHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(jweKey), JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
