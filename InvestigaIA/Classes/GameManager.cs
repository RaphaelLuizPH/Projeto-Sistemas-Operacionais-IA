using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestigaIA.API;
using Microsoft.Extensions.DependencyInjection;

namespace InvestigaIA.Classes
{
    public class GameManager
    {
        private GeminiService _geminiClient;
        private IServiceProvider _provider;
        public GameManager(GeminiService geminiClient, IServiceProvider provider)
        {
            _provider = provider;

            _geminiClient = geminiClient;
        }

        public Dictionary<string, GameInstance> Games = new();

        public GameInstance? GetGame(string id)
        {
            if (Games.ContainsKey(id))
            {
                return Games[id];
            }
            return null;
        }

        public void Init()
        {
            Console.WriteLine("GameManager initialized.");
        }
        public async Task CreateGame()
        {
            var id = Ulid.NewUlid().ToString();

            Games.Add(id, new GameInstance(_provider.GetRequiredService<GeminiService>()));
        }



    }
}