using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.GameHub
{
  public struct Dimensions
  {
    public int xDim;
    public int yDim;
    public int winSize;
  }

  public struct Conditions
  {
    public string winner;
    public string direction;
    public int i;
    public int j;
  }

  public class Game
  {
    internal bool GameOver { get; set; } = false;

    internal Dimensions GameDimensions { get; set; }
    internal Conditions GameOverConditions { get; set; }

    internal string CrossesId { get; set; }
    internal string NoughtsId { get; set; }


    internal static Game CreateGame(string player1Id, string player2Id, Dimensions dims)
    {
      Game game;

      Random rand = new Random();
      bool player1FirstMove = rand.Next(0, 2) == 0;

      if (player1FirstMove) // crosses = player1
      {
        game = new Game(player1Id, player2Id, dims);
      }
      else // crosses = player2
      {
        game = new Game(player2Id, player1Id, dims);
      }

      return game;
    }

    internal static bool DimensionsAreValid(Dimensions dimensions)
    {
      if (dimensions.xDim < MinXDim || dimensions.yDim < MinYDim || dimensions.winSize < MinWinSize ||
          dimensions.xDim > MaxXDim || dimensions.yDim > MaxYDim || dimensions.winSize > MaxWinSize ||
          dimensions.winSize > Math.Min(dimensions.xDim, dimensions.yDim))
      {
        return false;
      }

      return true;
    }

    internal bool TryMakeMove(int index)
    {
      if (Cells[index] != CellTypes.Clear)
        return false;

      if (MovesCount % 2 == 0)
      {
        Cells[index] = CellTypes.Cross;
      }
      else if (MovesCount % 2 == 1)
      {
        Cells[index] = CellTypes.Nought;
      }

      MovesCount++;

      CheckGame();

      return true;
    }

    internal string GetNextMovePlayerId()
    {
      if (MovesCount % 2 == 0)
      {
        return CrossesId;
      }
      else
      {
        return NoughtsId;
      }
    }

    internal string GetOpponentId(string currId)
    {
      if (currId == CrossesId)
      {
        return NoughtsId;
      }
      else
      {
        return CrossesId;
      }
    }

    internal bool FinishGame(string looserId)
    {
      if (!GameOver)
      {
        if (looserId == CrossesId)
        {
          Winner = "noughts";
        }
        else
        {
          Winner = "crosses";
        }

        GameOver = true;
        GameOverConditions = new Conditions { winner = Winner, direction = "forfeit", i = -1, j = -1 };

        return true;
      }

      return false;
    }

    #region private fields/props
    private enum CellTypes : byte { Clear = 0, Cross, Nought }

    private const int MinXDim = 3;
    private const int MinYDim = 3;
    private const int MinWinSize = 2;
    private const int MaxXDim = 100;
    private const int MaxYDim = 100;
    private const int MaxWinSize = 30;


    private CellTypes[] Cells;

    private string Winner = "";
    private int MovesCount { get; set; } = 0;
    #endregion

    #region private methods
    private Game(string crossesId, string noughtsId, Dimensions dimensions)
    {
      CrossesId = crossesId;
      NoughtsId = noughtsId;

      GameDimensions = dimensions;
      Cells = new CellTypes[GameDimensions.xDim * GameDimensions.yDim];
    }

    private string CalculateWinner()
    {
      if (MovesCount % 2 == 0)
      {
        return "noughts";
      }
      else
      {
        return "crosses";
      }
    }

    // ckeck for chain=|winSize cells of same type in a row| in 4 possible directions
    // for every valid starting point on the field
    private void CheckGame()
    {
      // check rows
      for (int i = 0; i < GameDimensions.yDim; i++)
      {
        for (int j = 0; j < GameDimensions.xDim - GameDimensions.winSize + 1; j++)
        {

          CellTypes start = Cells[i * GameDimensions.xDim + j];
          if (start != CellTypes.Clear)
          {

            bool foundChain = true;
            for (int k = 1; k < GameDimensions.winSize; k++)
            {
              if (Cells[i * GameDimensions.xDim + (j + k)] != start)
                foundChain = false;
            }

            if (foundChain)
            {
              Winner = CalculateWinner();
              GameOver = true;
              GameOverConditions = new Conditions { winner = Winner, direction = "right", i = i, j = j };
              return;
            }
          }
        }
      }

      // check columns
      for (int i = 0; i < GameDimensions.yDim - GameDimensions.winSize + 1; i++)
      {
        for (int j = 0; j < GameDimensions.xDim; j++)
        {

          CellTypes start = Cells[i * GameDimensions.xDim + j];
          if (start != CellTypes.Clear)
          {

            bool foundChain = true;
            for (int k = 1; k < GameDimensions.winSize; k++)
            {
              if (Cells[(i + k) * GameDimensions.xDim + j] != start)
                foundChain = false;
            }

            if (foundChain)
            {
              Winner = CalculateWinner();
              GameOver = true;
              GameOverConditions = new Conditions { winner = Winner, direction = "down", i = i, j = j };
              return;
            }
          }
        }
      }

      // check dioganal right+down
      for (int i = 0; i < GameDimensions.yDim - GameDimensions.winSize + 1; i++)
      {
        for (int j = 0; j < GameDimensions.xDim - GameDimensions.winSize + 1; j++)
        {

          CellTypes start = Cells[i * GameDimensions.xDim + j];
          if (start != CellTypes.Clear)
          {

            bool foundChain = true;
            for (int k = 1; k < GameDimensions.winSize; k++)
            {
              if (Cells[(i + k) * GameDimensions.xDim + (j + k)] != start)
                foundChain = false;
            }

            if (foundChain)
            {
              Winner = CalculateWinner();
              GameOver = true;
              GameOverConditions = new Conditions { winner = Winner, direction = "right+down", i = i, j = j };
              return;
            }
          }
        }
      }

      // check dioganal left+down
      for (int i = 0; i < GameDimensions.yDim - GameDimensions.winSize + 1; i++)
      {
        for (int j = GameDimensions.winSize - 1; j < GameDimensions.xDim; j++)
        {

          CellTypes start = Cells[i * GameDimensions.xDim + j];
          if (start != CellTypes.Clear)
          {

            bool foundChain = true;
            for (int k = 1; k < GameDimensions.winSize; k++)
            {
              if (Cells[(i + k) * GameDimensions.xDim + (j - k)] != start)
                foundChain = false;
            }

            if (foundChain)
            {
              Winner = CalculateWinner();
              GameOver = true;
              GameOverConditions = new Conditions { winner = Winner, direction = "left+down", i = i, j = j };
              return;
            }
          }
        }
      }

      if (MovesCount == GameDimensions.xDim * GameDimensions.yDim)
      {
        Winner = "draw";
        GameOver = true;
        GameOverConditions = new Conditions { winner = Winner, direction = "draw", i = -1, j = -1 };
        return;
      }
    }
    #endregion
  }
}
