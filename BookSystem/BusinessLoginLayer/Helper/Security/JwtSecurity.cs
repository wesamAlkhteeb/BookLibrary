using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLayer.Helper.Security
{
    public class JwtSecurity
    {
        public static JwtSecurity securityData { get; } = new JwtSecurity();
        private JwtSecurity() { }

        public string getHashPassword(string password)
        {
            //byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); 
            byte[] salt = Encoding.ASCII.GetBytes("Aw7DIn3e+4E+Tta/OAIJTQ==");
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt, // must be unique
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8)
            );
        }

        public Dictionary<string, object> GenerateToken(string username, int id, string role, JwtSettings jwt)
        {

            var Claim = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.PrimarySid, id.ToString()),
        };

            DateTime expire = DateTime.Now.AddDays(jwt.DurationExpiredInDay);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key!));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    Claim,
                    expires: expire,
                    signingCredentials: signIn
                );
            string finalToken = new JwtSecurityTokenHandler().WriteToken(token);
            //return finalToken + "*204*Wesam*" + expire;
            return new Dictionary<string, object>{
            {"token",finalToken},
            {"expire",expire},
            {"role",role}
        };
        }


        public string seperateTokenFromHeader(string token)
        {
            return token.Split(" ")[1];
        }
        public int getIdToken(string token)
        {
            token = seperateTokenFromHeader(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            foreach (var d in securityToken.Claims)
            {
                if (d.Type.Contains("primarysid"))
                {
                    return Convert.ToInt32(d.Value);
                }
            }
            throw new Exception("try re-login.");
        }
        public string getRoleToken(string token)
        {
            token = seperateTokenFromHeader(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            foreach (var d in securityToken.Claims)
            {
                if (d.Type.Contains("role"))
                {
                    return d.Value;
                }
            }
            return "null";
        }

    }
}