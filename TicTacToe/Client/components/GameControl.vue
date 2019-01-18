<template>
<div>
  <h5 class="margin-bottom" id="game-status"><span v-html="this.gameStatus"></span></h5>

  <div v-if="!opponentExited && !replayOffered" style="height: 3em;">
    <button class="game-btn margin-bottom" @click="$emit('offer-replay-click')">Offer Replay</button>
    <p v-show="replayDeclined" class="margin-bottom">Opponent declined your replay request</p>
  </div>
  <div v-else-if="!opponentExited">
    <p>Opponent offered to replay</p>
    <button class="game-btn margin-bottom" @click="$emit('accept-replay-click')">Accept</button>
    <button class="game-btn" @click="$emit('decline-replay-click')">Decline</button>
  </div>

  <button id="exit-btn" class="game-btn" @click="$emit('exit-click')">Exit Game</button>
</div>
</template>

<script>
export default {
  props: {
    opponentExited: {
      type: Boolean,
      default: false
    },

    replayOffered: {
      type: Boolean,
      default: false
    },

    replayDeclined: {
      type: Boolean,
      default: false
    }
  },

  computed: {
    gameState() {
      return this.$store.state.gameEntity
    },

    gameStatus() {
      let result = ''
      
      if (this.gameState.gameOver) {
        switch(this.gameState.winner) {
          case 'crosses':
            if (this.gameState.mySide === 'crosses')
              result = 'You won!'
            else
              result = 'You lost!'
            break
          case 'noughts':
            if (this.gameState.mySide === 'noughts')
              result = 'You won!'
            else
              result = 'You lost!'
            break
          case 'draw':
            result = 'Draw!'
            break      
        }

        if (this.gameState.winByForfeit)
          result = 'Opponent left.<br/>' + result
      }
      else
      {
        if (this.gameState.isMyTurn)
          result = 'Your turn.'
        else
          result = "Opponent's turn."
      }

      result = '<i>Your side: ' + this.gameState.mySide + '</i>.<br/><br/>' + result

      return result
    }
  }
}
</script>

<style>
  .game-btn {
    background: darkolivegreen;
    outline: none;
    align-self: center;
  }

  .game-btn:active {
    background: green;
  }

  .margin-bottom {
    margin: 0 0 5% 0;
  }

  @media (orientation: landscape) {
    #game-status {
      padding: 0 0 0 10%;
    }

    #exit-btn {
      margin: 0 0 15% 0;
    }

    .margin-bottom {
      margin: 0 0 15% 0;
    }
  }
</style>
