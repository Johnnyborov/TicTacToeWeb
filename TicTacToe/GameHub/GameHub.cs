using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.GameHub
{
  public class Player
  {
    internal Dictionary<string, Dimensions> InvitedPlayersIds { get; set; } = new Dictionary<string, Dimensions>();
    internal List<string> InvitedByIds { get; set; } = new List<string>();
    internal Game Game { get; set; }
  }

  public class GameHub : Hub
  {
    public static object _lock = new object();
    public static Dictionary<string, Player> AvaliablePlayers = new Dictionary<string, Player>();
    private static Dictionary<string, Player> PlayingPlayers = new Dictionary<string, Player>();

    public async Task SendMove(int index)
    {
      var game = PlayingPlayers[Context.ConnectionId].Game;

      // check senders id if it's really his turn
      string currentMovePlayerId = game.GetNextMovePlayerId();
      if (Context.ConnectionId != currentMovePlayerId)
        return;

      bool wasValidMove = game.TryMakeMove(index);

      if (!wasValidMove)
        return;

      await Clients.Client(game.GetNextMovePlayerId()).SendAsync("MoveRecieved", index);

      if (game.IsGameOver())
      {
        await Clients.Client(Context.ConnectionId).SendAsync("GameEnded", game.GetGameOverConditions());
        await Clients.Client(game.GetNextMovePlayerId()).SendAsync("GameEnded", game.GetGameOverConditions());
      }
    }

    public async Task SendInvite(string inviteeId, Dimensions dimensions)
    {
      string inviterId = Context.ConnectionId;

      if (inviteeId != inviterId && Game.DimensionsAreValid(dimensions))
      {
        bool added = false;
        lock (_lock)
        {
          if (AvaliablePlayers.ContainsKey(inviterId) && AvaliablePlayers.ContainsKey(inviteeId) &&
              !AvaliablePlayers[inviterId].InvitedPlayersIds.ContainsKey(inviteeId))
          {
            AvaliablePlayers[inviterId].InvitedPlayersIds.Add(inviteeId, dimensions);
            AvaliablePlayers[inviteeId].InvitedByIds.Add(inviterId);
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
        if (AvaliablePlayers.ContainsKey(inviterId) && AvaliablePlayers.ContainsKey(inviteeId))
        {
          removed = AvaliablePlayers[inviterId].InvitedPlayersIds.Remove(inviteeId);
          AvaliablePlayers[inviteeId].InvitedByIds.Remove(inviterId);
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

      Random rand = new Random();
      bool inviterFirstMove = rand.Next(0, 2) == 0;

      lock (_lock)
      {
        if (AvaliablePlayers.ContainsKey(inviterId) && AvaliablePlayers.ContainsKey(inviteeId) &&
            AvaliablePlayers[inviterId].InvitedPlayersIds.ContainsKey(inviteeId))
        {
          var dims = AvaliablePlayers[inviterId].InvitedPlayersIds[inviteeId];
          if (inviterFirstMove) // crosses = inviter
          {
            game = new Game(inviterId, inviteeId, dims);
          }
          else // crosses = invitee
          {
            game = new Game(inviteeId, inviterId, dims);
          }

          AvaliablePlayers[inviterId].Game = game;
          AvaliablePlayers[inviteeId].Game = game;


          // remove inviter from other avaliable players Invited and InvitedBy lists
          foreach (var id in AvaliablePlayers[inviterId].InvitedByIds)
          {
            if (id != inviterId && id != inviteeId && AvaliablePlayers.ContainsKey(id))
            {
              AvaliablePlayers[id].InvitedPlayersIds.Remove(inviterId);
              AvaliablePlayers[id].InvitedByIds.Remove(inviterId);
            }
          }
          // remove invitee from other avaliable players Invited and InvitedBy lists
          foreach (var id in AvaliablePlayers[inviteeId].InvitedByIds)
          {
            if (id != inviterId && id != inviteeId && AvaliablePlayers.ContainsKey(id))
            {
              AvaliablePlayers[id].InvitedPlayersIds.Remove(inviteeId);
              AvaliablePlayers[id].InvitedByIds.Remove(inviteeId);
            }
          }


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

        await Clients.GroupExcept("AvaliablePlayers", inviterId, inviteeId).SendAsync("InviteRemoved", inviterId);
        await Clients.GroupExcept("AvaliablePlayers", inviterId, inviteeId).SendAsync("InviteRemoved", inviteeId);
        
        await Clients.GroupExcept("AvaliablePlayers", inviterId, inviteeId).SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());

        if (inviterFirstMove)
        {
          await Clients.Clients(inviterId, inviteeId).SendAsync("GameCreated", inviterId, game.GameDimensions);
        }
        else
        {
          await Clients.Clients(inviterId, inviteeId).SendAsync("GameCreated", inviteeId, game.GameDimensions);
        }
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

      await Clients.Caller.SendAsync("MyIdAndAvaliablePlayers", currId, AvaliablePlayers.ToArray());
      await Clients.GroupExcept("AvaliablePlayers", Context.ConnectionId).SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());
    }

    public override async Task OnConnectedAsync()
    {
      lock(_lock)
      {
        AvaliablePlayers.Add(Context.ConnectionId, new Player());
      }

      await Groups.AddToGroupAsync(Context.ConnectionId, "AvaliablePlayers");


      await Clients.Caller.SendAsync("MyIdAndAvaliablePlayers", Context.ConnectionId, AvaliablePlayers.ToArray());
      await Clients.GroupExcept("AvaliablePlayers", Context.ConnectionId).SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());


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

      await Clients.GroupExcept("AvaliablePlayers", currId).SendAsync("InviteRemoved", currId);
      await Clients.GroupExcept("AvaliablePlayers", currId).SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());


      await base.OnDisconnectedAsync(exception);
    }
  }
}
