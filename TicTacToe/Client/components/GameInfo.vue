<template>
<div id="game-info">
  <h2>{{this.message}}</h2>
  <h3>{{this.gameStatus}}</h3>
  <button @click="$emit('reset-click')">Reset</button>
</div>
</template>

<script>
export default {
  data() {
    return {
      message: 'Game Info:',
    }
  },

  computed: {
    gameStatus() {
      let result = ''

      if (this.$store.state.gameEntity.gameOver) {
        switch(this.$store.state.gameEntity.winner) {
          case 'crosses':
            result = 'crosses won'
            break
          case 'noughts':
            result = 'noughts won'
            break
          case 'draw':
            result = 'draw'
            break      
        }
      } else {
        result = 'game is in progress'
      }

      return result
    }
  }
}
</script>

<style>
  #game-info {
    font-family: Verdana;
    color: forestgreen;
    background: gold;
    width: 34%;
    height: 100%;
    position: absolute;
    left: 66%;
  }
</style>
