using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class AccountDAO : BaseDAO<Account>
    {
        private readonly GreenRoamContext _context;
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountDAO(GreenRoamContext context, UserManager<Account> userManager,
            RoleManager<IdentityRole> roleManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<(int totalAccount, int ownersAccount, int customersAccount)> GetTotalAccount()
        {
            var customerRole = await _roleManager.FindByNameAsync("Customer");
            var customersCount = await _userManager.GetUsersInRoleAsync(customerRole.Name);

            var ownerRole = await _roleManager.FindByNameAsync("Owner");
            var ownersCount = await _userManager.GetUsersInRoleAsync(ownerRole.Name);

            

            int totalAccountsCount = customersCount.Count + ownersCount.Count;
            int ownersAccount = ownersCount.Count;
            int customersAccount = customersCount.Count;

            return (totalAccountsCount, ownersAccount, customersAccount);
        }
    }
}
