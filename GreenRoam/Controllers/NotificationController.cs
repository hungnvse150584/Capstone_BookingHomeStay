using AutoMapper;
using BusinessObject.Model;
using GreenRoam.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.IService;
using Service.RequestAndResponse.Request.Notifications;
using Service.RequestAndResponse.Response.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenRoam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public NotificationController(
            INotificationService notificationService,
            IMapper mapper,
            IHubContext<NotificationHub> notificationHub)
        {
            _notificationService = notificationService;
            _mapper = mapper;
            _notificationHub = notificationHub;
        }

        //[Authorize(Roles = "Admin, Owner, Staff")]
        // API 1: Tạo thông báo mới
        [HttpPost("create-notification")]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var notification = new Notification
                {
                    AccountID = request.AccountID,
                    Title = request.Title,
                    Message = request.Message,
                    BookingID = request.BookingID,
                    BookingServicesID = request.BookingServicesID,
                    TypeNotify = request.TypeNotify
                };

                var createdNotification = await _notificationService.CreateNotificationAsync(notification);
                var response = _mapper.Map<NotificationResponse>(createdNotification);

                // Gửi thông báo real-time qua SignalR
                await _notificationHub.Clients.All.SendAsync(
                    "ReceiveNotification",
                    createdNotification.AccountID,
                    createdNotification.Title,
                    createdNotification.Message,
                    createdNotification.NotificationID,
                    createdNotification.CreateAt,
                    createdNotification.IsRead,
                    createdNotification.TypeNotify
                );

                return Ok(new { message = "Notification created successfully.", data = response });
            }
            catch (Exception ex)
            {
                // Ghi lại chi tiết lỗi
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        // API 2: Lấy danh sách thông báo theo AccountID
        //[Authorize(Roles = "Admin, Owner, Staff")]
        [HttpGet("by-account/{accountId}")]
        public async Task<IActionResult> GetNotificationsByAccountId(string accountId)
        {
            try
            {
                if (string.IsNullOrEmpty(accountId))
                {
                    return BadRequest(new { message = "AccountId is required." });
                }

                var notifications = await _notificationService.GetNotificationsByAccountIdAsync(accountId);
                var response = _mapper.Map<List<NotificationResponse>>(notifications);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // API 3: Đánh dấu một thông báo là đã đọc
        //[Authorize(Roles = "Admin, Owner, Staff, Customer")]
        [HttpPut("mark-as-read")]
        public async Task<IActionResult> MarkNotificationAsRead([FromBody] MarkNotificationAsReadRequest request)
        {
            try
            {
                var notification = await _notificationService.MarkNotificationAsReadAsync(request.NotificationID);
                var response = _mapper.Map<NotificationResponse>(notification);

                return Ok(new { message = "Notification marked as read.", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // API 4: Đánh dấu tất cả thông báo của một tài khoản là đã đọc
        //[Authorize(Roles = "Admin, Owner, Staff, Customer")]
        [HttpPut("mark-all-as-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead([FromBody] MarkAllNotificationsAsReadRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AccountID))
                {
                    return BadRequest(new { message = "AccountId is required." });
                }

                await _notificationService.MarkAllNotificationsAsReadAsync(request.AccountID);
                return Ok(new { message = "All notifications marked as read." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}