<template>
  <div id="game-hub">
    <p>{{message}}</p>
  </div>
</template>

<script>
import * as SignalR from '@aspnet/signalr'

export default {
  data() {
    return {
      hubConnection: null,

      message: 'INITIAL MESSAGE'
    }
  },

  mounted() {
    this.hubConnection = new SignalR.HubConnectionBuilder()
      .withUrl('http://localhost:45353/game')
      .configureLogging(SignalR.LogLevel.Information)
      .build()

    this.hubConnection.serverTimeoutInMilliseconds = 12 * 1000

    this.hubConnection.on('Notify', msg => {
      this.message = msg
    })

    this.hubConnection.start()
  }
}
</script>
