using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Messages;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private const int MaxPageSize = 10;

        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;

        public MessageService(IMessageRepository messageRepo, IUserRepository userRepo)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
        }

        public async Task<Result<MessageDto>> SendMessageAsync(int senderId, SendMessageRequest request)
        {
            if (senderId == request.ReceiverId)
                return new Result<MessageDto> { IsSuccess = false, Message = "You cannot message yourself.", StatusCode = 400 };

            var receiver = await _userRepo.GetPublicByIdAsync(request.ReceiverId!.Value);
            if (receiver == null)
                return new Result<MessageDto> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = request.ReceiverId!.Value,
                Content = request.Content,
                IsRead = false
            };

            await _messageRepo.AddAsync(message);
            await _messageRepo.SaveChangesAsync();

            return new Result<MessageDto> { Data = ToDto(message, senderId), IsSuccess = true, Message = "Message sent successfully.", StatusCode = 201 };
        }

        public async Task<Result<MessageListDto>> GetConversationAsync(int userId, int otherUserId, int page, int pageSize)
        {
            var other = await _userRepo.GetPublicByIdAsync(otherUserId);
            if (other == null)
                return new Result<MessageListDto> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            if (page < 1)
                return new Result<MessageListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<MessageListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _messageRepo.CountConversationAsync(userId, otherUserId);
            var messages = await _messageRepo.GetConversationAsync(userId, otherUserId, page, pageSize);

            var result = new MessageListDto
            {
                Messages = messages.Select(m => ToDto(m, userId)),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<MessageListDto> { Data = result, IsSuccess = true, Message = "Conversation retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<ConversationListDto>> GetConversationsAsync(int userId, int page, int pageSize)
        {
            if (page < 1)
                return new Result<ConversationListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<ConversationListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _messageRepo.CountConversationPartnersAsync(userId);
            var totalUnreadCount = await _messageRepo.CountUnreadTotalAsync(userId);
            var partnerIds = await _messageRepo.GetConversationPartnerIdsPageAsync(userId, page, pageSize);

            var latestMessages = await _messageRepo.GetLatestMessagesAsync(userId, partnerIds);
            var unreadCounts = await _messageRepo.CountUnreadByPartnersAsync(userId, partnerIds);
            var partners = await _userRepo.GetByIdsWithProfileAsync(partnerIds);

            // partnerIds is already ordered newest-conversation-first from the repo query,
            // and .Select preserves that order, so no re-sort is needed here.
            var conversations = partnerIds
                .Where(id => latestMessages.ContainsKey(id) && partners.ContainsKey(id))
                .Select(id => new ConversationDto
                {
                    PartnerId = id,
                    PartnerUsername = partners[id].Username,
                    PartnerProfileImageUrl = partners[id].Profile?.ProfileImageUrl,
                    LastMessage = latestMessages[id].Content,
                    LastMessageAt = latestMessages[id].CreatedAt,
                    UnreadCount = unreadCounts.GetValueOrDefault(id)
                });

            var result = new ConversationListDto
            {
                Conversations = conversations,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalUnreadCount = totalUnreadCount
            };

            return new Result<ConversationListDto> { Data = result, IsSuccess = true, Message = "Conversations retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> MarkConversationAsReadAsync(int userId, int otherUserId)
        {
            var other = await _userRepo.GetPublicByIdAsync(otherUserId);
            if (other == null)
                return new Result<object> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            await _messageRepo.MarkConversationAsReadAsync(userId, otherUserId);

            return new Result<object> { IsSuccess = true, Message = "Conversation marked as read.", StatusCode = 200 };
        }

        public async Task<Result<int>> GetUnreadCountAsync(int userId)
        {
            var count = await _messageRepo.CountUnreadTotalAsync(userId);
            return new Result<int> { Data = count, IsSuccess = true, Message = "Unread count retrieved successfully.", StatusCode = 200 };
        }

        private static MessageDto ToDto(Message message, int currentUserId) => new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            Content = message.Content,
            IsRead = message.IsRead,
            IsMine = message.SenderId == currentUserId,
            CreatedAt = message.CreatedAt
        };
    }
}