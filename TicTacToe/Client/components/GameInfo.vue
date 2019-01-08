<template>
<div id="game-info">
  <h5 id="game-status">{{this.gameStatus}}</h5>
  <div id="game-dims">
    <div>
      <label>x-dim:</label>
      <input v-model.number="dimensions.xDim" type="number" :min="limits.minXDim" :max="limits.maxXDim" required>
    </div>
    <div>
      <label>y-dim:</label>
      <input v-model.number="dimensions.yDim" type="number" :min="limits.minYDim" :max="limits.maxYDim" required/>
    </div>
    <div>
      <label>win-size:</label>
      <input v-model.number="dimensions.winSize" type="number" :min="limits.minWinSize" :max="limits.maxWinSize" required/>
    </div>
    <button id="apply-btn" class="game-btn" @click="$emit('sizes-click', dimensions)">Apply</button>
  </div>
  <button id="reset-btn" class="game-btn" @click="$emit('reset-click')">Reset</button>
</div>
</template>

<script>
import {dimLimits} from '../store/modules/gameEntity.js'

export default {
  data() {
    return {
      dimensions: {
        xDim: this.$store.state.gameEntity.xDim,
        yDim: this.$store.state.gameEntity.yDim,
        winSize: this.$store.state.gameEntity.winSize
      },

      limits: {
        minXDim: dimLimits.minXDim,
        minYDim: dimLimits.minYDim,
        minWinSize: dimLimits.minWinSize,

        maxXDim: dimLimits.maxXDim,
        maxYDim: dimLimits.maxYDim,
        maxWinSize: dimLimits.maxWinSize
      }
    }
  },

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
  .game-btn {
    background: darkolivegreen;
    outline: none;
  }

  .game-btn:active {
    background: green;
  }

  #game-info {
    font-family: Verdana;
    color: forestgreen;
    background: burlywood;

    display: flex;
    flex-direction: column;
    justify-content: space-between;

    width: 100%;
    height: 30%;
    position: absolute;
    top: 70%;
  }

  #game-dims {
    display: flex;
    flex-direction: row;
  }

  #game-dims > div {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    margin: 0 0 5% 0;
  }

  input {
    box-sizing: border-box;
    width: 75%;
    margin: 0 5% 0 0;
    border-color: green;
  }

  input:invalid {
    border-color: red;
  }

  button {
    align-self: center;
  }

  #apply-btn {
    margin: 0 0 5% 0;
  }

  @media (orientation: landscape) {
    #game-info {
      width: 30%;
      height: 100%;
      top: 0%;
      left: 70%;
    }

    #game-dims {
      flex-direction: column;
    }

    #game-status {
      padding: 0 0 0 10%;
    }

    label {
      padding: 0 0 0 10%;
    }

    input {
      width: 25%;
      margin: 0 10% 0 0;
    }

    #apply-btn {
      margin: 10% 0 0 0;
    }

    #reset-btn {
      margin: 0 0 15% 0;
    }
  }
</style>
