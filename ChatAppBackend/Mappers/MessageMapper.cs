using ChatAppBackend.Models;
using ChatAppBackend.ViewModels;

namespace ChatAppBackend.Mappers;

public static class MessageMapper
{
    public static MessageVM ToVM(Message message) => new()
    {
        SenderId = message.SenderId,
        ReceiverId = message.ReceiverId,
        Content = message.Content,
        Timestamp = message.Timestamp
    };
}
