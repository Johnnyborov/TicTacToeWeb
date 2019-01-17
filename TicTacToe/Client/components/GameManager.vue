<template>
  <div id="game-manager">
    <game-search v-if="lookingForGame" @invite-click="inviteClickHandler"
      @accept-click="acceptClickHandler" @decline-click="declineClickHandler"></game-search>
    <game-field v-else @cell-clicked="cellClickHandler" :cellIcons="resources.cellIcons"></game-field>

    <game-info @exit-click="exitClickHandler" @sizes-click="sizesClickHandler" @offer-replay-click="offerReplayClickHandler"
      @accept-replay-click="acceptReplayClickHandler" @decline-replay-click="declineReplayClickHandler" :lookingForGame="lookingForGame"
      :opponentExited="opponentExited" :replayOffered="replayOffered" :replayDeclined="replayDeclined"></game-info>
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
      resources: {
        cellIcons: null
      },

      hubConnection: null,
      myId: '',

      lookingForGame: true,

      opponentExited: false,
      replayOffered: false,
      replayDeclined: false
    }
  },


  created() {
    this.resources.cellIcons = Resources.getCellIcons()

    this.$store.dispatch('invites/initialize')

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

      this.$store.dispatch('invites/updateAvaliablePlayers', {players: players, myId: myId})
    })

    this.hubConnection.on('AvaliablePlayersUpdtated', players => {
      this.$store.dispatch('invites/updateAvaliablePlayers', {players: players, myId: this.myId})
    })

    this.hubConnection.on('InviteRequested', (opponentId, dimensions) => {
      this.$store.commit('invites/addInvite', {id: opponentId, dimensions: dimensions})
    })

    this.hubConnection.on('InviteRemoved', (opponentId) => {
     this.$store.dispatch('invites/removeInvite', opponentId)
    })

    
    this.hubConnection.on('OpponentExited', () => {
      this.opponentExited = true
    })

    this.hubConnection.on('ReplayRequested', () => {
      this.replayOffered = true
    })

    this.hubConnection.on('ReplayDeclined', () => {
      this.replayDeclined = true
    })

    this.hubConnection.start()
  },


  methods: {
    cellClickHandler(index) {
      this.$store.commit('gameEntity/makeMove', index)

      this.hubConnection.invoke('SendMove', index)
    },
    

    sizesClickHandler(dimensions) {
      this.$store.commit('invites/setMyPreferredDimensions', dimensions)
    },


    exitClickHandler() {
      this.lookingForGame = true

      this.$store.commit('invites/reset')

      this.hubConnection.invoke('ResumeGameSearch')
    },


    inviteClickHandler(opponentId) {
      this.hubConnection.invoke('SendInviteRequest', opponentId, this.$store.state.invites.myPreferredDimensions)
    },

    acceptClickHandler(opponentId) {
      this.hubConnection.invoke('SendInviteAccept', opponentId)
    },

    declineClickHandler(opponentId) {
      this.hubConnection.invoke('SendInviteDecline', opponentId)
    },


    offerReplayClickHandler() {
      this.replayDeclined = false

      this.hubConnection.invoke('SendReplayRequest')
    },

    acceptReplayClickHandler() {  
      this.hubConnection.invoke('SendReplayAccept')
    },

    declineReplayClickHandler() {
      this.replayOffered = false

      this.hubConnection.invoke('SendReplayDecline')
    },


    createGame(isMyTurn, dimensions) {
      this.lookingForGame = false

      this.opponentExited = false
      this.replayOffered = false
      this.replayDeclined = false

      this.$store.commit('gameEntity/newGame', {dimensions: dimensions, isMyTurn: isMyTurn})
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