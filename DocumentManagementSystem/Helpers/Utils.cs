using HashidsNet;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DocumentManagementSystem.Helpers
{
    public static class Utils
    {
        public static string GenerateJwtToken(IConfiguration config, IList<Claim> userClaims, DateTime expiry)
        {
            var secret = config.GetSection("AppSettings:Token").Value ?? "";
            var key = new SymmetricSecurityKey(Utils.StringToByteArray($"{secret}-{secret}"));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(userClaims),
                Expires=expiry,
                SigningCredentials = cred,
                Audience= config["AppSetting:Issuer"],
                IssuedAt = DateTime.UtcNow,
                Issuer = config["AppSetting:Audience"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
        public static byte[] StringToByteArray(string stringToConvert) => Encoding.UTF8.GetBytes(stringToConvert);

      //  public static string GetIdHashed(IHashids hashids, int id) => hashids.Encode(id);
        public static string GetIdHashed(IHashids hashids, long id) => hashids.EncodeLong(id);

    
    }
}
