using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestigaIA.Classes;

namespace webAPI.Services
{
    public class GameService : BackgroundService
    {

        private readonly GameManager _gameManager;





        public GameService(GameManager gameManager)
        {
            _gameManager = gameManager;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {


            _gameManager.Init();




            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

    }
}