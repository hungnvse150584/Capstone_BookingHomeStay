using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Task<(int totalAccount, int ownersAccount, int customersAccount)> GetTotalAccount();
        Task<Account> GetByAccountIdAsync(string accountId);
    }
}
