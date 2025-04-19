using AutoMapper;
using DocumentManagementSystem.Data;
using DocumentManagementSystem.DTOs.Account;
using DocumentManagementSystem.DTOs.AccountsDtos;
using DocumentManagementSystem.Helpers;
using DocumentManagementSystem.Model;
//using HashidsNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using static DocumentManagementSystem.Helpers.Utils;

namespace DocumentManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{

    private readonly IConfiguration config;
    private readonly IMapper mappper;
 //   private readonly IHashids hashids;
    private readonly UserManager<User> userManager;
    private readonly RoleManager<Role> roleManager;
    private readonly SignInManager<User> signInManager;
    private readonly ApplicationDBContext dbContext;
    private readonly ILogger<AccountController> logger;


    public AccountController(IConfiguration config, IMapper mappper, UserManager<User> userManager,
        RoleManager<Role> roleManager, /*IHashids hashids*/ SignInManager<User> signInManager, ApplicationDBContext context, ILogger<AccountController> logger)
    {
        this.config = config;
        this.mappper = mappper;
      //  this.hashids = hashids;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.signInManager = signInManager;
        this.dbContext = context;
        this.logger = logger;

    }

    [HttpPost("user/create")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserForRegisterDto registerDto)
    {

        try
        {
            registerDto.Role = registerDto.Role.ToLower();
            registerDto.FirstName = registerDto.FirstName.Trim().ToLower();
            registerDto.LastName = registerDto.LastName.Trim().ToLower();

            if (!await roleManager.RoleExistsAsync(registerDto.Role))
                return BadRequest(CallResult.Error("The role you are trying to access doesn't exist"));

            var userToCreate = mappper.Map<User>(registerDto);

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == registerDto.PhoneNumber);

            if (user != null)
                return BadRequest(CallResult.Error("Account with this username or email already exists"));

            userToCreate.CreatedAt = DateTime.UtcNow;

            var result = await userManager.CreateAsync(userToCreate, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(new CallResult()
                {
                    Data = result.Errors,
                    Message = "Failed to create a user Account                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  ount"
                });

            user = await dbContext.Users.Include(P=>P.MyFiles).FirstOrDefaultAsync(x => x.PhoneNumber == registerDto.PhoneNumber);
            if (user is null)
                return BadRequest(CallResult.Error("Failed to find the account"));

            var roleResult = await userManager.AddToRoleAsync(user, registerDto.Role.ToLower());

            if (!roleResult.Succeeded)
                //return BadRequest(CallResult.Error(string.Join(", ", roleResult.Errors.Select(x => x.Description))));
                return BadRequest(new CallResult
                {
                    IsSuccess = false,
                    Message = "something went wrong",
                    Data = roleResult.Errors
                });

            return Ok(CallResult.Ok("Account created succesfully"));
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error(ex));
        }



    }

    [HttpPost("users/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserLoginForDto loginDto)
    {
        try
        {
            var userFromDB = await userManager.FindByNameAsync(loginDto.UserName);
            if (userFromDB is null)
                return BadRequest(CallResult.Error("Can't find the account you are trying to access"));
            if (userFromDB.IsDeleted)
                return BadRequest(CallResult.Error("The account you're trying to access is deleted"));
            if (!userFromDB.IsActive)
                return BadRequest(CallResult.Error("The account you are trying to access is desactivated"));

            var result = await signInManager.CheckPasswordSignInAsync(userFromDB, loginDto.Password, true);
            if (!result.Succeeded)
                return Unauthorized(CallResult.Ok("Wrong password or username"));

            // var hashedId = Utils.GetIdHashed(hashids, userFromDB.Id);

            userFromDB.Roles = await userManager.GetRolesAsync(userFromDB);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,userFromDB.Id.ToString()),
                new Claim(Constants.UserName,userFromDB.UserName??""),
                new Claim(Constants.Email,userFromDB.Email??""),
                new Claim(Constants.PhoneNumber,userFromDB.PhoneNumber??"")
            };
            var role = "";
            if (userFromDB.Roles.Count >= 1)
            {
                role = userFromDB.Roles[0];
                claims.Add(new Claim(Constants.Role, role));

                var theRole = await roleManager.FindByNameAsync(role);
                if (theRole != null)
                    claims.AddRange(await roleManager.GetClaimsAsync(theRole));

            }
            List<String> roleClaims = new();
            claims.ForEach(c => { if (c.Type == Constants.Privilege && c.Value != null) roleClaims.Add(c.Value); });
            var tempUser = mappper.Map<UserDetailsDto>(userFromDB);

            tempUser.Id = Convert.ToString(userFromDB.Id);
            tempUser.Role = role;
            tempUser.Permission = roleClaims;

            var res = new
            {
                user = tempUser,
                token = Utils.GenerateJwtToken(config, claims, DateTime.UtcNow.AddDays(1))

            };
            return Ok(CallResult.Ok("Login Sucessfully", res));

        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error(ex));
        }


    }
    [HttpPost("users/list")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetUsers(PaginationDto? dto)
    {
        try
        {
            dto ??= new PaginationDto();


            var pagedUsers = await PagedList<User>.CreateAsync(dbContext.Users.AsNoTracking(), dto.PageNumber, dto.PageSize);

            var userToReturn = mappper.Map<IEnumerable<UserForListDto>>(pagedUsers);
            foreach (var u in userToReturn)
            {
                var user = mappper.Map<User>(userToReturn);
                var role = await userManager.GetRolesAsync(user);
                if (role.Count > 1)
                    u.Role = role[0];
            }
            return Ok(CallResult.Ok(data: userToReturn));
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error(ex));
        }

    }


    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        try
        {
            var userFromDb = await dbContext.Users.FirstOrDefaultAsync(x => x.Id.ToString() == userId);

            if (userFromDb is null)

                return NotFound(CallResult.Error("Can't find the acccount you are trying to access"));

            userFromDb.Roles = await userManager.GetRolesAsync(userFromDb);


            var userToReturn = mappper.Map<UserDetailsDto>(userFromDb);

            if (userToReturn.Role.Count() > 0)
                userToReturn.Role = userFromDb.Roles[0];

            return Ok(CallResult.Ok(data: userToReturn));
        }
        catch (Exception ex )
        {

            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error(ex.Message));
        }


    }


    [HttpPut("user/{userId}")]
    public async Task<IActionResult>Update(string userId,UserForUpdateDto userForUpdateDto)
    {
        userForUpdateDto.FullName = userForUpdateDto.FullName.Trim().ToLower();
        userForUpdateDto.Role = userForUpdateDto.Role.Trim().ToLower();
        userForUpdateDto.UserName = userForUpdateDto.UserName.Trim().ToLower();
        userForUpdateDto.LastName = userForUpdateDto.LastName.Trim().ToLower();
        userForUpdateDto.Email = userForUpdateDto.Email.Trim().ToLower();
        userForUpdateDto.PhoneNumber = userForUpdateDto.PhoneNumber.Trim().ToLower();

        var id = userId; 
        var userFromDB = await dbContext.Users.FirstOrDefaultAsync(x => x.Id.ToString().Equals(id));

        if (userFromDB is null)
            return BadRequest(CallResult.Error("Can't find the account your are trying to access"));

        if (!userFromDB.IsActive)
            return BadRequest(CallResult.Error("user you are trying to access is desactivated"));

        mappper.Map(userForUpdateDto ,userFromDB);

        var roles = await userManager.GetRolesAsync(userFromDB);

        var role = await roleManager.FindByNameAsync(userForUpdateDto.Role);

        if (role is null)
            return BadRequest(CallResult.Error("The role you are trying to access does not exist"));

        var res = await userManager.RemoveFromRolesAsync(userFromDB, roles);

        if (!res.Succeeded)
            return BadRequest(CallResult.Error("failed to update user role"));

        await userManager.AddToRoleAsync(userFromDB, role.Name!);

        userFromDB.CreatedAt = DateTime.UtcNow;

        dbContext.Update(userFromDB);

        if (await dbContext.SaveChangesAsync() <= 0)
            return BadRequest(CallResult.Error("unable to update user"));


        return Ok(CallResult.Ok());

    }

    [HttpPost("user/desactivate/{userId}")]
    [Authorize(Policy =("Admin"))]
    public async Task<IActionResult>DesactivateUser(string userId)
    {
      try
        {
            return await UserActivation(userId, false);
        }
        catch (Exception ex )
        {

            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error(ex.Message));
        }
    }



    [HttpPost("user/Activate/{userId}")]
    [Authorize(Policy = ("Admin"))]
    public async Task<IActionResult> ActivateUser(string userId)
    {
        try
        {
            return await UserActivation(userId,true);
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error(ex.Message));
        }
    }



    [HttpPost("user/change-password/{userId}")]
    public async Task<IActionResult>ChangePassword(string userId,ChangePasswordDto passwordDto)
    {
        try
        {
            if (!User.IsUserAllowed(userId))
                return Unauthorized();

            var id = userId;

            var userFormDB = await dbContext.Users.FirstOrDefaultAsync(x => x.Id.ToString().Equals(id));

            if (userFormDB is null)
                return NotFound(CallResult.Error("Can't find the account you are trying to access"));

            var res = await userManager.ChangePasswordAsync(userFormDB, passwordDto.OldPassword.Trim(), passwordDto.NewPassword.Trim());

            if(res.Succeeded)
            {
                return Ok(CallResult.Ok("Passwordchanged Succesfully"));

            }

            return BadRequest(CallResult.Error(string.Join(",", res.Errors.Select(s => s.Description))));

            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
           return BadRequest(CallResult.Error(ex.Message));
           
        }
        


    }


    [HttpPost("user/reset-password/{userId}")]
    public async Task<IActionResult>ResetPassword(string userId)
    {

        try
        {
            var id = userId;

            var userFromDB = await dbContext.Users.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (userFromDB is null)
                return BadRequest(CallResult.Error("Can't find the account you are trying to access"));

            var result = await userManager.RemovePasswordAsync(userFromDB);

            if (!result.Succeeded)
                return BadRequest(CallResult.Error(string.Join(",", result.Errors.Select(s => s.Description))));
            var password = Utils.GeneratePassword(6);
            var res = await userManager.AddPasswordAsync(userFromDB,password );

            if (!res.Succeeded)
                return BadRequest(CallResult.Error(string.Join(",", res.Errors.Select(s => s.Description))));

            userFromDB.UpdatedAt = DateTime.Now.ToUniversalTime();

            dbContext.Update(userFromDB);

            if (await dbContext.SaveChangesAsync() <= 0)
                return BadRequest(CallResult.Error());

            return Ok(CallResult.Ok(""));


        }
        catch (Exception ex )
        {
            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error());
           
        }
       
    }

    [HttpGet("Claims")]
    [AllowAnonymous]
    public async Task<IActionResult> GetClaims()
    {
        try
        {
            List<Claim> Claims = new();

            var roles = await roleManager.Roles.ToListAsync();

            foreach(var role in roles)
            {

                Claims.AddRange(await roleManager.GetClaimsAsync(role));
            }

            return Ok(CallResult.Ok(data: Claims.Select(x => x.Value).Distinct().ToList()));
        }
        catch (Exception ex )
        {

            Console.WriteLine(ex.StackTrace);
                return BadRequest(CallResult.Error(ex.Message));
        }


    }
    [HttpPost("roles/create")]

   // [Authorize(Policy = "Admim")]
    public async Task<IActionResult>CreateRole(RoleDto roleForCreate)
    {
        try
        {
            var role = await roleManager.FindByNameAsync(roleForCreate.Name);
            if (role != null)
                return NotFound(CallResult.Error("Role already exist"));

            role = new Role() { Name = roleForCreate.Name };
            var res = await roleManager.CreateAsync(role);

            if (!res.Succeeded)
                return BadRequest(CallResult.Error(string.Join(",", res.Errors.Select(s => s.Description))));

            var existingClaims = await roleManager.GetClaimsAsync(role);

            foreach (var claim in roleForCreate.Claims.Distinct().ToList())
            {
                if (!existingClaims.Any(x => x.Value == claim))
                    roleManager.AddClaimAsync(role, new Claim(Constants.Privilege, claim)).Wait();

            }
            role.UpdateAt = DateTime.Now;
            dbContext.Update(role);
            if (await dbContext.SaveChangesAsync() <= 0)
                return BadRequest(CallResult.Error());

            return Ok(CallResult.Ok());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error(ex.Message));

        }



    }
    [HttpGet("roles")]

    [Authorize(Policy = "Admim")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {

            var roles = await roleManager.Roles.Include(x => x.RoleClaims).ToListAsync();
            roles.ForEach(role =>
            {
                var rClaims = roleManager.GetClaimsAsync(role).Result;
                role.Claims = rClaims.Select(x => x.Value).ToList();
            });

            var res = roles.Select(x => new
            {
                x.Id,
                x.Name,
                x.NormalizedName,
                roleClaims = x.Claims 


            }).ToList();
            return Ok(CallResult.Ok(data: res));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error(ex.Message));

        }


    }
    [HttpGet("roles/{id}")]
    public async Task<IActionResult>GetRole( string id)
    {
        try
        {
            var userId = id;
            var role = await roleManager.Roles.Include(x => x.RoleClaims).FirstOrDefaultAsync(x => x.Id.ToString().Equals(id));

            if (role is null)
                return NotFound(CallResult.Error("Role not found"));

            var rClaims = await roleManager.GetClaimsAsync(role);
            role.Claims = rClaims.Select(x => x.Value).ToList();

            var data = new
            {

                role.Id,
                role.Name,
                role.NormalizedName,
                roleClaims = role.Claims

            };
            return Ok(CallResult.Ok(data: data));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error(ex.Message));

        }
  

    }

    [HttpPut("roles/{id}")]
    [Authorize(Policy = "Admim")]
    public async Task <IActionResult>UpdateRole(string roleId, RoleDto roleForCreate)
    {
        try
        {
            var id = roleId;
            var role = await roleManager.Roles.Include(s => s.RoleClaims).FirstOrDefaultAsync(x => x.Id.ToString().Equals(id));
            if (role is null)
                return NotFound(CallResult.Error("Role not found"));

            var existingRole = await roleManager.FindByNameAsync(roleForCreate.Name);

            if (existingRole != null && existingRole.Id != role.Id)
                return BadRequest(CallResult.Error("Role with the same name already exist"));

            var existingClaims = await roleManager.GetClaimsAsync(role);
            foreach (var claim in existingClaims)
                roleManager.RemoveClaimAsync(role, claim).Wait();

            foreach (var roleClaim in roleForCreate.Claims.Distinct().ToList())
                roleManager.AddClaimAsync(role, new Claim(Constants.Privilege, roleClaim)).Wait();

            role.Name = roleForCreate.Name;
            role.UpdateAt = DateTime.UtcNow;
            var res = await roleManager.UpdateAsync(role);
            if (!res.Succeeded)
                return BadRequest(CallResult.Error());

            return Ok(CallResult.Ok("Role Updated Successfully"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            return BadRequest(CallResult.Error(ex.Message));

        }

    }

    [HttpDelete("role/{id}")]
    public async Task<IActionResult>DeleteRole(string roleId)
    {
        try
        {

            var id = roleId;

            var role = await roleManager.Roles.FirstOrDefaultAsync(x => x.Id.ToString().Equals(id));
            if (role is null)
                BadRequest(CallResult.Error("Role not found"));
            var users = await userManager.GetUsersInRoleAsync(role.Name!);
            if (users.Count() > 0)
                BadRequest(CallResult.Error("Role can't be deleted"));

            var roleClaims = await roleManager.GetClaimsAsync(role);

            foreach(var claim in roleClaims)
            {
                await roleManager.RemoveClaimAsync(role, claim);

            }
            var result = await roleManager.DeleteAsync(role);

            if (!result.Succeeded)
                return BadRequest(CallResult.Error());

            return Ok(CallResult.Ok());


        }
        catch   (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest(CallResult.Error(ex.Message));


        }


    }

    


    [ApiExplorerSettings(IgnoreApi = true)]

    private async Task<IActionResult>UserActivation(string userId, bool activate)
    {


        var id = userId;

        var userFromDB = await dbContext.Users.FirstOrDefaultAsync(x => x.Id.ToString().Equals(id));

        if (userFromDB is null && activate == true)
        {
            userFromDB.UpdatedAt = DateTime.Now.ToUniversalTime();

            dbContext.Update(userFromDB);
        }
        if (await dbContext.SaveChangesAsync() <= 0)
            return BadRequest(CallResult.Error());

        return Ok(CallResult.Ok());


    }
}
