using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.Bookings;
using Service.RequestAndResponse.Response.HomeStays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;

        public AccountService(IMapper mapper, IAccountRepository accountRepository)
        {
            _mapper = mapper;
            _accountRepository = accountRepository;
        }

        public async Task<Account> GetByStringId(string id)
        {
            var account = await _accountRepository.GetByStringId(id);
            if (account == null)
            {
                throw new ArgumentException("Cannot Find account!");
            }
            return account;
        }

        public async Task<BaseResponse<GetTotalAccount>> GetTotalAccount()
        {
            var accounts = await _accountRepository.GetTotalAccount();
            var response = new GetTotalAccount
            {
                totalAccount = accounts.totalAccount,
                customersAccount = accounts.customersAccount,
                ownersAccount = accounts.ownersAccount,
                staffsAccount = accounts.staffsAccount
            };
            if (response == null)
            {
                return new BaseResponse<GetTotalAccount>("Get All Fail", StatusCodeEnum.BadGateway_502, response);
            }
            return new BaseResponse<GetTotalAccount>("Get All Success", StatusCodeEnum.OK_200, response);
        }
    }
}
