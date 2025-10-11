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
        public GameManager(IServiceProvider provider)
        {
            _provider = provider;

        }

        public Dictionary<string, GameInstance> Games = new();

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

           Games.Add(id, new GameInstance(_provider.GetRequiredService<GeminiService>(), id, _provider.GetRequiredService<IHubContext<GameHub>>()));
        }



    }
}




