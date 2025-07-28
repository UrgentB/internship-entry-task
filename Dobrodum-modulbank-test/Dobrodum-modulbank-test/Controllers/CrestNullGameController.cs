using Dobrodum_modulbank_test.Models;
using Dobrodum_modulbank_test.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Dobrodum_modulbank_test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrestNullGameController : ControllerBase
    {
        uint fieldSize;
        uint winningLength;
        uint percentageOfOccupiedCells;
        ApplicationSQliteContext appDbContext;
        public CrestNullGameController(IConfiguration config, ApplicationSQliteContext appDbContext)
        {
            fieldSize = Convert.ToUInt32(config["fieldSize"]);
            winningLength = Convert.ToUInt32(config["winningLenght"]);
            percentageOfOccupiedCells = Convert.ToUInt32(config["percentageOfOccupiedCells"]);
            this.appDbContext = appDbContext;
        }

        [HttpPost("newGame")]
        public IActionResult PostGame()
        {
            try
            {
                var newGame = new CrestNullGame(fieldSize, winningLength, percentageOfOccupiedCells);
                appDbContext.Games.Add(newGame);
                appDbContext.SaveChanges();
                return Ok(newGame.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("moves")]
        public IActionResult MakeMove(uint id, uint x, uint y, char symbol)
        {
            try
            {
                var game = appDbContext.Games.Find(id);
                if (game.NextMove(x, y, symbol) == 1)
                {
                    appDbContext.Games.Update(game);
                    appDbContext.SaveChanges();
                }

                var GameStateString = game.GameState.ToString();
                var etag = GenerateETag(GameStateString + game.Round.ToString());
                Response.Headers.ETag = etag;

                return Ok(new { GameState = GameStateString, game.Round });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("game")]
        public IActionResult GetGame(uint id)
        {
            try
            {   var game = appDbContext.Games.Find(id);
                if (game == null)
                    return NotFound("Игра не найдена");  
                return Ok(game);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GenerateETag(string data)
        {
            using (var sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
