using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using O2GEN.Models;

namespace O2GEN.Authorization
{
    public static class JwtTokenExtension
    {
        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "MyAuthClient";
        internal const string KEY = "childerolandtothedarktowercame";

        public static string GenerateJwtToken(this Credentials user)
        {
            // Set our tokens claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim("id", user.Id.ToString()),
                new Claim("did", user.DeptId.ToString())
            };

            // Create the credentials used to generate the token
            var credentials = new SigningCredentials(
                // Get the secret key
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY)),
                // Use HS256 algorithm
                SecurityAlgorithms.HmacSha256);

            // Generate the Jwt Token
            var token = new JwtSecurityToken(
                issuer: ISSUER,
                audience: AUDIENCE,
                claims: claims,
                signingCredentials: credentials,
                notBefore: DateTime.Now,
                // Expire if not used
                expires: DateTime.Now.AddDays(1)//AddHours(24)
                );

            // Return the generated token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static bool ValidateJwtToken(string token, Credentials user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validateParameters = GetValidationParameters();

            SecurityToken securityToken;
            try
            {
                IPrincipal principal = tokenHandler.ValidateToken(token, validateParameters, out securityToken);
                var sec = tokenHandler.ReadToken(token) as JwtSecurityToken;

                //string nameClaim = sec.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                //string defaultClaim = sec.Claims.First(c => c.Type == ClaimsIdentity.DefaultNameClaimType).Value;
                //string guidClaim = sec.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static Credentials ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validateParameters = GetValidationParameters();
            Credentials output = new Credentials();
            SecurityToken securityToken;
            try
            {
                var sec = tokenHandler.ReadToken(token) as JwtSecurityToken;
                long tmp = 0;
                if (!long.TryParse(sec.Claims.First(c => c.Type == "id").Value, out tmp))
                {
                    return null;
                }
                output.Id = tmp;
                if (!long.TryParse(sec.Claims.First(c => c.Type == "did").Value, out tmp))
                {
                    return null;
                }
                output.DeptId = tmp;
                output.UserName = sec.Claims.First(c => c.Type == ClaimTypes.Name).Value;

                IPrincipal principal = tokenHandler.ValidateToken(token, validateParameters, out securityToken);
                output.TokenException = Authorization.TokenExceprion.Ok;
                return output;
                //string defaultClaim = sec.Claims.First(c => c.Type == ClaimsIdentity.DefaultNameClaimType).Value;
                //string guidClaim = sec.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            }
            catch (SecurityTokenExpiredException e)
            {
                output.TokenException = Authorization.TokenExceprion.Expired;
                output.Token = GenerateJwtToken(output);
                return output;
            }
            catch (Exception e)
            {
                output.TokenException = Authorization.TokenExceprion.WrongToken;
                return output;
            }
        }

        private static TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = ISSUER,
                ValidAudience = AUDIENCE,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY)) // The same key as the one that generate the token
            };
        }
    }
}
