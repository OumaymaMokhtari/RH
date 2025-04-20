// using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.SignalR;
using ChatAppBackend.Models;
using ChatAppBackend.Data;
using System;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private readonly MongoDbContext _context;

    public ChatHub(MongoDbContext context)
    {
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
 
        // var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var userId = Context.GetHttpContext()?.Request.Query["userId"];

        if (!string.IsNullOrEmpty(userId))
        {
            Console.WriteLine($"Utilisateur {userId} connecté avec ConnectionId: {Context.ConnectionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }
        else
        {
            Console.WriteLine("Connexion détectée sans userId !");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"];
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string receiverId, string content)
    {
        // var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var senderId = Context.GetHttpContext()?.Request.Query["userId"];

        if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId))
        {
            Console.WriteLine("Erreur : senderId ou receiverId manquant !");
            return;
        }

        try
        {
            var message = new Message
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

            await _context.Messages.InsertOneAsync(message);
            Console.WriteLine($"Message enregistré : {content} de {senderId} vers {receiverId}");

            await Clients.Group(receiverId).SendAsync("ReceiveMessage", senderId, receiverId, content, message.Timestamp);
            await Clients.Group(senderId).SendAsync("ReceiveMessage", senderId, receiverId, content, message.Timestamp);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur SignalR : {ex.Message}");
        }
    }
}
