<template>
  <div id="game-search">
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
      <p>Invites Opponents Ids:</p>
      <div class="buttons-div">
        <button class="small-btn" @click="acceptButtonHandler">Accept</button>
        <button class="small-btn" @click="declineButtonHandler">Decline</button>
      </div>
      <ul>
        <li v-for="invite in invitesOpponentsIds" :key="invite.id" @click="inviteSelectedHandler($event, invite.id)">
          {{invite.id}} <br/> x:{{invite.dimensions.xDim}} y:{{invite.dimensions.yDim}} w:{{invite.dimensions.winSize}}
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      avaliablePlayers: [],
      invitesOpponentsIds: [],

      prevSelectedPlayerTarget: undefined,
      prevSelectedInviteTarget: undefined,
      selectedPlayerId: '',
      selectedInviteOpponentId: ''
    }
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
      this.selectedInviteOpponentId = id
    },


    inviteButtonHandler() {
      if (this.avaliablePlayers.find(player => player.key === this.selectedPlayerId)) {
        this.$emit('invite-click', this.selectedPlayerId)
      }
    },

    acceptButtonHandler() {
      if (this.invitesOpponentsIds.find(invite => invite.id === this.selectedInviteOpponentId)) {
        this.$emit('accept-click', this.selectedInviteOpponentId)
      }
    },

    declineButtonHandler() {
      if (this.invitesOpponentsIds.find(invite => invite.id === this.selectedInviteOpponentId)) {
        this.$emit('decline-click', this.selectedInviteOpponentId)
      }
    },


    updateAvaliablePlayers(players, myId) {
      this.avaliablePlayers = players.filter(player=>player.key !== myId)

      if (!this.avaliablePlayers.find(player=> player.key === this.selectedPlayerId)) {
        this.selectedPlayerId = ''
      }
    },

    removeInvite(id) {
      let index = this.invitesOpponentsIds.findIndex(invite => invite.id === id)
      if (index !== -1) {
        this.invitesOpponentsIds.splice(index,1)

        if (id == this.selectedInviteOpponentId) {
          if (typeof(this.prevSelectedInviteTarget) !== 'undefined')
            this.prevSelectedInviteTarget.className = ''

          this.selectedInviteOpponentId = ''
        }
      }
    }
  }
}
</script>

<style>
  #game-search {
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
    #game-search {
      width: 70%;
      height: 100%;
    }
  }
</style>