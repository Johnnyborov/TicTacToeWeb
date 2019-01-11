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
      <p>Invites Openents Ids:</p>
      <button @click="acceptButtonHandler">Accept</button>
      <ul>
        <li v-for="id in invitesOpenentsIds" :key="id" @click="inviteSelectedHandler($event, id)">
          {{id}}
        </li>
      </ul>
    </div>
    <div class="narrow">
      <p>My id: {{myId}}</p>
      <p>Notification Messages:</p>
      <ul style="list-style: none;">
        <li v-for="(msg, index) in reverseNotMsg" :key="index">
          {{reverseNotMsg.length - index}}. {{msg}}
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
    }
  },

  data() {
    return {
      myId: null,

      notificationsMessages: [],
      avaliablePlayers: [],
      invitesOpenentsIds: [],

      prevSelectedPlayerTarget: undefined,
      prevSelectedInviteTarget: undefined,
      selectedPlayerId: '',
      selectedInviteOponentId: ''
    }
  },

  computed: {
    reverseNotMsg() {
      return this.notificationsMessages.slice().reverse()
    }
  },

  created() {
    this.hubConnection.on('OnConnected', id => {
      this.myId = id
    })

    this.hubConnection.on('AvaliablePlayersUpdtated', players => {
      this.avaliablePlayers = players.filter(player=>player.key !== this.myId)
    })

    this.hubConnection.on('NotificationRecieved', msg => {
      this.notificationsMessages.push(msg)
    })

    this.hubConnection.on('InviteRequested', (id) => {
      this.invitesOpenentsIds.push(id)
    })

    this.hubConnection.on('InviteRemoved', (id) => {
      let index = this.invitesOpenentsIds.indexOf(id)
      if (index !== -1)
        this.invitesOpenentsIds.splice(index,1)
    })

    this.hubConnection.on('GameCreated', id => {
      this.$emit('game-created', id === this.myId)
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
      if (this.avaliablePlayers.find(player=>player.key === this.selectedPlayerId)) {
        this.hubConnection.invoke('SendInvite', this.selectedPlayerId)
      }
    },

    acceptButtonHandler() {
      if (this.invitesOpenentsIds.includes(this.selectedInviteOponentId)) {
        this.hubConnection.invoke('SendAccept', this.selectedInviteOponentId)
      }
    }
  }
}
</script>

<style>
  #game-searcher {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
  }

  .narrow {
    width: 25%;
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
</style>