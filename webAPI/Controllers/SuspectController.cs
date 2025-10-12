using InvestigaIA.Model.Game;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuspectController(GameManager gameManager) : ControllerBase
    {
        private readonly GameManager _gameManager = gameManager;

        [HttpPost("Ask", Name = "AskSuspect")]

        public async Task<IActionResult> Ask(string gameId, string suspectName, string question)
        {
            try
            {

                var game = _gameManager.GetGame(gameId);

                if (game == null)
                {
                    return NotFound($"Game with ID {gameId} not found.");
                }
                var response = await game.Ask(question, suspectName);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }


        }
    }
}
