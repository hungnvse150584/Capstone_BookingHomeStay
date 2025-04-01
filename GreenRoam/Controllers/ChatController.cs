// GreenRoam/Controllers/ChatController.cs
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("conversations/{userId}")]
    public async Task<IActionResult> GetConversations(string userId)
    {
        var conversations = await _chatService.GetConversationsByUserAsync(userId);
        var conversationResponses = new List<SimplifiedConversationResponse>();

        foreach (var conversation in conversations)
        {
            var response = _mapper.Map<SimplifiedConversationResponse>(conversation);

            if (conversation.User1ID == userId)
            {
                response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User2);
            }
            else
            {
                response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User1);
            }

            conversationResponses.Add(response);
        }

        return Ok(conversationResponses);
    }

    [HttpGet("messages/{conversationId}")]
    public async Task<IActionResult> GetMessages(int conversationId)
    {
        var messages = await _chatService.GetMessagesByConversationAsync(conversationId);
        var messageResponses = _mapper.Map<List<SimplifiedMessageResponse>>(messages);
        return Ok(messageResponses);
    }

    [HttpGet("conversation-with-homestay-owner/{customerId}/{homeStayId}")]
    public async Task<IActionResult> GetOrCreateConversationWithHomeStayOwner(string customerId, int homeStayId)
    {
        var conversation = await _chatService.GetOrCreateConversationWithHomeStayOwnerAsync(customerId, homeStayId);
        var response = _mapper.Map<SimplifiedConversationResponse>(conversation);

        if (conversation.User1ID == customerId)
        {
            response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User2);
        }
        else
        {
            response.OtherUser = _mapper.Map<SimplifiedAccountResponse>(conversation.User1);
        }

        return Ok(response);
    }

    [HttpGet("owner-conversations/{ownerId}")]
    public async Task<IActionResult> GetOwnerConversations(string ownerId)
    {
        var conversations = await _chatService.GetConversationsForOwnerAsync(ownerId);
        var conversationResponses = new List<OwnerConversationResponse>();

        foreach (var conversation in conversations)
        {
            var response = new OwnerConversationResponse
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

            // Lấy tin nhắn cuối cùng
            var messages = await _chatService.GetMessagesByConversationAsync(conversation.ConversationID);
            var lastMessage = messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
            if (lastMessage != null)
            {
                response.LastMessage = _mapper.Map<SimplifiedMessageResponse>(lastMessage);
            }

            // Lấy số lượng tin nhắn chưa đọc
            response.UnreadMessageCount = await _chatService.GetUnreadMessageCountAsync(conversation.ConversationID, ownerId);

            conversationResponses.Add(response);
        }

        return Ok(conversationResponses);
    }
}