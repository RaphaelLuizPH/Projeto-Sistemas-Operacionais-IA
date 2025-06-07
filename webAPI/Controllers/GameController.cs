using InvestigaIA.Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace webAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private GameManager _gameManager;
        public GameController(GameManager gameManager)
        {
            _gameManager = gameManager;
        }


        [HttpPost("Create", Name = "CreateGame")]
        public async Task<IActionResult> CreateGame()
        {
            try
            {
                await _gameManager.CreateGame();
                var gameId = _gameManager.Games.Keys.LastOrDefault();

                return Ok(gameId);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }


        [HttpPost("Send/{id}", Name = "SendMessage")]
        public async Task<IActionResult> Send(string message, string id, string suspectName)
        {


            try
            {
                var suspect = _gameManager.Games[id].suspects.FirstOrDefault(s => s.Name == suspectName);


                var res = await _gameManager.Games[id].SendMessageGPT(message, suspect);

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
                        Suspects = gameInstance.suspects,
                        Objectives = gameInstance._objectives,

                        CaseFile = gameInstance._CaseFile,

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
                return Ok(_gameManager.Games[id].suspects);
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


                return Ok(_gameManager.Games);
            }
            catch
            {

                return StatusCode(500, "An error occurred while processing your request.");
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

        // Add more endpoints as needed
    }
}