<template>
  <div id="game-manager">
    <game-searcher v-if="lookingForGame" :hubConnection="hubConnection" :myId="myId"
      :myPreferredDimensions="myPreferredDimensions"></game-searcher>
    <game-field v-else @cell-clicked="cellClickedHandler" :cellIcons="resources.cellIcons" :isMyTurn="isMyTurn"></game-field>

    <game-info @exit-click="exitGameHandler" @sizes-click="changeSizesHandler" :isMyTurn="isMyTurn"
      :lookingForGame="lookingForGame" :myPreferredDimensions="myPreferredDimensions"></game-info>
  </div>
</template>

<script>
import GameSearcher from './GameSearcher.vue'
import GameField from './GameField.vue'
import GameInfo from './GameInfo.vue'

import Resources from '../resources/resources.js'
import * as SignalR from '@aspnet/signalr'


export default {
  components: {
    'game-searcher': GameSearcher,
    'game-field': GameField,
    'game-info': GameInfo
  },

  data() {
    return {
      lookingForGame: true,

      hubConnection: null,
      myId: null,

      myPreferredDimensions: {
        xDim: this.$store.state.gameEntity.xDim,
        yDim: this.$store.state.gameEntity.yDim,
        winSize: this.$store.state.gameEntity.winSize,
      },
      isMyTurn: false,

      resources: {
        cellIcons: null
      }
    }
  },

  created() {
    this.resources.cellIcons = Resources.getCellIcons()

    this.hubConnection = new SignalR.HubConnectionBuilder()
      .withUrl('http://localhost:45353/game')
      .configureLogging(SignalR.LogLevel.Information)
      .build()

    this.hubConnection.serverTimeoutInMilliseconds = 12 * 1000

    this.hubConnection.on('OnConnected', id => {
      this.myId = id
    })

    this.hubConnection.on('GameCreated', (id, dimensions) => {
      this.createGame(id === this.myId, dimensions)
    })

    this.hubConnection.on('MoveRecieved', index=>{
      this.$store.commit('gameEntity/makeMove', index)
      this.isMyTurn = true
    })

    this.hubConnection.on('GameEnded', conditions=>{
      this.$store.commit('gameEntity/finishGame', conditions)
    })

    this.hubConnection.start()
  },

  methods: {
    cellClickedHandler(index) {
      this.isMyTurn = false

      this.$store.commit('gameEntity/makeMove', index)

      this.hubConnection.invoke('SendMove', index)
    },

    exitGameHandler() {
      this.lookingForGame = true
    },

    changeSizesHandler(dimensions) {
      this.myPreferredDimensions = dimensions
    },

    createGame(isMyTurn, dimensions) {
      this.lookingForGame = false

      this.isMyTurn = isMyTurn
      this.$store.commit('gameEntity/changeSizes', dimensions)
    }
  }
}
</script>

<style>
  #game-manager {
    background: burlywood;
    width: 91%;
    padding-bottom: 130%;
    position: relative;
  }

  @media (orientation: landscape) {
    #game-manager {
      width: 100%;
      padding-bottom: 70%;
    }
  }
</style>