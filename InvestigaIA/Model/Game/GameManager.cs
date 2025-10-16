using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestigaIA.API;
using InvestigaIA.API.Gemini;
using Microsoft.AspNetCore.SignalR;
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

        public void Init()
        {
            while (Games.Count > 0)
            {
                Console.Write("Game tick at " + DateTime.Now.ToString("HH:mm:ss"));
            }

        }
        public async Task CreateGame()
        {
            var id = Ulid.NewUlid().ToString();

           Games.Add(id, new GameInstance(_geminiService, id, _hubContext, _gameService));
        }



    }
}




