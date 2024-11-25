using SportSpot.V1.Session.Chat.Dtos;
using SportSpot.V1.Session.Chat.Entities;

namespace SportSpot.V1.Session.Chat.Mapper
{
    public static class MessageMapper
    {
        public static MessageDto ConvertToDto(this MessageEntity entity)
        {
            return new MessageDto
            {
                Id = entity.Id,
                SessionId = entity.SessionId,
                CreatorId = entity.CreatorId,
                Message = entity.Message,
                CreatedAt = entity.CreatedAt,
                ParentMessageId = entity.ParentMessageId
            };
        }
    }
}