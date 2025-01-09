using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IWardRepository : IBaseRepository<Ward>
    {
        Task<string> GetWardNameById(int? wardId);
    }
}
