// GreenRoam/Controllers/ChatController.cs
using AutoMapper;
using GreenRoam.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.IService;
using Service.RequestAndResponse.Response.Conversation;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IMapper _mapper;

    public ChatController(IChatService chatService, IMapper mapper)
    {
        _chatService = chatService;
        _mapper = mapper;
    }

    //[HttpGet("conversations/{userId}")]
    //public async Task<IActionResult> GetConversations(string userId)
    //{
    //    var conversations = await _chatService.GetConversationsByUserAsync(userId);
    //    var conversationResponses = new List<SimplifiedConversationResponse>();

    //    foreach (var conversation in conversations)
    //    {
    //        var response = _mapper.Map<SimplifiedConversationResponse>(conversation);

    //        if (conversation.User1ID == userId)
    //        {
    //            response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User2);
    //        }
    //        else
    //        {
    //            response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User1);
    //        }

    //        conversationResponses.Add(response);
    //    }

    //    return Ok(conversationResponses);
    //}

    //[HttpGet("messages/{conversationId}")]
    //public async Task<IActionResult> GetMessages(int conversationId)
    //{
    //    var messages = await _chatService.GetMessagesByConversationAsync(conversationId);
    //    var messageResponses = _mapper.Map<List<SimplifiedMessageResponse>>(messages);
    //    return Ok(messageResponses);
    //}

    //[HttpGet("conversation-with-homestay-owner/{customerId}/{homeStayId}")]
    //public async Task<IActionResult> GetOrCreateConversationWithHomeStayOwner(string customerId, int homeStayId)
    //{
    //    var conversation = await _chatService.GetOrCreateConversationWithHomeStayOwnerAsync(customerId, homeStayId);
    //    var response = _mapper.Map<SimplifiedConversationResponse>(conversation);

    //    if (conversation.User1ID == customerId)
    //    {
    //        response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User2);
    //    }
    //    else
    //    {
    //        response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User1);
    //    }

    //    return Ok(response);
    //}

    //[HttpGet("owner-conversations/{ownerId}")]
    //public async Task<IActionResult> GetOwnerConversations(string ownerId)
    //{
    //    var conversations = await _chatService.GetConversationsForOwnerAsync(ownerId);
    //    var conversationResponses = new List<OwnerConversationResponse>();

    //    foreach (var conversation in conversations)
    //    {
    //        var response = new OwnerConversationResponse
    //        {
    //            ConversationID = conversation.ConversationID
    //        };

    //        if (conversation.User1ID == ownerId)
    //        {
    //            response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User2);
    //        }
    //        else
    //        {
    //            response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User1);
    //        }

    //        var messages = await _chatService.GetMessagesByConversationAsync(conversation.ConversationID);
    //        var lastMessage = messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
    //        if (lastMessage != null)
    //        {
    //            response.LastMessage = _mapper.Map<SimplifiedMessageResponse>(lastMessage);
    //        }

    //        response.UnreadMessageCount = await _chatService.GetUnreadMessageCountAsync(conversation.ConversationID, ownerId);

    //        conversationResponses.Add(response);
    //    }

    //    return Ok(conversationResponses);
    //}

    // API 1: Lấy danh sách cuộc trò chuyện dựa trên homeStayId
    [HttpGet("conversations/by-homestay/{homeStayId}")]
    public async Task<IActionResult> GetConversationsByHomeStay(int homeStayId)
    {
        try
        {
            // Lấy OwnerId từ homeStayId
            var ownerId = await _chatService.GetOwnerIdByHomeStayIdAsync(homeStayId);
            if (ownerId == null)
            {
                return BadRequest(new { message = "Owner not found for this HomeStay." });
            }

            // Lấy danh sách cuộc trò chuyện của Owner
            var conversations = await _chatService.GetConversationsByUserAsync(ownerId);
            var conversationResponses = new List<ConversationWithMessagesResponse>();

            foreach (var conversation in conversations)
            {
                var response = new ConversationWithMessagesResponse
                {
                    ConversationID = conversation.ConversationID
                };

                // Xác định OtherUser
                if (conversation.User1ID == ownerId)
                {
                    response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User2);
                }
                else
                {
                    response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User1);
                }

                // Lấy tất cả tin nhắn của cuộc trò chuyện
                var messages = await _chatService.GetMessagesByConversationAsync(conversation.ConversationID);
                //response.Messages = _mapper.Map<List<SimplifiedMessageResponse>>(messages);

                // Lấy tin nhắn cuối cùng
                var lastMessage = messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
                if (lastMessage != null)
                {
                    response.LastMessage = _mapper.Map<SimplifiedMessageResponse>(lastMessage);
                }

                conversationResponses.Add(response);
            }

            return Ok(conversationResponses);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    //[HttpGet("conversations/by-homestay/{homeStayId}")]
    //public async Task<IActionResult> GetConversationsByHomeStay(int homeStayId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    //{
    //    try
    //    {
    //        var ownerId = await _chatService.GetOwnerIdByHomeStayIdAsync(homeStayId);
    //        if (ownerId == null)
    //        {
    //            return BadRequest(new { message = "Owner not found for this HomeStay." });
    //        }

    //        var conversations = await _chatService.GetConversationsByUserAsync(ownerId);
    //        var conversationResponses = new List<ConversationWithMessagesResponse>();

    //        foreach (var conversation in conversations)
    //        {
    //            var response = new ConversationWithMessagesResponse
    //            {
    //                ConversationID = conversation.ConversationID
    //            };

    //            if (conversation.User1ID == ownerId)
    //            {
    //                response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User2);
    //            }
    //            else
    //            {
    //                response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User1);
    //            }

    //            // Lấy tin nhắn với phân trang
    //            var messages = await _chatService.GetMessagesByConversationAsync(conversation.ConversationID);
    //            var pagedMessages = messages
    //                .OrderByDescending(m => m.SentAt)
    //                .Skip((pageIndex - 1) * pageSize)
    //                .Take(pageSize)
    //                .ToList();
    //            response.Messages = _mapper.Map<List<SimplifiedMessageResponse>>(pagedMessages);

    //            var lastMessage = messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
    //            if (lastMessage != null)
    //            {
    //                response.LastMessage = _mapper.Map<SimplifiedMessageResponse>(lastMessage);
    //            }

    //            conversationResponses.Add(response);
    //        }

    //        return Ok(conversationResponses);
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //}

    // API 2: Lấy lịch sử tin nhắn dựa trên customerId và homeStayId
    // GreenRoam/Controllers/ChatController.cs
    [HttpGet("OwnerMessage")]
    public async Task<IActionResult> GetMessagesByCustomerAndHomeStay([FromQuery] string customerId, [FromQuery] int homeStayId)
    {
        try
        {
            var conversation = await _chatService.GetOrCreateConversationWithHomeStayOwnerAsync(customerId, homeStayId);
            if (conversation == null)
            {
                return BadRequest(new { message = "Conversation not found." });
            }

            var messages = await _chatService.GetMessagesByConversationAsync(conversation.ConversationID);
            Console.WriteLine($"Retrieved {messages.Count} messages for ConversationID {conversation.ConversationID}");
            var sortedMessages = messages.OrderByDescending(m => m.SentAt).ToList();
            var messageResponses = _mapper.Map<List<SimplifiedMessageResponse>>(sortedMessages);
            return Ok(messageResponses);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // API 3: Đánh dấu tất cả tin nhắn là đã đọc
    [HttpPut("mark-as-read")]
    public async Task<IActionResult> MarkAllMessagesAsRead([FromBody] MarkAsReadRequest request)
    {
        try
        {
            // Lấy hoặc tạo cuộc trò chuyện giữa customerId và owner của homeStayId
            var conversation = await _chatService.GetOrCreateConversationWithHomeStayOwnerAsync(request.CustomerId, request.HomeStayId);
            if (conversation == null)
            {
                return BadRequest(new { message = "Conversation not found." });
            }

            // Đánh dấu tất cả tin nhắn là đã đọc
            await _chatService.MarkAllMessagesAsReadAsync(conversation.ConversationID, request.CustomerId);
            return Ok(new { message = "Messages marked as read." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // API 4: Gửi tin nhắn
    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            // Lấy ownerId từ homeStayId
            var ownerId = await _chatService.GetOwnerIdByHomeStayIdAsync(request.HomeStayId);
            if (ownerId == null)
            {
                return BadRequest(new { message = "Owner not found for this HomeStay." });
            }

            // Gửi tin nhắn từ customerId đến ownerId
            var message = await _chatService.SendMessageAsync(request.CustomerId, ownerId, request.Content);

            // Thông báo qua SignalR
            var hubContext = (IHubContext<ChatHub>)HttpContext.RequestServices.GetService(typeof(IHubContext<ChatHub>));
            await hubContext.Clients.All.SendAsync("ReceiveMessage", request.CustomerId, request.Content, message.SentAt, message.MessageID, message.ConversationID);

            return Ok(new { message = "Message sent successfully.", data = _mapper.Map<SimplifiedMessageResponse>(message) });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
public class ConversationWithMessagesResponse
{
    public int ConversationID { get; set; }
    public SimplifiedAccountResponse OtherUser { get; set; }
    public SimplifiedMessageResponse LastMessage { get; set; }
    //public List<SimplifiedMessageResponse> Messages { get; set; }
}
// Request models
public class MarkAsReadRequest
{
    public string CustomerId { get; set; }
    public int HomeStayId { get; set; }
}

public class SendMessageRequest
{
    public string CustomerId { get; set; }
    public int HomeStayId { get; set; }
    public string Content { get; set; }
}