using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IStaffRepository : IBaseRepository<Staff>
    {
        Task<IEnumerable<Staff>> GetAllStaffByOwner(string accountID);
        Task<Staff?> GetStaffByID(string accountID);
        Task<IEnumerable<Staff>> GetAllStaffByHomeStay(int homeStayID);

    }
}
