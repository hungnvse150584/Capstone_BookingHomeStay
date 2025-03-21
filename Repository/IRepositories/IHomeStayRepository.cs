﻿using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IHomeStayRepository : IBaseRepository<HomeStay>
    {
        Task<HomeStay?> ChangeHomeStayStatus(int homestayId, HomeStayStatus status);
        Task<IEnumerable<HomeStay>> GetAllRegisterHomeStayAsync();
        Task<HomeStay> GetHomeStayDetailByIdAsync(int id);
        Task<HomeStay> GetOwnerHomeStayByIdAsync(string accountId);
        Task SaveChangesAsync();
    }
}
