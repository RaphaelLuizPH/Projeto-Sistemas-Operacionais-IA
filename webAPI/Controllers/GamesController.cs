
using InvestigaIA.Model.Game;
using InvestigaIA.Model.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sprache;

namespace webAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private GameManager _gameManager;

 
        public GamesController(GameManager gameManager)
        {
            _gameManager = gameManager;
        }


        [HttpPost("Create", Name = "CreateGame")]
        public async Task<GenericResult<IActionResult>> CreateGame()
        {
            try
            {
                await _gameManager.CreateGame();
                var gameId = _gameManager.Games.Keys.LastOrDefault();
                return ResultFactory<IActionResult>.Success(Ok(gameId), "Game created successfully");
            }
            catch (Exception ex)
            {

                return ResultFactory<IActionResult>.Failure(BadRequest(ex.Message), "Failed to create game");
            }
        }


        [HttpGet("/{id}")]

        public async Task<GenericResult<IActionResult>> GetGame(string id)
        {
            try
            {
              var game = _gameManager.GetGame(id);

                return  ResultFactory<IActionResult>.Success(Ok(game), $"Sucessfully fetch game with id {id}") ;

            } catch(Exception ex)
            {
                return ResultFactory<IActionResult>.Failure(BadRequest(ex.Message),$"Game with id {id} not found");

                 
            }
        }


        [HttpGet(Name = "GetAll")] 

        public async Task<GenericResult<IActionResult>> GetAll()
        {

            try
            {
                var gameIds = _gameManager.Games.Where(g => g.Value.Public).Select(g => g.Key);


                return ResultFactory<IActionResult>.Success(Ok(gameIds), $"Fetch all games: {_gameManager.Count}");
            } catch(Exception ex)
            {

                return ResultFactory<IActionResult>.Failure(BadRequest(ex.Message), "Failed to fetch games");
            }


          
           
        }




        /*
                [HttpPost("Send/{id}", Name = "SendMessage")]
                public async Task<IActionResult> Send(string message, string id, string suspectName)
                {


                    try
                    {
                        var suspect = _gameManager.Games[id].Suspects.FirstOrDefault(s => s.Name == suspectName);


                         var res = await _gameManager.Games[id].(message, suspect);
                        //var res = await _gameManager.Games[id].SendMessage(message, suspect);
                        return Ok(res);


                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
                    }
                }


                [HttpGet("Get/{id}", Name = "GetGame")]
                public IActionResult Get(string id)
                {
                    try
                    {
                        if (_gameManager.Games.TryGetValue(id, out var gameInstance))
                        {


                            var gameInfo = new
                            {
                                gameInstance.CreatedAt,
                                Suspects = gameInstance.Suspects,
                                Objectives = gameInstance._objectives,
                                isRunning = gameInstance._running,
                                CaseFile = gameInstance.CaseFile,
                                EndGameStats = gameInstance.EndGameStats,

                            };
                            return Ok(gameInfo);
                        }


                        return NotFound($"Game with ID {id} not found.");

                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
                    }
                }


                [HttpPatch("Start", Name = "StartGame")]
                public async Task<IActionResult> Start(string id)
                {
                    try
                    {
                        if (!_gameManager.Games.ContainsKey(id))
                        {
                            return NotFound($"Game with ID {id} not found.");
                        }


                        await _gameManager.Games[id].StartGame();
                        return Ok(_gameManager.Games[id].Suspects);
                    }
                    catch (Exception ex)
                    {

                        // Log the exception (ex) if needed
                        return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
                    }



                }


                [HttpGet("List", Name = "ListGames")]
                public async Task<IActionResult> ListGames()
                {
                    try
                    {


                        return Ok(_gameManager.Games.Select(g => new
                        {
                            g.Key,
                            g.Value.CreatedAt,
                            title = g.Value.CaseFile is not null ? g.Value.CaseFile.Title : "Jogo não iniciado",
                            g.Value.EndGameStats
                        }).ToList());
                    }
                    catch (Exception ex)
                    {

                        return StatusCode(500, "Error retrieving game list: " + ex.Message);
                    }


                }

                [HttpPost("end-game/{gameId}")]
                public async Task<IActionResult> EndGame(string gameId, string nome)
                {

                    try
                    {
                        var gameInstance = _gameManager.GetGame(gameId);
                        if (gameInstance == null)
                        {
                            return NotFound(new { Message = "Game instance not found." });
                        }

                        var acusado = gameInstance.Suspects.FirstOrDefault(s => s.Name == nome);

                        var res = await gameInstance.EndGame(acusado);
                        return Ok(res);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { Message = "An error occurred while ending the game.", Error = ex.Message });
                    }
                }

                // [HttpGet("/Info", Name = "GetGameStatus")]
                // public async Task<IActionResult> Get(string id)
                // {
                //     try
                //     {


                //         // return Ok(new GameInfo
                //         // {
                //         //     time = _gameManager.time,
                //         //     suspects = _gameManager.suspects,
                //         //     _CaseFile = _gameManager._CaseFile
                //         // });
                //     }
                //     catch
                //     {

                //         return StatusCode(500, "An error occurred while processing your request.");
                //     }


                // }

                */
    }
}