namespace TicTacToe.Api.Requests;

public record StartRequest (string Player1Name, string Player2Name, int BoardSize);
