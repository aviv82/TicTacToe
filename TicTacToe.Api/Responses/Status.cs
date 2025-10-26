using TicTacToe.Core;

namespace TicTacToe.Api.Responses;

public record Status(GameStatus GameStatus, Player CurrentPlayer);