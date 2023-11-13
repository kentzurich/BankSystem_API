using API.Services;
using Application.Account.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserAccountController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<UserAccount> _userManager;
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly TokenService _tokenService;

        public UserAccountController(IMapper mapper,
                                     UserManager<UserAccount> userManager,
                                     SignInManager<UserAccount> signInManager,
                                     TokenService tokenService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserDTO>> Register(RegisterAccountDTO registerAccountDTO)
        {
            if (registerAccountDTO is null) return NotFound(registerAccountDTO);

            //validate if username is already created
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerAccountDTO.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return ValidationProblem();
            }

            UserAccount userAccount = new()
            {
                AccountId = Guid.NewGuid(),
                FirstName = registerAccountDTO.FirstName,
                MiddleName = registerAccountDTO.MiddleName,
                LastName = registerAccountDTO.LastName,
                Birthdate = registerAccountDTO.Birthdate,
                UserName = registerAccountDTO.Username
            };

            //create user
            var result = await _userManager.CreateAsync(userAccount, registerAccountDTO.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(_mapper.Map<UserDTO>(userAccount));
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserDTO>> Login(LoginAccountDTO loginAccountDTO)
        {
            //check if username is existing to db
            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(x => x.UserName == loginAccountDTO.Username);

            if (user == null) return Unauthorized("Invalid username.");

            //check if password is existing to db
            var result = await _signInManager
                .CheckPasswordSignInAsync(user, loginAccountDTO.Password, false);

            if (!result.Succeeded) return Unauthorized("Invalid password.");

            //setting refresh token after username and password is validated
            await SetRefreshToken(user);

            return Ok(CreateUserObject(user));
        }

        private async Task SetRefreshToken(UserAccount userAccount)
        {
            //generate refresh token
            var refreshToken = _tokenService.GenerateRefreshToken();

            //save refresh token to db
            userAccount.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(userAccount);

            //setting cookie options
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            //saved token to cookie
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        private UserDTO CreateUserObject(UserAccount user)
        {
            return new UserDTO
            {
                AccountId = user.AccountId,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.MiddleName,
                Birthdate = user.Birthdate,
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
