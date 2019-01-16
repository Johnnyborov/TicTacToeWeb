<template>
  <div id="game-manager">
    <game-search ref="game-search" v-if="lookingForGame" @invite-click="inviteClickHandler"
      @accept-click="acceptClickHandler" @decline-click="declineClickHandler"></game-search>
    <game-field v-else @cell-clicked="cellClickHandler" :cellIcons="resources.cellIcons" :isMyTurn="isMyTurn"></game-field>

    <game-info ref="game-info" @exit-click="exitClickHandler" @sizes-click="sizesClickHandler"
      @offer-replay-click="offerReplayClickHandler" @accept-replay-click="acceptReplayClickHandler" @decline-replay-click="declineReplayClickHandler"
      :isMyTurn="isMyTurn" :mySide="mySide" :lookingForGame="lookingForGame" :myPreferredDimensions="myPreferredDimensions"></game-info>
  </div>
</template>

<script>
import GameSearch from './GameSearch.vue'
import GameField from './GameField.vue'
import GameInfo from './GameInfo.vue'

import Resources from '../resources/resources.js'
import {HubConnectionBuilder, LogLevel} from '@aspnet/signalr'

const debug = process.env.NODE_ENV !== 'production'

export default {
  components: {
    'game-search': GameSearch,
    'game-field': GameField,
    'game-info': GameInfo
  },

  data() {
    return {
      lookingForGame: true,

      hubConnection: null,
      myId: '',

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

    this.hubConnection.on('GameCreated', (crossesId, noughtsId, dimensions) => {
      let isMyTurn, opponentId
      if (crossesId === this.myId) {
        isMyTurn = true
        opponentId = noughtsId
      } else {
        isMyTurn = false
        opponentId = crossesId
      }

      this.createGame(isMyTurn, dimensions, opponentId)
    })


    this.hubConnection.on('MyIdAndAvaliablePlayers', (myId, players)=> {
      this.myId = myId

      this.$refs['game-search'].updateAvaliablePlayers(players, myId)
    })

    this.hubConnection.on('AvaliablePlayersUpdtated', players => {
      this.$refs['game-search'].updateAvaliablePlayers(players, this.myId)
    })

    this.hubConnection.on('InviteRequested', (opponentId, dimensions) => {
      this.$refs['game-search'].invitesOpponentsIds.push({id: opponentId, dimensions: dimensions})
    })

    this.hubConnection.on('InviteRemoved', (opponentId) => {
      this.$refs['game-search'].removeInvite(opponentId)
    })

    
    this.hubConnection.on('OpponentExited', () => {
      this.$refs['game-info'].opponentExited = true
    })

    this.hubConnection.on('ReplayRequested', () => {
      this.$refs['game-info'].replayOffered = true
    })

    this.hubConnection.on('ReplayDeclined', () => {
      this.$refs['game-info'].replayDeclined = true
    })

    this.hubConnection.start()
  },


  methods: {
    cellClickHandler(index) {
      this.isMyTurn = false

      this.$store.commit('gameEntity/makeMove', index)

      this.hubConnection.invoke('SendMove', index)
    },

    sizesClickHandler(dimensions) {
      this.myPreferredDimensions = dimensions
    },

    exitClickHandler() {
      this.lookingForGame = true

      this.hubConnection.invoke('ResumeGameSearch')
    },

    inviteClickHandler(opponentId) {
      this.hubConnection.invoke('SendInviteRequest', opponentId, this.myPreferredDimensions)
    },

    acceptClickHandler(opponentId) {
      this.hubConnection.invoke('SendInviteAccept', opponentId)
    },

    declineClickHandler(opponentId) {
      this.hubConnection.invoke('SendInviteDecline', opponentId)
    },


    offerReplayClickHandler() {
      this.hubConnection.invoke('SendReplayRequest')
    },

    acceptReplayClickHandler() {
      this.hubConnection.invoke('SendReplayAccept')
    },

    declineReplayClickHandler() {
      this.hubConnection.invoke('SendReplayDecline')
    },


    createGame(isMyTurn, dimensions, opponentId) {
      this.lookingForGame = false
      
      if (isMyTurn)
        this.mySide = 'crosses'
      else
        this.mySide = 'noughts'

      this.isMyTurn = isMyTurn

      this.$refs['game-info'].replayOffered = false
      this.$refs['game-info'].opponentExited = false

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