using Microsoft.AspNetCore.Mvc;
using ChatAppBackend.Models;
using ChatAppBackend.Data;
using ChatAppBackend.Mappers;
using ChatAppBackend.ViewModels;
using MongoDB.Driver;

namespace ChatAppBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly MongoDbContext _context;

    public MessageController(MongoDbContext context) => _context = context;

    [HttpGet("messages/{receiverId}")]
    public async Task<IActionResult> GetMessages(string receiverId)
    {
        var senderId = Request.Query["senderId"].ToString();

        var filter = Builders<Message>.Filter.Or(
            Builders<Message>.Filter.And(
                Builders<Message>.Filter.Eq(m => m.SenderId, senderId),
                Builders<Message>.Filter.Eq(m => m.ReceiverId, receiverId)
            ),
            Builders<Message>.Filter.And(
                Builders<Message>.Filter.Eq(m => m.SenderId, receiverId),
                Builders<Message>.Filter.Eq(m => m.ReceiverId, senderId)
            )
        );

        var messages = await _context.Messages
            .Find(filter)
            .SortBy(msg => msg.Timestamp)
            .ToListAsync();

        return Ok(messages.Select(MessageMapper.ToVM));
    }

    [HttpPost("sendMessage")]
    public async Task<IActionResult> SendMessage([FromBody] MessageVM messageVm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var senderId = messageVm.SenderId ?? Request.Query["senderId"].ToString();

        var message = new Message
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            SenderId = senderId,
            ReceiverId = messageVm.ReceiverId,
            Content = messageVm.Content,
            Timestamp = DateTime.UtcNow,
            IsRead = false
        };

        await _context.Messages.InsertOneAsync(message);

        return Ok(MessageMapper.ToVM(message));
    }

    [HttpPost("markAsRead")]
    public async Task<IActionResult> MarkAsRead([FromBody] MarkReadRequest request)
    {
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.SenderId, request.SenderId),
            Builders<Message>.Filter.Eq(m => m.ReceiverId, request.ReceiverId),
            Builders<Message>.Filter.Eq(m => m.IsRead, false)
        );

        var update = Builders<Message>.Update.Set(m => m.IsRead, true);
        await _context.Messages.UpdateManyAsync(filter, update);
        return Ok();
    }

    [HttpGet("unreadByUser")]
    public async Task<IActionResult> GetUnreadByUser()
    {
        var receiverId = Request.Query["receiverId"].ToString();

        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.ReceiverId, receiverId),
            Builders<Message>.Filter.Eq(m => m.IsRead, false)
        );

        var messages = await _context.Messages.Find(filter).ToListAsync();

        var grouped = messages
            .GroupBy(m => m.SenderId)
            .ToDictionary(g => g.Key, g => g.Count());

        return Ok(grouped);
    }

    public class MarkReadRequest
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
    }
}
