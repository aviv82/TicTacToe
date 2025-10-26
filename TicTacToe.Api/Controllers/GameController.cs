using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Requests;
using TicTacToe.Api.Responses;
using TicTacToe.Core;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class GameController :ControllerBase
{
    private readonly GameEngine _engine;

    public GameController(GameEngine engine)
    {
        _engine = engine;
    }
    
    [HttpGet("state")]
    public IActionResult GetGameState() => Ok(new Status(_engine.Status, _engine.CurrentPlayer));

    [HttpPost("start")]
    [ProducesResponseType(typeof(Status), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Warning), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Start([FromBody] StartRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Player1Name)
            || string.IsNullOrWhiteSpace(request.Player2Name)
            || request.BoardSize < 3
            || request.BoardSize > 9)
        {
            return BadRequest(new Warning("Invalid input data"));
        }

        if (request.Player1Name == request.Player2Name)
        {
            return BadRequest(new Warning("Players cannot have the same name"));
        }

        var player1 = new Player(request.Player1Name, 'X');
        var player2 = new Player(request.Player2Name, 'O');
        _engine.SetPlayers(player1, player2);
        _engine.SetBoardSize(request.BoardSize);
        return CreatedAtAction(nameof(GetGameState), new Status(_engine.Status, _engine.CurrentPlayer));
    }
    
    [HttpPost("move")]
    [ProducesResponseType(typeof(Status), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Warning), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult MakeMove([FromBody] int position)
    {
        if (_engine.Status != GameStatus.InProgress)
            return BadRequest();

        if (!_engine.TryPlayMove(position))
            return BadRequest(new Warning("Invalid move"));

        return Ok(new Status(_engine.Status, _engine.CurrentPlayer));
    }

    [HttpPost("undo")]
    [ProducesResponseType(typeof(Status), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Warning), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Undo()
    {
        if (!_engine.TryUndoLastMove())
            return BadRequest(new Warning("No moves to undo"));

        return Ok(new Status(_engine.Status, _engine.CurrentPlayer));
    }
}