using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace InvestigaIA.Model.Game
{


    public class GameHub(GameManager gameManager) : Hub
    {

        private readonly GameManager _gameManager = gameManager;


        public async Task EraseGame(string gameId)
        {
            if (_gameManager.Games.Remove(gameId))
            {

                await Clients.Group(gameId).SendAsync("ReceiveMessage", $"Game {gameId} erased.");
            }
            else
            {
                await Clients.Group(gameId).SendAsync("ReceiveMessage", $"Game {gameId} not found.");
            }
        }


        public async Task JoinGame(string gameId, string suspectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId + suspectId);

       
            await Clients.Caller.SendAsync("ReceiveMessage", $"Joined game {gameId}");
        }

        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            await Clients.Caller.SendAsync("ReceiveMessage", $"Left game {gameId}");
        }


       
    }

}