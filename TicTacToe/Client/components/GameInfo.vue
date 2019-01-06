<template>
<div id="game-info">
  <h4>{{this.message}}</h4>
  <h5>Game status: <br/> {{this.gameStatus}}</h5>
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

    display: flex;
    flex-direction: column;
    justify-content: space-around;

    width: 100%;
    height: 30%;
    position: absolute;
    top: 70%;
  }

  @media (orientation: landscape) {
    #game-info {
      width: 30%;
      height: 100%;
      top: 0%;
      left: 70%;
    }
  }
</style>
