<template>
<div id="game-info">
  <h5>{{this.gameStatus}}</h5>
  <div id="game-sizes">
    <div>
      <label>x-dim:</label>
      <input/>
    </div>
    <div>
      <label>y-dim:</label>
      <input/>
    </div>
    <div>
      <label>win-size:</label>
      <input/>
    </div>
    <button @click="$emit('sizes-click')">Apply</button>
  </div>
  <button @click="$emit('reset-click')">Reset</button>
</div>
</template>

<script>
export default {
  computed: {
    gameStatus() {
      let result = ''
      
      if (this.$store.state.gameEntity.gameOver) {
        switch(this.$store.state.gameEntity.winner) {
          case 'crosses':
            result = 'Crosses won'
            break
          case 'noughts':
            result = 'Noughts won'
            break
          case 'draw':
            result = 'Draw'
            break      
        }
      } else {
        result = 'Game is in progress'
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
    justify-content: space-between;

    width: 100%;
    height: 30%;
    position: absolute;
    top: 70%;
  }

  #game-sizes {
    display: flex;
    flex-direction: row;
  }

  #game-sizes > div {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
  }

  input {
    box-sizing: border-box;
    width: 50%;
    margin: 0 10% 0 0;
  }

  @media (orientation: landscape) {
    #game-info {
      width: 30%;
      height: 100%;
      top: 0%;
      left: 70%;
    }

    #game-sizes {
      flex-direction: column;
    }

    label {
      padding: 0 0 0 10%;
    }

    input {
      margin: 0 10% 0 0;
    }
  }
</style>
