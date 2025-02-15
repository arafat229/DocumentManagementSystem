using DocumentManagementSystem.Model;

namespace DocumentManagementSystem.Interface
{
    public interface ITokenService
    {
         string CreateToken(AppUser user);
    }
}
