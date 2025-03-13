﻿using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class RoomTypesDAO : BaseDAO<RoomTypes>
    {
        private readonly GreenRoamContext _context;
        public RoomTypesDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RoomTypes>> GetAllRoomTypesAsync(int roomTypeId)
        {
            return await _context.RoomTypes
                        .Where(c => c.RoomTypesID == roomTypeId)
                        .Include(c => c.ImageRoomTypes)
                        .ToListAsync();
        }


        public async Task<RoomTypes> GetRoomTypeByIdAsync(int? id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<RoomTypes>()
                        .Include(c => c.ImageRoomTypes)
               .SingleOrDefaultAsync(c => c.RoomTypesID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

    }
}
