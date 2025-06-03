using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace InvestigaIA.Classes
{


    public class GameHub(GameManager gameManager) : Hub
    {

        private readonly GameManager _gameManager = gameManager;

        public async Task SendMessage(string message, string gameId)
        {
            // Example method for clients to send messages
            await Clients.Group(gameId).SendAsync("ReceiveMessage", message);
        }

        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Caller.SendAsync("ReceiveMessage", $"Joined game {gameId}");
        }

        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            await Clients.Caller.SendAsync("ReceiveMessage", $"Left game {gameId}");
        }
    }

}