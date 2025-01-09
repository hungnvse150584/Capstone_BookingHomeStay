using BusinessObject.IdentityModel;
using BusinessObject.Model;
using Repository.IBaseRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        public TokenService(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<TokenModel> createToken(Account application)
        {
            return await _tokenRepository.createToken(application);
        }

        public async Task<ApiResponse> renewToken(TokenModel model)
        {
            return await _tokenRepository.renewToken(model);
        }
    }
}
