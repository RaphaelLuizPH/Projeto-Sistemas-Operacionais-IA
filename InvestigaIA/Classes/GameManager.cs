using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestigaIA.API;
using Microsoft.AspNetCore.SignalR;
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
            while (Games.Count > 0)
            {
                Console.Write("Game tick at " + DateTime.Now.ToString("HH:mm:ss"));
            }

        }
        public async Task CreateGame()
        {
            var id = Ulid.NewUlid().ToString();

            Games.Add(id, new GameInstance(_provider.GetRequiredService<GeminiService>(), id, _provider.GetRequiredService<IHubContext<GameHub>>(), _provider.GetRequiredService<OpenAiService>()));
        }



    }
}