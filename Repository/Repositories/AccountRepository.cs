using BusinessObject.Model;
using DataAccessObject;
using Repository.BaseRepository;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        private readonly AccountDAO _accountDao;

        public AccountRepository(AccountDAO accountDao) : base(accountDao)
        {
            _accountDao = accountDao;
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _accountDao.GetAllAsync();
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _accountDao.GetByIdAsync(id);
        }

        public async Task<Account> GetByStringIdAsync(string id)
        {
            return await _accountDao.GetByStringIdAsync(id);
        }

        public async Task<Account> AddAsync(Account entity)
        {
            return await _accountDao.AddAsync(entity);
        }

        public async Task<Account> UpdateAsync(Account entity)
        {
            return await _accountDao.UpdateAsync(entity);
        }

        public async Task<Account> DeleteAsync(Account entity)
        {
            return await _accountDao.DeleteAsync(entity);
        }

        public async Task<(int totalAccount, int ownersAccount, int customersAccount, int staffsAccount)> GetTotalAccount()
        {
            return await _accountDao.GetTotalAccount();
        }

        public async Task<Account> GetByAccountIdAsync(string accountId)
        {
      
            if (int.TryParse(accountId, out int id))
            {
                return await GetByIdAsync(id);
            }
            return null;
        }
     
    }
}
