using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestigaIA.API;
using InvestigaIA.API.Gemini;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InvestigaIA.Model.Game
{
    public class GameManager
    {
       
        private IServiceProvider _provider;

        private GeminiService _geminiService;

        private IHubContext<GameHub> _hubContext;

        private GameService _gameService;

        public Dictionary<string, GameInstance> Games = new();

        public long Count => Games.Count;


        public GameManager(IServiceProvider provider, IHubContext<GameHub> hubContext, GeminiService geminiService, GameService gameService)
        {
            _provider = provider;
            _hubContext = hubContext;
            _geminiService = geminiService;
            _gameService = gameService;
        }



        public GameInstance? GetGame(string id)
        {
            if (Games.TryGetValue(id, out GameInstance? value))
            {
                return value;
            }
            return null;
        }

   
        public async Task CreateGame(string? apiKey = null)
        {
            
            apiKey ??= _provider.GetRequiredService<IConfiguration>().GetSection("APIKey").Value;

            var id = Ulid.NewUlid().ToString();

            var geminiService = new GeminiService(apiKey);

            var game = new GameInstance(geminiService, id, _hubContext, _gameService, apiKey);

           

            Games.Add(id, game);
        }

        internal List<ChatMessage> GetChat(string gameId, string chatId)
        {
            
            
            if (Games.TryGetValue(gameId, out GameInstance? value))
            {
              
                return value.Chats.TryGetValue(chatId, out var chat) ? chat : new List<ChatMessage>(); ;

            }

            throw new Exception($"No game with id {gameId} found.");
        }
    }
}




