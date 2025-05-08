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

        public async Task<(int totalAccount, int ownersAccount, int customersAccount, int staffsAccount)> GetTotalAccount()
        {
            var customerRole = await _roleManager.FindByNameAsync("Customer");
            var customersCount = await _userManager.GetUsersInRoleAsync(customerRole.Name);

            var ownerRole = await _roleManager.FindByNameAsync("Owner");
            var ownersCount = await _userManager.GetUsersInRoleAsync(ownerRole.Name);

            var StaffRole = await _roleManager.FindByNameAsync("Staff");
            var staffsCount = await _userManager.GetUsersInRoleAsync(StaffRole.Name);



            int totalAccountsCount = customersCount.Count + ownersCount.Count + staffsCount.Count;
            int ownersAccount = ownersCount.Count;
            int customersAccount = customersCount.Count;
            int staffsAccount = staffsCount.Count;  

            return (totalAccountsCount, ownersAccount, customersAccount, staffsAccount);
        }
    }
}
