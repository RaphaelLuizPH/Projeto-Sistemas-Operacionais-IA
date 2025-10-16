using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestigaIA.Model.Game;

namespace webAPI.Services
{
    public class GameCleanUpService : BackgroundService
    {

        private readonly GameManager _gameManager;





        public GameCleanUpService(GameManager gameManager)
        {
            _gameManager = gameManager;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while(!stoppingToken.IsCancellationRequested)
            {
                var runningsGames = _gameManager.Games.Where(g => (g.Value.CreatedAt - DateTime.Now) > TimeSpan.FromHours(4));



                foreach (var game in runningsGames)
                {

                    game.Value.Dispose();
                    _gameManager.Games.Remove(game.Key);

                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); 
            }


           

        }

    }
}