<template>
  <div id="game-manager">
    <game-searcher v-if="lookingForGame" @game-created="createGameHandler($event)" :hubConnection="hubConnection"></game-searcher>
    <template v-else>
      <game-field @cell-clicked="cellClickedHandler" :cellIcons="resources.cellIcons" :isMyTurn="isMyTurn"></game-field>
      <game-info @reset-click="resetGameHandler" @sizes-click="changeSizesHandler"></game-info>
    </template>
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

    this.hubConnection.on('MoveRecieved', index=>{
      this.$store.commit('gameEntity/makeMove', index)
      this.isMyTurn = true
    })

    this.hubConnection.on('GameEnded', conditions=>{
      store.commit('gameEntity/finishGame', conditions)
    })

    this.hubConnection.start()
  },

  mounted() {
    this.$store.commit('gameEntity/newGame')
  },

  methods: {
    cellClickedHandler(index) {
      this.isMyTurn = false

      this.$store.commit('gameEntity/makeMove', index)

      this.hubConnection.invoke('SendMove', index)
    },

    resetGameHandler() {
      this.$store.commit('gameEntity/newGame')
    },

    changeSizesHandler(dimensions) {
      this.$store.commit('gameEntity/changeSizes', dimensions)
    },

    createGameHandler(isMyTurn) {
      this.lookingForGame = false

      this.isMyTurn = isMyTurn
      this.$store.commit('gameEntity/newGame')
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