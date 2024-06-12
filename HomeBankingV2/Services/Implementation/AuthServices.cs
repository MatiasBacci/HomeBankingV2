using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using Microsoft.AspNetCore.Identity;


namespace HomeBankingV2.Services.Implementation
{
    public class AuthServices : IAuthServices
    {
        private readonly IClientServices _clientServices;
        private readonly PasswordHasher<Client> _passwordHasher;

        public AuthServices(IClientServices clientServices)
        {
            _clientServices = clientServices;
            _passwordHasher = new PasswordHasher<Client>();
        }

        public bool VerifyPassword(ClientLoginDTO clientLoginDTO)
        {
            var client = _clientServices.GetByEmail(clientLoginDTO.Email);
            if (client == null) 
                return false;

            
            var verificationResult = _passwordHasher.VerifyHashedPassword(client, client.Password, clientLoginDTO.Password);
            return verificationResult == PasswordVerificationResult.Success;
        }
    }
}
