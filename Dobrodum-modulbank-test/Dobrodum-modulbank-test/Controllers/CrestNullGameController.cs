using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public uint PostGame()
        {
            var newGame = new CrestNullGame(fieldSize, winningLength, percentageOfOccupiedCells);
            appDbContext.Games.Add(newGame);
            appDbContext.SaveChanges();
            return newGame.Id;
        }

        [HttpPost("moves")]
        public IActionResult MakeMove(uint id, uint x, uint y, char symbol)
        {
            try
            {
                var game = appDbContext.Games.Find(id);
                game.NextMove(x, y, symbol);
                appDbContext.Games.Update(game);
                appDbContext.SaveChanges();
                return Ok((game.GameState, game.Round));
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
            {            
                return Ok(appDbContext.Games.Find(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
