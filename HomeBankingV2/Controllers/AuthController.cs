using HomeBankingV2.Models;
using HomeBankingV2.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HomeBankingV2.DTO_s;
using HomeBankingV2.Services;


namespace HomeBankingV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IClientServices _clientServices;
        public AuthController(IClientServices clientServices) => _clientServices = clientServices;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ClientLoginDTO clientDTO)
        {
            try
            {
                Client user = _clientServices.GetByEmail(clientDTO.Email);
               
                if (!String.Equals(user.Password, clientDTO.Password))
                    return StatusCode(403, "Invalid password");

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