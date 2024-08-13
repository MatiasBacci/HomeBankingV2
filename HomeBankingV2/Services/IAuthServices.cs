using HomeBankingV2.DTO_s;

namespace HomeBankingV2.Services
{
    public interface IAuthServices
    {
        public bool VerifyPassword(ClientLoginDTO clientLoginDTO);
    }
}
