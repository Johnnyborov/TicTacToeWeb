using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.GameHub
{
  public class Player
  {
    public List<string> InvitedPlayersIds { get; set; } = new List<string>();
  }

  public class GameHub : Hub
  {
    public static Dictionary<string,Player> AvaliablePlayers = new Dictionary<string, Player>();
    public static object _lock = new object();

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
          // maybe reuse these player variables later in game description
          //AvaliablePlayers[inviterId].InvitedPlayersIds.Clear();
          //AvaliablePlayers[inviteeId].InvitedPlayersIds.Clear();

          AvaliablePlayers.Remove(inviterId);
          AvaliablePlayers.Remove(inviteeId);

          removed = true;
        }
      }

      if (removed)
      {
        // createGame(inviterId,inviteeId);

        await Clients.Clients(inviterId, inviteeId).SendAsync("GameCreated");

        await Clients.All.SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());

        await Clients.All.SendAsync("NotificationRecieved", $"{inviterId} 's invite was accepted by {inviteeId}");
      }
    }

    public override async Task OnConnectedAsync()
    {
      lock(_lock)
      {
        AvaliablePlayers.Add(Context.ConnectionId, new Player());
      }

      await Clients.Caller.SendAsync("OnConnected", Context.ConnectionId);

      await Clients.All.SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());
      await Clients.All.SendAsync("NotificationRecieved", $"{Context.ConnectionId} connected");

      await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
      lock (_lock)
      {
        AvaliablePlayers.Remove(Context.ConnectionId);
      }

      await Clients.All.SendAsync("AvaliablePlayersUpdtated", AvaliablePlayers.ToArray());
      await Clients.All.SendAsync("NotificationRecieved", $"{Context.ConnectionId} disconnected");

      await base.OnDisconnectedAsync(exception);
    }
  }
}
