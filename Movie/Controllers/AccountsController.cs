using Microsoft.AspNetCore.Mvc;
using Movie.DTOs;
using Movie.Services.Interfaces;

namespace Movie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAuthService authService;
        public AccountsController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(UserRegisterationDTO userRegisterationDTO)
        {
            if(!ModelState.IsValid)  return BadRequest(ModelState);
            AuthResult result=await authService.RegisterAsync(userRegisterationDTO);

            if(!result.Success) return BadRequest(result.Errors);

            // Set the refresh token in a cookie if it exists

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(UserLoginDTO userLoginDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            AuthResult result = await authService.LoginAsync(userLoginDTO);

            if (!result.Success) return BadRequest(result.Errors);

            // Set the refresh token in a cookie if it exists
            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }

            return Ok(result);

        }

        [HttpPost("AddRoleToUser")]
        public async Task<IActionResult> AddRoleToUserAsync(AddRoleToUserDTO addRoleDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            RoleResult result = await authService.AddRoleToUserAsync(addRoleDTO);

            if (!result.Success) return BadRequest(result.Errors);

            return Ok(result);

        }


        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRoleAsync([FromForm]string Role)
        {
            if (string.IsNullOrEmpty(Role)) return BadRequest("Role Name is Required");
            RoleResult result = await authService.CreateRoleAsync(Role);

            if (!result.Success) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            // get refreshToken in cookie
            var resfreshToken = Request.Cookies["RefreshToken"];
            var result= await authService.RefreshTokenAsync(resfreshToken);

            if (!result.Success) return BadRequest(result.Errors);

            //save refreshToken in cookie
            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);

        }

        [HttpPost("RevokeToken")]
        public async Task<IActionResult> RevokeToken(RevokeTokenDTO revokeTokenDTO)
        {
            //  we get token from body and u can get it from request cookie
            
            //if token in dto is null => get it from cookies
            var token=revokeTokenDTO.Token?? Request.Cookies["RefreshToken"];

            // if it also null return bad request
            if (string.IsNullOrEmpty(token)) return BadRequest("Token is Required");


            var result=await authService.RevokeTokenAsync(token);

            if (!result) return BadRequest("Token is invalid");

            return Ok($"{token} is Revoked successfull");

        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime? ExpiresOn)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = ExpiresOn.Value.ToLocalTime()

            };
            Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
        }

        [HttpGet("GetTokenOfUser")]
        
        public async Task<IActionResult> GetTokenOfUser([FromBody]string Email)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var token =await authService.GetRefreshTokenOfUser(Email);
            if (string.IsNullOrEmpty(token)) return BadRequest("Failed");
            return Ok(token);

        }






    }
}
