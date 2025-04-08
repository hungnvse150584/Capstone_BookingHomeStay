using BusinessObject.Model;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GreenRoam.Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.Identity?.Name; // Lấy userId từ token (nếu dùng authentication)
            if (string.IsNullOrEmpty(userId))
            {
                userId = Context.GetHttpContext().Request.Query["userId"];
            }

            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId != null)
            {
                _userConnections.TryRemove(userId, out _);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterUser(string userId)
        {
            _userConnections[userId] = Context.ConnectionId;
        }

        // Phương thức để gửi thông báo đến một người dùng cụ thể
        public async Task SendNotification(string userId, string title, string message, int notificationId, DateTime createAt, bool isRead, NotificationType typeNotify)
        {
            if (_userConnections.TryGetValue(userId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", title, message, notificationId, createAt, isRead, typeNotify);
            }
        }
    }
}