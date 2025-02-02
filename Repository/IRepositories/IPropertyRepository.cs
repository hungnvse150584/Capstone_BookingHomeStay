using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetAllAsync();
        Task<Property> AddAsync(Property entity);
        Task<Property> UpdateAsync(Property entity);
        Task<Property> DeleteAsync(Property entity);
        Task<Property> GetByIdAsync(int id);
    }
}
