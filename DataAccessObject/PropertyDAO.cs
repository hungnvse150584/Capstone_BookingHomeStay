using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class PropertyDAO : BaseDAO<Property>
    {
        private readonly GreenRoamContext _context;
        public PropertyDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }
    }
}
