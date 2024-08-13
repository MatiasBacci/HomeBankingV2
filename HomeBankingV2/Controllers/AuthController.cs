using HomeBankingV2.Models;
using HomeBankingV2.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HomeBankingV2.DTO_s;
using HomeBankingV2.Services;
using HomeBankingV2.Services.Implementation;


namespace HomeBankingV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IClientServices _clientServices;
        private readonly IAuthServices _authServices;
        public AuthController(IClientServices clientServices, IAuthServices authServices)
        {
            _clientServices = clientServices;
            _authServices = authServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ClientLoginDTO clientLoginDTO)
        {
            try
            {
                bool isPasswordValid = _authServices.VerifyPassword(clientLoginDTO);

                if (!isPasswordValid)
                {
                    return Unauthorized("Invalid email or password");
                }

                Client user = _clientServices.GetByEmail(clientLoginDTO.Email);
              
                var claims = _clientServices.AddClaims(user);
       
                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}