﻿using System;
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
    private enum CellTypes: byte { Clear = 0, Cross, Nought }

    private CellTypes[] Cells;

    private int MovesCount { get; set; } = 0;
    private bool GameOver { get; set; } = false;

    private string Winner = "";

    internal Dimensions GameDimensions { get; set; }
    private Conditions GameOverConditions { get; set; }


    internal string CrossesId { get; set; }
    internal string NoughtsId { get; set; }

    public Game(string inviterId, string inviteeId, Dimensions dims)
    {
      CrossesId = inviterId;
      NoughtsId = inviteeId;

      GameDimensions = dims;
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
            }
          }
        }
      }

      if (MovesCount == GameDimensions.xDim * GameDimensions.yDim)
      {
        Winner = "draw";
        GameOver = true;
        GameOverConditions = new Conditions { winner = Winner, direction = "draw", i = -1, j = -1 };
      }
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

    internal bool IsGameOver()
    {
      return GameOver;
    }

    internal Conditions GetGameOverConditions()
    {
      return GameOverConditions;
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
  }
}
