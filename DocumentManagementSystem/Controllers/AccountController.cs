using AutoMapper;
using DocumentManagementSystem.Data;
using DocumentManagementSystem.DTOs.Account;
using DocumentManagementSystem.Helpers;
using DocumentManagementSystem.Model;
using HashidsNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DocumentManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{

    private readonly IConfiguration config;
    private readonly IMapper mappper;
    private readonly IHashids hashids;
    private readonly UserManager<User> userManager;
    private readonly RoleManager<Role> roleManager;
    private readonly SignInManager<User> signInManager;
    private readonly ApplicationDBContext dbContext;
    private readonly ILogger<AccountController> logger;


    public AccountController(IConfiguration config, IMapper mappper, UserManager<User> userManager,
        RoleManager<Role> roleManager, IHashids hashids, SignInManager<User> signInManager, ApplicationDBContext context, ILogger<AccountController> logger)
    {
        this.config = config;
        this.mappper = mappper;
        this.hashids = hashids;
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
                    Message = "Failed to create a user Acoount"
                });

            user = await dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == registerDto.PhoneNumber);
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

            var hashedId = Utils.GetIdHashed(hashids, userFromDB.Id);

            userFromDB.Roles = await userManager.GetRolesAsync(userFromDB);
            var claims = new List<Claim>()
            {
                new Claim(Constants.UserId,hashedId),
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

            tempUser.Id = hashedId;
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
    

}
