<template>
  <div id="game-searcher">
    <div>
      <p>Connected Players Ids:</p>
      <div class="buttons-div">
        <button class="small-btn" @click="inviteButtonHandler">Invite</button>
      </div>
      <ul>
        <li v-for="player in avaliablePlayers" :key="player.key" @click="playerSelectedHandler($event, player.key)">
          {{player.key}}
        </li>
      </ul>
    </div>
    <div>
      <p>Invites Oponents Ids:</p>
      <div class="buttons-div">
        <button class="small-btn" @click="acceptButtonHandler">Accept</button>
        <button class="small-btn" @click="declineButtonHandler">Decline</button>
      </div>
      <ul>
        <li v-for="invite in invitesOpenentsIds" :key="invite.id" @click="inviteSelectedHandler($event, invite.id)">
          {{invite.id}} <br/> x:{{invite.dimensions.xDim}} y:{{invite.dimensions.yDim}} w:{{invite.dimensions.winSize}}
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    hubConnection: {
      type: Object,
      default: null
    },

    myPreferredDimensions: {
      type: Object,
      default: null
    }
  },

  data() {
    return {
      myId: '',

      avaliablePlayers: [],
      invitesOpenentsIds: [],

      prevSelectedPlayerTarget: undefined,
      prevSelectedInviteTarget: undefined,
      selectedPlayerId: '',
      selectedInviteOponentId: ''
    }
  },

  created() {
    this.hubConnection.on('MyIdAndAvaliablePlayers', (id, players)=> {
      this.myId = id
      this.updateAvaliablePlayers(players)
    })

    this.hubConnection.on('AvaliablePlayersUpdtated', players => {
      this.updateAvaliablePlayers(players)
    })

    this.hubConnection.on('InviteRequested', (id, dimensions) => {
      this.invitesOpenentsIds.push({id: id, dimensions: dimensions})
    })

    this.hubConnection.on('InviteRemoved', (id) => {
      this.removeInvite(id)
    })

    this.hubConnection.on('GameCreated', (id, dimensions) => {
      this.$emit('game-created', {isMyTurn: id === this.myId, dimensions: dimensions})
    })
  },

  methods: {
    playerSelectedHandler(event, id) {
      if (typeof(this.prevSelectedPlayerTarget) !== 'undefined')
        this.prevSelectedPlayerTarget.className = ''

      this.prevSelectedPlayerTarget = event.target

      event.target.className = 'selected'
      this.selectedPlayerId = id
    },

    inviteSelectedHandler(event, id) {
      if (typeof(this.prevSelectedInviteTarget) !== 'undefined')
        this.prevSelectedInviteTarget.className = ''

      this.prevSelectedInviteTarget = event.target

      event.target.className = 'selected'
      this.selectedInviteOponentId = id
    },


    inviteButtonHandler() {
      if (this.avaliablePlayers.find(player => player.key === this.selectedPlayerId)) {
        this.hubConnection.invoke('SendInvite', this.selectedPlayerId, this.myPreferredDimensions)
      }
    },

    acceptButtonHandler() {
      if (this.invitesOpenentsIds.find(invite => invite.id === this.selectedInviteOponentId)) {
        this.hubConnection.invoke('SendAccept', this.selectedInviteOponentId)
      }
    },

    declineButtonHandler() {
      if (this.invitesOpenentsIds.find(invite => invite.id === this.selectedInviteOponentId)) {
        this.hubConnection.invoke('SendDecline', this.selectedInviteOponentId)
      }
    },


    updateAvaliablePlayers(players) {
      this.avaliablePlayers = players.filter(player=>player.key !== this.myId)

      if (!this.avaliablePlayers.find(player=> player.key === this.selectedPlayerId)) {
        this.selectedPlayerId = ''
      }
    },

    removeInvite(id) {
      let index = this.invitesOpenentsIds.findIndex(invite => invite.id === id)
      if (index !== -1) {
        this.invitesOpenentsIds.splice(index,1)

        if (id == this.selectedInviteOponentId) {
          if (typeof(this.prevSelectedInviteTarget) !== 'undefined')
            this.prevSelectedInviteTarget.className = ''

          this.selectedInviteOponentId = ''
        }
      }
    }
  }
}
</script>

<style>
  #game-searcher {
    background: burlywood;
    width: 100%;
    height: 70%;
    position: absolute;

    display: flex;
    flex-direction: row;
    justify-content: space-around;
  }

  .buttons-div {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
  }

  .small-btn {
    width: 45%;
    background-color: darkolivegreen;
  }

  .selected {
    background-color: green;
  }

  button {
    outline: none;
  }

  button:active {
    background: green;
  }


  @media (orientation: landscape) {
    #game-searcher {
      width: 70%;
      height: 100%;
    }
  }
</style>