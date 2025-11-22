using InvestigaIA.Model.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace InvestigaIA.Model.Game
{

    [Authorize]
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
            await Groups.AddToGroupAsync(Context.UserIdentifier, gameId);

            

            await Clients.Caller.SendAsync("ReceiveMessage", $"Joined game {gameId}");
        }


        public async Task OpenChat(string gameId, string suspectId)
        {
            string chatGroupName = $"{gameId}_chat_{suspectId}";

        

            try
            {
              


                await Groups.AddToGroupAsync(Context.UserIdentifier, chatGroupName);

                var chat = _gameManager.GetChat(gameId, suspectId);

                await Clients.Caller.SendAsync("ReceiveChatHistory", chat);

                await Clients.Caller.SendAsync("ReceiveMessage", "Chat opened.");
            } catch(Exception ex)
            {
                await Clients.Group(chatGroupName).SendAsync("ReceiveMessage", ex);
            }

           

        }


        public async Task SendMessage(SendMessageRequest messageRequest)
        {

            string chatGroupName = $"{messageRequest.gameId}_chat_{messageRequest.chatId}";

            var game = _gameManager.GetGame(messageRequest.gameId);

            if(game == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", $"Game with ID {messageRequest.gameId} not found.");
                return;
            }

       


            var request = new AskRequest()
            {
                GameId = messageRequest.gameId,
                SuspectID = messageRequest.chatId,
                Message = messageRequest.message,
                SenderID = Context.UserIdentifier,
                Sender = Context.User.FindFirst(ClaimTypes.Name).Value ?? "User",
                ChatId = chatGroupName

            };

    

            await game.Ask(request);

           
        }


        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            await Clients.Group(gameId).SendAsync("ReceiveMessage", $"Left game {gameId}");
           
        }



    }

}