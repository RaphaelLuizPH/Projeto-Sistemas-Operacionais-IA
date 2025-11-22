using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestigaIA.API.Gemini;
using InvestigaIA.Model.DTOs;
using InvestigaIA.Model.Game;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace webAPI.Services
{
    public class GameAutoLoad : IHostedService
    {

        private readonly GameManager _gameManager;
        private readonly IServiceProvider _sp;
        private readonly IHubContext<GameHub> _gameHubContext;
        private readonly GameService _gameService;
  
        public GameAutoLoad(GameManager gameManager, IServiceProvider IServiceProvider, IHubContext<GameHub> gameHubContext, GameService gameService)
        {
            _gameManager = gameManager;
            _sp = IServiceProvider;
            _gameHubContext = gameHubContext;
            _gameService = gameService;
   
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {


            List<GameInstanceJSON> gamesToLoad = new List<GameInstanceJSON>();
            string folderPath = Path.Combine(AppContext.BaseDirectory, "Saves");

           
            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                try
                {
                    
                    if (!File.Exists(file))
                    {
                        Console.WriteLine($"File not found: {file}");
                        continue;
                    }


                    var jsonText = File.ReadAllText(file);


                    var save = JsonConvert.DeserializeObject<GameInstanceJSON>(jsonText);

                    if(save == null)
                    {
                        Console.WriteLine($"Failed to deserialize game from file: {file}");
                        continue;
                    }

                   


            
                    GameInstance instance = new GameInstance(save.GameId, _gameHubContext, _gameService, save);



                    _gameManager.Games.TryAdd(save.GameId, instance);


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing file {file}: {ex.Message}");
                    continue;
                }
             

            }



            

            return Task.CompletedTask;

        }

      
       
    }
}