<template>
  <div id="game-searcher">
    <div>
      <p>Connected Players Ids:</p>
      <button @click="inviteButtonHandler">Invite</button>
      <ul>
        <li v-for="player in avaliablePlayers" :key="player.key" @click="playerSelectedHandler($event, player.key)">
          {{player.key}}
        </li>
      </ul>
    </div>
    <div>
      <p>Invites Oponents Ids:</p>
      <div id="invites-buttons">
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

    myId: {
      type: String,
      default: null
    },

    myPreferredDimensions: {
      type: Object,
      default: null
    }
  },

  data() {
    return {
      avaliablePlayers: [],
      invitesOpenentsIds: [],

      prevSelectedPlayerTarget: undefined,
      prevSelectedInviteTarget: undefined,
      selectedPlayerId: '',
      selectedInviteOponentId: ''
    }
  },

  created() {
    this.hubConnection.on('AvaliablePlayersUpdtated', players => {
      this.avaliablePlayers = players.filter(player=>player.key !== this.myId)

      // if connection hasn't started yet
      if (this.myId == null) setTimeout(()=>{
        this.avaliablePlayers = this.avaliablePlayers.filter(player=>player.key !== this.myId)
      }, 300)
    })

    this.hubConnection.on('InviteRequested', (id, dimensions) => {
      this.invitesOpenentsIds.push({id: id, dimensions: dimensions})
    })

    this.hubConnection.on('InviteRemoved', (id) => {
      let index = this.invitesOpenentsIds.findIndex(invite => invite.id === id)
      if (index !== -1) {
        this.invitesOpenentsIds.splice(index,1)

        if (id == this.selectedInviteOponentId) {
          if (typeof(this.prevSelectedInviteTarget) !== 'undefined')
            this.prevSelectedInviteTarget.className = ''
        }
      }
    })
  },

  methods: {
    playerSelectedHandler(event, id) {
      if (typeof(this.prevSelectedPlayerTarget) !== 'undefined')
        this.prevSelectedPlayerTarget.className = ''

      this.selectedPlayerId = id
      this.prevSelectedPlayerTarget = event.target
      event.target.className = 'selected'
    },

    inviteSelectedHandler(event, id) {
      if (typeof(this.prevSelectedInviteTarget) !== 'undefined')
        this.prevSelectedInviteTarget.className = ''

      this.selectedInviteOponentId = id
      this.prevSelectedInviteTarget = event.target
      event.target.className = 'selected'
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

  #invites-buttons {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
  }

  .small-btn {
    width: 45%;
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