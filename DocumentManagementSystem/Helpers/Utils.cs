using HashidsNet;
using Microsoft.EntityFrameworkCore;
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

        public class PagedList<T> : List<T>
        { 
        
        public int CurrentPage { get; set; }
        public int PageSige { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

            public PagedList(List<T>items,int pageNumber,int pageSize, int count)
            {
                CurrentPage = pageNumber;
                TotalCount = count;
                PageSige = pageSize;
                TotalPages = (int)Math.Ceiling(Count / (double)pageSize);
                this.AddRange(items);

            }
            public static async Task <PagedList<T>>CreateAsync(IQueryable<T> source,int pageNumber,int pageSize)
            {
                var count = await source.CountAsync();
                var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                return new PagedList<T>(items, pageNumber, pageSize, count);

            }
          

        }
        public static string GeneratePassword(int lenght)
        {
            List<char> chars = new List<char>();

            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            int j = 0;
            while (j < lenght)
            {
                sb.Append(chars[random.Next(0, chars.Count)]);
                j++;
            }
            return sb.ToString();

        }



    }
}
