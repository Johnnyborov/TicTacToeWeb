using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.GameHub
{
  public class GameHub : Hub
  {
    public override async Task OnConnectedAsync()
    {
      await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} connected");

      await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
      await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} disconnected");

      await base.OnDisconnectedAsync(exception);
    }
  }
}
