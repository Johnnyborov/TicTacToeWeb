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
    public string direction;
    public int i;
    public int j;
  }

  public class Game
  {
    private int MovesCount { get; set; } = 0;
    private bool GameOver { get; set; } = false;

    internal Dimensions GameDimensions { get; set; }
    private Conditions GameOverConditions { get; set; }


    internal string CrossesId { get; set; }
    internal string NoughtsId { get; set; }

    internal void MakeMove()
    {
      MovesCount++;

      if (MovesCount > 3) GameOver = true;

      GameOverConditions = new Conditions{ direction = "right", i = 0, j = 0};
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
    public static Dictionary<string, Player> PlayingPlayers = new Dictionary<string, Player>();


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
        if (AvaliablePlayers.ContainsKey(inviterId) && AvaliablePlayers[inviterId].InvitedPlayersIds.ContainsKey(inviteeId))
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
      lock (_lock)
      {
        AvaliablePlayers.Remove(Context.ConnectionId);
      }

      await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AvaliablePlayers");


      await Clients.Group("AvaliablePlayers").SendAsync("InviteRemoved", Context.ConnectionId);
      await Clients.Group("AvaliablePlayers").SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());


      await base.OnDisconnectedAsync(exception);
    }
  }
}
