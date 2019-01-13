<template>
  <div id="game-manager">
    <game-searcher v-if="lookingForGame" :hubConnection="hubConnection" @game-created="createGame"
      :myPreferredDimensions="myPreferredDimensions"></game-searcher>
    <game-field v-else @cell-clicked="cellClickedHandler" :cellIcons="resources.cellIcons" :isMyTurn="isMyTurn"></game-field>

    <game-info @exit-click="exitGameHandler" @sizes-click="changeSizesHandler" :isMyTurn="isMyTurn" :mySide="mySide"
      :lookingForGame="lookingForGame" :myPreferredDimensions="myPreferredDimensions"></game-info>
  </div>
</template>

<script>
import GameSearcher from './GameSearcher.vue'
import GameField from './GameField.vue'
import GameInfo from './GameInfo.vue'

import Resources from '../resources/resources.js'
import {HubConnectionBuilder, LogLevel} from '@aspnet/signalr'

const debug = process.env.NODE_ENV !== 'production'

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

      myPreferredDimensions: {
        xDim: this.$store.state.gameEntity.xDim,
        yDim: this.$store.state.gameEntity.yDim,
        winSize: this.$store.state.gameEntity.winSize,
      },
      isMyTurn: false,
      mySide: '',

      resources: {
        cellIcons: null
      }
    }
  },

  created() {
    this.resources.cellIcons = Resources.getCellIcons()

    let url = '/game'
    if (debug)
      url = 'http://localhost:45353/game'

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(url)
      .configureLogging(LogLevel.Information)
      .build()

    this.hubConnection.serverTimeoutInMilliseconds = 12 * 1000

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

      this.hubConnection.invoke('ResumeSearching')
    },

    changeSizesHandler(dimensions) {
      this.myPreferredDimensions = dimensions
    },

    createGame({isMyTurn, dimensions}) {
      this.lookingForGame = false
      
      if (isMyTurn)
        this.mySide = 'crosses'
      else
        this.mySide = 'noughts'

      this.isMyTurn = isMyTurn

      this.$store.commit('gameEntity/newGame', dimensions)
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