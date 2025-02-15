using DocumentManagementSystem.DTOs.Account;
using DocumentManagementSystem.Interface;
using DocumentManagementSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;

namespace DocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            

        }

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]


        [HttpPost("Register")]
        
        public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
        {
            try
            {

                if (!ModelState.IsValid)
                {

                    return BadRequest(ModelState);
                }

                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,

                };
                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {

                        return Ok(
                            new NewUserDto
                            {

                                UserName = appUser.UserName,
                                Email = appUser.Email,
                                Token = _tokenService.CreateToken(appUser)

                            });

                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }


                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
               
            }
        }



        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Login")]
        [Authorize]
        
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username);
            if (user == null)
            {

                return Unauthorized("Invalid Input");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password,true);

            if (!result.Succeeded)
            {
                return Unauthorized("Incorred PassWord and/or Username not found");
            }
            return Ok(
                new userDto
                {
                    Username =user.UserName,
                    Email= user.Email,
                   
                });

        }


    }
}
