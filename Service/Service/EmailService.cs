// Service/EmailService.cs
using BusinessObject.Model;
using Repository;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;
using Repository.IRepositories;
using Service.IService;

namespace Service
{
    public class EmailService : IEmailService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IConfiguration _configuration;

        public EmailService(IBookingRepository bookingRepository, IConfiguration configuration)
        {
            _bookingRepository = bookingRepository;
            _configuration = configuration;
        }

        public async Task SendCheckInReminderEmailAsync()
        {
            var bookings = await _bookingRepository.GetBookingsForCheckInReminderAsync();

            foreach (var booking in bookings)
            {
                if (booking.Account == null || string.IsNullOrEmpty(booking.Account.Email))
                {
                    continue;
                }

                var checkInDate = booking.BookingDetails?.FirstOrDefault()?.CheckInDate;
                var checkOutDate = booking.BookingDetails?.FirstOrDefault()?.CheckOutDate;

                if (checkInDate == null || checkOutDate == null)
                {
                    continue;
                }

                StringBuilder emailMessage = new StringBuilder();
                emailMessage.Append("<html>");
                emailMessage.Append("<body>");
                emailMessage.Append($"<p>Kính gửi {booking.Account.Name},</p>");
                emailMessage.Append("<p>Chúng tôi xin nhắc nhở bạn rằng ngày check-in của bạn tại homestay đang đến gần.</p>");
                emailMessage.Append("<h3>Thông tin đặt phòng:</h3>");
                emailMessage.Append($"<p>- Homestay: {booking.HomeStay.Name}</p>");
                emailMessage.Append($"<p>- Địa chỉ: {booking.HomeStay.Address}</p>");
                emailMessage.Append($"<p>- Ngày check-in: {checkInDate.Value:dd/MM/yyyy}</p>");
                emailMessage.Append($"<p>- Ngày check-out: {checkOutDate.Value:dd/MM/yyyy}</p>");
                emailMessage.Append($"<p>- Tổng giá: {booking.Total} VND</p>");
                emailMessage.Append("<p>Vui lòng chuẩn bị và liên hệ với chúng tôi nếu bạn có bất kỳ câu hỏi nào.</p>");
                emailMessage.Append("<br>");
                emailMessage.Append("<p>Trân trọng,</p>");
                emailMessage.Append("<p><strong>GreenRoam Support Team</strong></p>");
                emailMessage.Append("</body>");
                emailMessage.Append("</html>");

                string message = emailMessage.ToString();

                await SendEmailAsync(booking.Account.Email, "Nhắc nhở: Sắp đến ngày check-in tại GreenRoam", message);
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var senderName = _configuration["EmailSettings:SenderName"];

            var email = new MimeMessage();
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.From.Add(MailboxAddress.Parse(senderEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(senderEmail, senderPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}