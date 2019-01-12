using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

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
    private int MovesCount { get; set; } = 0;
    private bool GameOver { get; set; } = false;

    private string Winner = "";

    internal Dimensions GameDimensions { get; set; }
    private Conditions GameOverConditions { get; set; }


    internal string CrossesId { get; set; }
    internal string NoughtsId { get; set; }

    internal void MakeMove()
    {
      MovesCount++;

      if (MovesCount > 3)
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

  public class Player
  {
    internal Dictionary<string, Dimensions> InvitedPlayersIds { get; set; } = new Dictionary<string, Dimensions>();
    internal Game Game { get; set; }
  }

  public class GameHub : Hub
  {
    public static object _lock = new object();
    public static Dictionary<string, Player> AvaliablePlayers = new Dictionary<string, Player>();
    private static Dictionary<string, Player> PlayingPlayers = new Dictionary<string, Player>();

    public async Task SendMove(int index)
    {
      // check senders id if it's really his turn

      var game = PlayingPlayers[Context.ConnectionId].Game;
      game.MakeMove();


      if (game.IsGameOver())
      {
        await Clients.Client(Context.ConnectionId).SendAsync("GameEnded", game.GetGameOverConditions());
        await Clients.Client(game.GetNextMovePlayerId()).SendAsync("GameEnded", game.GetGameOverConditions());
      }
      else
      {
        await Clients.Client(game.GetNextMovePlayerId()).SendAsync("MoveRecieved", index);
      }
    }

    public async Task SendInvite(string inviteeId, Dimensions dimensions)
    {
      string inviterId = Context.ConnectionId;

      if (inviteeId != inviterId)
      {
        bool added = false;
        lock (_lock)
        {
          if (AvaliablePlayers.ContainsKey(inviterId) && !AvaliablePlayers[inviterId].InvitedPlayersIds.ContainsKey(inviteeId))
          {
            AvaliablePlayers[inviterId].InvitedPlayersIds.Add(inviteeId, dimensions);
            added = true;
          }
        }

        if (added)
        {
          await Clients.Client(inviteeId).SendAsync("InviteRequested", inviterId, dimensions);
        }
      }
    }

    public async Task SendDecline(string inviterId)
    {
      string inviteeId = Context.ConnectionId;

      bool removed = false;
      lock (_lock)
      {
        if (AvaliablePlayers.ContainsKey(inviterId))
        {
          removed = AvaliablePlayers[inviterId].InvitedPlayersIds.Remove(inviteeId);
        }
      }

      if (removed)
      {
        await Clients.Client(inviteeId).SendAsync("InviteRemoved", inviterId);
      }
    }

    public async Task SendAccept(string inviterId)
    {
      string inviteeId = Context.ConnectionId;

      Game game = null;

      lock (_lock)
      {
        if (AvaliablePlayers.ContainsKey(inviterId) && AvaliablePlayers.ContainsKey(inviteeId) &&
            AvaliablePlayers[inviterId].InvitedPlayersIds.ContainsKey(inviteeId))
        {
          var dims = AvaliablePlayers[inviterId].InvitedPlayersIds[inviteeId];
          game = new Game { CrossesId = inviterId, NoughtsId = inviteeId, GameDimensions = dims };
          AvaliablePlayers[inviterId].Game = game;
          AvaliablePlayers[inviteeId].Game = game;

          AvaliablePlayers[inviterId].InvitedPlayersIds.Clear();
          AvaliablePlayers[inviteeId].InvitedPlayersIds.Clear();

          PlayingPlayers.Add(inviterId, AvaliablePlayers[inviterId]);
          PlayingPlayers.Add(inviteeId, AvaliablePlayers[inviteeId]);

          AvaliablePlayers.Remove(inviterId);
          AvaliablePlayers.Remove(inviteeId);
        }
      }

      if (game != null)
      {
        await Groups.RemoveFromGroupAsync(inviterId, "AvaliablePlayers");
        await Groups.RemoveFromGroupAsync(inviteeId, "AvaliablePlayers");

        await Clients.Group("AvaliablePlayers").SendAsync("InviteRemoved", inviterId);
        await Clients.Group("AvaliablePlayers").SendAsync("InviteRemoved", inviteeId);
        
        await Clients.Group("AvaliablePlayers").SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());


        await Clients.Clients(inviterId, inviteeId).SendAsync("GameCreated", inviterId, game.GameDimensions);
      }
    }

    public async Task ResumeSearching()
    {
      var currId = Context.ConnectionId;

      Game game = null;

      lock (_lock)
      {
        if (PlayingPlayers.ContainsKey(currId))
        {
          game = PlayingPlayers[currId].Game;
          PlayingPlayers.Remove(currId);
        }

        AvaliablePlayers.Add(currId, new Player());
      }

      if (game != null)
      {
        bool forcedGameOver = game.FinishGame(currId);
        if (forcedGameOver)
        {
          var conditions = game.GetGameOverConditions();
          if (conditions.winner == "crosses")
            await Clients.Client(game.CrossesId).SendAsync("GameEnded", conditions);
          else
            await Clients.Client(game.NoughtsId).SendAsync("GameEnded", conditions);
        }
      }

      await Groups.AddToGroupAsync(currId, "AvaliablePlayers");
      await Clients.Group("AvaliablePlayers").SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());
    }

    public override async Task OnConnectedAsync()
    {
      lock(_lock)
      {
        AvaliablePlayers.Add(Context.ConnectionId, new Player());
      }

      await Groups.AddToGroupAsync(Context.ConnectionId, "AvaliablePlayers");


      await Clients.Caller.SendAsync("OnConnected", Context.ConnectionId);
      await Clients.Group("AvaliablePlayers").SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());


      await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
      var currId = Context.ConnectionId;

      Game game = null;

      lock (_lock)
      {
        if (PlayingPlayers.ContainsKey(currId))
        {
          game = PlayingPlayers[currId].Game;
          PlayingPlayers.Remove(currId);
        }

        AvaliablePlayers.Remove(currId);
      }

      if (game != null)
      {
        bool forcedGameOver = game.FinishGame(currId);
        if (forcedGameOver)
        {
          var conditions = game.GetGameOverConditions();
          if (conditions.winner == "crosses")
            await Clients.Client(game.CrossesId).SendAsync("GameEnded", conditions);
          else
            await Clients.Client(game.NoughtsId).SendAsync("GameEnded", conditions);
        }
      }


      await Groups.RemoveFromGroupAsync(currId, "AvaliablePlayers");

      await Clients.Group("AvaliablePlayers").SendAsync("InviteRemoved", currId);
      await Clients.Group("AvaliablePlayers").SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());


      await base.OnDisconnectedAsync(exception);
    }
  }
}
