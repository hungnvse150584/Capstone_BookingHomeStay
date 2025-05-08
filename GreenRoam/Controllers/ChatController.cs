// GreenRoam/Controllers/ChatController.cs
using AutoMapper;
using BusinessObject.Model;
using GreenRoam.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.Response.Conversation;
using System.ComponentModel.DataAnnotations;


[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IMapper _mapper;
    private readonly IHomeStayRepository _homeStayRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IStaffRepository _staffRepository;

    public ChatController(
        IChatService chatService,
        IMapper mapper,
        IHomeStayRepository homeStayRepository,
        IAccountRepository accountRepository,
        IStaffRepository staffRepository)
    {
        _chatService = chatService;
        _mapper = mapper;
        _homeStayRepository = homeStayRepository;
        _accountRepository = accountRepository;
        _staffRepository = staffRepository;
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
    //[Authorize(Roles = "Owner, Staff, Customer")]
    [HttpGet("messages/{conversationId}")]
    public async Task<IActionResult> GetMessages(int conversationId)
    {
        var messages = await _chatService.GetMessagesByConversationAsync(conversationId);
        var messageResponses = _mapper.Map<List<SimplifiedMessageResponse>>(messages);
        return Ok(messageResponses);
    }

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
    //[Authorize(Roles = "Owner, Staff, Customer")]
    [HttpGet("conversations/by-customer/{customerId}")]
    public async Task<IActionResult> GetAllChatByCustomerId(string customerId)
    {
        try
        {
            if (string.IsNullOrEmpty(customerId))
            {
                return BadRequest(new { message = "CustomerId is required." });
            }

            var conversations = await _chatService.GetConversationsByCustomerIdAsync(customerId);

            if (conversations == null || !conversations.Any())
            {
                return Ok(new List<GetConversationResponse>());
            }

            var conversationResponses = new List<GetConversationResponse>();

            foreach (var conversation in conversations)
            {
                if (!conversation.HomeStayID.HasValue)
                {
                    continue;
                }

                var response = new GetConversationResponse
                {
                    ConversationID = conversation.ConversationID,
                    HomeStayID = conversation.HomeStayID.Value
                };

                var homeStay = await _homeStayRepository.GetByIdAsync(conversation.HomeStayID.Value);
                if (homeStay == null)
                {
                    continue;
                }

                response.HomeStayName = homeStay.Name;
                response.OwnerID = homeStay.AccountID;

                // Kiểm tra xem người nhận là nhân viên hay owner
                var receiverId = conversation.User1ID == customerId ? conversation.User2ID : conversation.User1ID;
                var staff = await _staffRepository.GetByStaffIdAccountAsync(receiverId);
                if (staff != null && staff.HomeStayID == homeStay.HomeStayID)
                {
                    response.Staff = _mapper.Map<SimplifiedStaffResponse>(staff);
                }
                else
                {
                    var owner = await _accountRepository.GetByAccountIdAsync(receiverId);
                    if (owner != null)
                    {
                        response.Owner = _mapper.Map<SimplifiedAccountResponse>(owner);
                    }
                }

                var messages = await _chatService.GetMessagesByConversationAsync(conversation.ConversationID);
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
    // API 1: Lấy danh sách cuộc trò chuyện dựa trên homeStayId
    //[Authorize(Roles = "Owner, Staff")]
    [HttpGet("conversations/by-homestay/{homeStayId}")]
    public async Task<IActionResult> GetConversationsByHomeStay(int homeStayId)
    {
        try
        {
            var ownerId = await _chatService.GetOwnerIdByHomeStayIdAsync(homeStayId);
            if (ownerId == null)
            {
                return BadRequest(new { message = "Owner not found for this HomeStay." });
            }

            var filteredConversations = await _chatService.GetConversationsByHomeStayIdAsync(homeStayId);

            var conversationResponses = new List<ConversationWithMessagesResponse>();

            foreach (var conversation in filteredConversations)
            {
                var response = new ConversationWithMessagesResponse
                {
                    ConversationID = conversation.ConversationID
                };

                if (conversation.User1ID == ownerId)
                {
                    response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User2);
                }
                else if (conversation.User2ID == ownerId)
                {
                    response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User1);
                }
                else
                {
                    response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User1);
                }

                var messages = await _chatService.GetMessagesByConversationAsync(conversation.ConversationID);
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

    //[Authorize(Roles = "Owner")]
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
    //[Authorize(Roles = "Owner, Staff, Customer")]
    [HttpPut("mark-as-read")]
    public async Task<IActionResult> MarkAllMessagesAsRead([FromBody] MarkAsReadRequest request)
    {
        try
        {
            // Lấy hoặc tạo cuộc trò chuyện giữa customerId và owner của homeStayId
            var conversation = await _chatService.GetOrCreateConversationWithHomeStayOwnerAsync(request.SenderId, request.HomeStayId);
            if (conversation == null)
            {
                return BadRequest(new { message = "Conversation not found." });
            }

            // Đánh dấu tất cả tin nhắn là đã đọc
            await _chatService.MarkAllMessagesAsReadAsync(conversation.ConversationID, request.SenderId);
            return Ok(new { message = "Messages marked as read." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    //[Authorize(Roles = "Owner, Staff, Customer")]
    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromForm] SendMessageRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.ReceiverID) || string.IsNullOrEmpty(request.SenderName) || request.HomeStayId <= 0)
            {
                return BadRequest(new { message = "ReceiverID, SenderName, and HomeStayId are required." });
            }

            string receiverId;
            var staffList = await _staffRepository.GetStaffByHomeStayIdAsync(request.HomeStayId); // Lấy danh sách nhân viên
            var staffId = staffList?.FirstOrDefault()?.StaffIdAccount; // Lấy StaffIdAccount từ nhân viên đầu tiên nếu có
            if (!string.IsNullOrEmpty(staffId))
            {
                receiverId = staffId; // Ưu tiên gửi tin nhắn đến nhân viên nếu có
            }
            else
            {
                receiverId = await _chatService.GetOwnerIdByHomeStayIdAsync(request.HomeStayId);
                if (receiverId == null)
                {
                    return BadRequest(new { message = "Owner not found for this HomeStay." });
                }
            }

            var message = await _chatService.SendMessageAsync(
                request.SenderID,
                receiverId,
                request.Content,
                request.SenderName,
                request.HomeStayId,
                request.Images
            );

            var hubContext = (IHubContext<ChatHub>)HttpContext.RequestServices.GetService(typeof(IHubContext<ChatHub>));
            await hubContext.Clients.All.SendAsync(
                "ReceiveMessage",
                request.SenderID,
                message.Content,
                message.SentAt,
                message.MessageID,
                message.ConversationID,
                message.senderName,
                message.receiverID
            );

            return Ok(new { message = "Message sent successfully.", data = _mapper.Map<SimplifiedMessageResponse>(message) });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    //[Authorize(Roles = "Owner, Staff, Customer")]
    [HttpPost("create-conversation")]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.SenderID) || string.IsNullOrEmpty(request.ReceiverID) || request.HomeStayId <= 0)
            {
                return BadRequest(new { message = "SenderID, ReceiverID, and HomeStayId are required." });
            }

            string receiverId;
            var staff = await _staffRepository.GetByStaffIdAccountAsync(request.ReceiverID);
            if (staff != null && staff.HomeStayID == request.HomeStayId)
            {
                receiverId = request.ReceiverID; // Chat với nhân viên nếu đúng HomeStay
            }
            else
            {
                receiverId = await _chatService.GetOwnerIdByHomeStayIdAsync(request.HomeStayId);
                if (receiverId == null)
                {
                    return BadRequest(new { message = "Owner not found for this HomeStay." });
                }
            }

            var conversation = await _chatService.GetOrCreateConversationAsync(
                request.SenderID,
                receiverId,
                request.HomeStayId
            );

            var response = _mapper.Map<ConversationResponse>(conversation);

            return Ok(new { message = "Conversation created successfully.", data = response });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}

//public class MaxImagesAttribute : ValidationAttribute
//{
//    private readonly int _maxCount;

//    public MaxImagesAttribute(int maxCount)
//    {
//        _maxCount = maxCount;
//    }

//    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//    {
//        var images = value as List<IFormFile>;
//        if (images != null && images.Count > _maxCount)
//        {
//            return new ValidationResult($"The number of images cannot exceed {_maxCount}.");
//        }
//        return ValidationResult.Success;
//    }
//}
//public class AtLeastOneContentAttribute : ValidationAttribute
//{
//    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//    {
//        var request = (SendMessageRequest)validationContext.ObjectInstance;

//        // Kiểm tra nếu cả Content và Images đều rỗng
//        bool hasContent = !string.IsNullOrWhiteSpace(request.Content);
//        bool hasImages = request.Images != null && request.Images.Any();

//        if (!hasContent && !hasImages)
//        {
//            return new ValidationResult("Either Content or Images must be provided.");
//        }

//        return ValidationResult.Success;
//    }
//}

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
    public string SenderId { get; set; }
    public int HomeStayId { get; set; }
}

public class SendMessageRequest
{
    [Required(ErrorMessage = "ReceiverID is required.")]
    public string ReceiverID { get; set; }

    public string SenderName { get; set; }
    [Required(ErrorMessage = "SenderID is required.")]
    public string SenderID { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "HomeStayId must be greater than 0.")]
    public int HomeStayId { get; set; }
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Content cannot be empty or contain only whitespace.")]
    //[Range(1, int.MaxValue, ErrorMessage = "ConversationID must be greater than 0.")]
    //public int ConversationID { get; set; }
    public string? Content { get; set; }
    //[MaxImages(10, ErrorMessage = "You can only upload a maximum of 10 images at a time.")]
    public List<IFormFile>? Images { get; set; }
    //[AtLeastOneContent]
    //public bool ValidateAtLeastOneContent => true;
}
public class CreateConversationRequest
{
    [Required(ErrorMessage = "ReceiverID is required.")]
    public string ReceiverID { get; set; }

    [Required(ErrorMessage = "SenderID is required.")]
    public string SenderID { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "HomeStayId must be greater than 0.")]
    public int HomeStayId { get; set; }
    public DateTime CreatedAt { get; set; }

}
public class GetConversationResponse
{
    public int ConversationID { get; set; }
    public int HomeStayID { get; set; } // Thêm HomeStayID
    public string HomeStayName { get; set; } // Thêm tên homestay
    public string OwnerID { get; set; }
    public SimplifiedAccountResponse Owner { get; set; } // Chủ homestay (thay thế OtherUser)
    public SimplifiedStaffResponse Staff { get; set; }
    public SimplifiedMessageResponse LastMessage { get; set; }
}
