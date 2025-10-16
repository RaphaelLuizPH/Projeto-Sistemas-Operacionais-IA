using InvestigaIA.Model.Game;
using InvestigaIA.Model.Infrastructure;
using InvestigaIA.Model.Utilities;
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

        public async Task<GenericResult<IActionResult>> Ask(AskRequest request)
        {
            try
            {
             
                var game = _gameManager.GetGame(request.GameId);

                if (game == null)
                {
                    return ResultFactory<IActionResult>.Failure(NotFound($"Game with ID {request.GameId} not found."), $"Game with ID {request.GameId} not found.", 404)   ;
                }



                var response = await game.Ask(request);

                return ResultFactory<IActionResult>.Success(Ok(response), "Send message and receive answer from the model");
            }
            catch (Exception ex)
            {
                return  ResultFactory<IActionResult>.Failure(BadRequest($"An error occurred while processing your request: {ex.Message}"), $"An error occurred while processing your request", 400);
            }


        }
    }
}
