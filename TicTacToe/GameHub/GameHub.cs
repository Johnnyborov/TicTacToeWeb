using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.GameHub
{
  public class Game
  {
    internal int MovesCount { get; set; }
    internal string CrossesId { get; set; }
    internal string NoughtsId { get; set; }
  }

  public class Player
  {
    internal List<string> InvitedPlayersIds { get; set; } = new List<string>();
    internal Game Game { get; set; }
  }

  public class GameHub : Hub
  {
    public static object _lock = new object();
    public static Dictionary<string, Player> AvaliablePlayers = new Dictionary<string, Player>();
    public static Dictionary<string, Player> PlayingPlayers = new Dictionary<string, Player>();


    public async Task SendMove(int index)
    {
      var game = PlayingPlayers[Context.ConnectionId].Game;
      game.MovesCount++;

      string oponentId = "";
      if (game.MovesCount % 2 == 0)
      {
        oponentId = game.CrossesId;
      }
      else
      {
        oponentId = game.NoughtsId;
      }

      await Clients.Client(oponentId).SendAsync("MoveRecieved", index);
    }

    public async Task SendInvite(string inviteeId)
    {
      string inviterId = Context.ConnectionId;

      if (inviteeId != inviterId)
      {
        bool added = false;
        lock (_lock)
        {
          if (AvaliablePlayers.ContainsKey(inviterId) && !AvaliablePlayers[inviterId].InvitedPlayersIds.Contains(inviteeId))
          {
            AvaliablePlayers[inviterId].InvitedPlayersIds.Add(inviteeId);
            added = true;
          }
        }

        if (added)
        {
          await Clients.Client(inviteeId).SendAsync("InviteRequested", inviterId);

          await Clients.All.SendAsync("NotificationRecieved", $"{inviteeId} was invited by {inviterId}");
        }
      }
    }

    public async Task SendAccept(string inviterId)
    {
      string inviteeId = Context.ConnectionId;

      bool removed = false;
      lock (_lock)
      {
        if (AvaliablePlayers.ContainsKey(inviterId) && AvaliablePlayers[inviterId].InvitedPlayersIds.Contains(inviteeId))
        {
          AvaliablePlayers[inviterId].InvitedPlayersIds.Clear();
          AvaliablePlayers[inviteeId].InvitedPlayersIds.Clear();

          var game = new Game { CrossesId = inviterId, NoughtsId = inviteeId };
          AvaliablePlayers[inviterId].Game = game;
          AvaliablePlayers[inviteeId].Game = game;

          PlayingPlayers.Add(inviterId, AvaliablePlayers[inviterId]);
          PlayingPlayers.Add(inviteeId, AvaliablePlayers[inviteeId]);

          AvaliablePlayers.Remove(inviterId);
          AvaliablePlayers.Remove(inviteeId);

          removed = true;
        }
      }

      if (removed)
      {
        await Groups.RemoveFromGroupAsync(inviterId, "AvaliablePlayers");
        await Groups.RemoveFromGroupAsync(inviteeId, "AvaliablePlayers");

        await Clients.Group("AvaliablePlayers").SendAsync("InviteRemoved", inviterId);
        await Clients.Group("AvaliablePlayers").SendAsync("InviteRemoved", inviteeId);
        
        await Clients.Group("AvaliablePlayers").SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());


        await Clients.Clients(inviterId, inviteeId).SendAsync("GameCreated", inviterId);


        await Clients.All.SendAsync("NotificationRecieved", $"{inviterId} 's invite was accepted by {inviteeId}");
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

      await Clients.All.SendAsync("NotificationRecieved", $"{Context.ConnectionId} connected");

      await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
      lock (_lock)
      {
        AvaliablePlayers.Remove(Context.ConnectionId);
      }

      await Groups.AddToGroupAsync(Context.ConnectionId, "AvaliablePlayers");


      await Clients.Group("AvaliablePlayers").SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());

      await Clients.All.SendAsync("NotificationRecieved", $"{Context.ConnectionId} disconnected");

      await base.OnDisconnectedAsync(exception);
    }
  }
}
