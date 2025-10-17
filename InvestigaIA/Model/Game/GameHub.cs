using InvestigaIA.Model.Utilities;
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


        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);


            await Clients.Caller.SendAsync("ReceiveMessage", $"Joined game {gameId}");
        }


        public async Task OpenChat(string gameId, int chatID)
        {
            
        
            
            
            await Groups.AddToGroupAsync(Context.ConnectionId, chatID.ToString());

            var chat = _gameManager.GetChat(gameId, chatID);

            await Clients.Caller.SendAsync("ReceiveChatHistory", chat);

            await Clients.Caller.SendAsync("ReceiveMessage", "Chat opened.");


        }


        public async Task SendMessage(string gameId, string chatId, string message)
        {

            var game = _gameManager.GetGame(gameId);

            var request = new AskRequest()
            {
                GameId = gameId,
                SuspectID = chatId,
                Message = message,
                SenderID = Context.UserIdentifier,
                Sender = Context.User.Identity.Name ?? "User"
            };

    

            var messageAnswer = await game.Ask(request);

           
        }


        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            await Clients.Group(gameId).SendAsync("ReceiveMessage", $"Left game {gameId}");
        }



    }

}