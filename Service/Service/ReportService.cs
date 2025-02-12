using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Report;
using Service.RequestAndResponse.Response.Reports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ReportService : IReportService
    {
        private readonly IMapper _mapper;
        private readonly IReportRepository _reportRepository;

        public ReportService(IMapper mapper, IReportRepository reportRepository)
        {
            _mapper = mapper;
            _reportRepository = reportRepository;
        }

        public async Task<BaseResponse<IEnumerable<GetAllReport>>> GetAllReport()
        {
            IEnumerable<Report> reports = await _reportRepository.GetAllReportsAsync();
            if (reports == null)
            {
                return new BaseResponse<IEnumerable<GetAllReport>>(
                    "Something went wrong!",
                    StatusCodeEnum.BadGateway_502,
                    null);
            }

            var response = _mapper.Map<IEnumerable<GetAllReport>>(reports);
            return new BaseResponse<IEnumerable<GetAllReport>>(
                "Get all reports success",
                StatusCodeEnum.OK_200,
                response);
        }

        public async Task<BaseResponse<GetAllReport>> GetReportById(int id)
        {
            var report = await _reportRepository.GetReportByIdAsync(id);
            var response = _mapper.Map<GetAllReport>(report);
            return new BaseResponse<GetAllReport>("Get report by id success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<CreateReport>> CreateReport(CreateReport createReport)
        {
            Report report = _mapper.Map<Report>(createReport);
            await _reportRepository.AddAsync(report);
            var response = _mapper.Map<CreateReport>(report);
            return new BaseResponse<CreateReport>("Create report success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<UpdateReport>> UpdateReport(int id, UpdateReport updateReport)
        {
            Report report = await _reportRepository.GetByIdAsync(id);
            _mapper.Map(updateReport, report);
            await _reportRepository.UpdateAsync(report);
            var response = _mapper.Map<UpdateReport>(report);
            return new BaseResponse<UpdateReport>("Update report success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<string>> DeleteReport(int id)
        {
            var report = await _reportRepository.GetByIdAsync(id);
            await _reportRepository.DeleteAsync(report);
            return new BaseResponse<string>("Delete report success", StatusCodeEnum.OK_200, "Deleted successfully");
        }

        public async Task<BaseResponse<IEnumerable<GetAllReport>>> SearchReport(string search, int pageIndex, int pageSize)
        {
            var reports = await _reportRepository.SearchReportAsync(search, pageIndex, pageSize);
            var response = _mapper.Map<IEnumerable<GetAllReport>>(reports);
            return new BaseResponse<IEnumerable<GetAllReport>>("Search report success", StatusCodeEnum.OK_200, response);
        }
    }
}
