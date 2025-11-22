using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestigaIA.Model.Game;

namespace webAPI.Services
{
    public class GameAutoSave : BackgroundService
    {

        private readonly GameManager _gameManager;





        public GameAutoSave(GameManager gameManager)
        {
            _gameManager = gameManager;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {




            while (!stoppingToken.IsCancellationRequested)
            {
                var runningsGames = _gameManager.Games;



                foreach (var game in runningsGames)
                {
                    await game.Value.Serialize();

                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }




        }

    }
}