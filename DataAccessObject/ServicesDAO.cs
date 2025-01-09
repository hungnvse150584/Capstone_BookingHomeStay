using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class ServicesDAO : BaseDAO<Services>
    {
        private readonly GreenRoamContext _context;
        public ServicesDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }
    }
}
