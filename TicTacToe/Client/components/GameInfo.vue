<template>
<div id="game-info">
  <h5 class="margin-bottom" id="game-status"><span v-html="this.gameStatus"></span></h5>
  <p>Preferred Dimensions:</p>
  <div id="game-dims">
    <div>
      <label>x-dim:</label>
      <input v-model.trim.number="xDim" type="number"
            :min="limits.minXDim" :max="limits.maxXDim" required>
    </div>
    <div>
      <label>y-dim:</label>
      <input v-model.trim.number="yDim" type="number"
            :min="limits.minYDim" :max="limits.maxYDim" required/>
    </div>
    <div>
      <label>win-size:</label>
      <input v-model.trim.number="winSize" type="number"
            :min="limits.minWinSize" :max="limits.maxWinSize" required/>
    </div>
    <button :disabled="!lookingForGame" id="apply-btn" class="game-btn" @click="applyButtonHandler">Apply</button>
  </div>
  <div id="curr-dims">
    <p class="margin-bottom">My Current Preferred Dimensions: </p>
    <p>{{myPreferredDimensions.xDim}} y:{{myPreferredDimensions.yDim}} w:{{myPreferredDimensions.winSize}}</p>
  </div>
  <button :disabled="lookingForGame" id="exit-btn" class="game-btn" @click="$emit('exit-click')">Exit Game</button>
</div>
</template>

<script>
import {dimLimits} from '../store/modules/gameEntity.js'

function dimensionsAreValid({xDim, yDim, winSize}) {
  if (typeof(xDim) !== 'number' || typeof(yDim) !== 'number' || typeof(winSize) !== 'number'||
      xDim < dimLimits.minXDim || yDim < dimLimits.minYDim || winSize < dimLimits.minWinSize ||
      xDim > dimLimits.maxXDim || yDim > dimLimits.maxYDim || winSize > dimLimits.maxWinSize ||
      winSize > Math.min(xDim,yDim)
      )
    return false
  else
    return true
}

function dimensionsChanged(state, {xDim, yDim, winSize}) {
  if (xDim === state.xDim && yDim === state.yDim && winSize === state.winSize)
    return false
  else
    return true
}

export default {
  props: {
    lookingForGame: {
      type: Boolean,
      default: true
    },

    isMyTurn: {
      type: Boolean,
      default: true
    },

    mySide: {
      type: String,
      default: ''
    },

    myPreferredDimensions: {
      type: Object,
      default: null
    }
  },

  data() {
    return {
      xDim: this.$store.state.gameEntity.xDim,
      yDim: this.$store.state.gameEntity.yDim,
      winSize: this.$store.state.gameEntity.winSize,

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
      
      if (this.lookingForGame) {
        result = "Search games."
      }
      else
      {
        if (this.$store.state.gameEntity.gameOver) {
          switch(this.$store.state.gameEntity.winner) {
            case 'crosses':
              if (this.mySide === 'crosses')
                result = 'You won!'
              else
                result = 'You lost!'
              break
            case 'noughts':
              if (this.mySide === 'noughts')
                result = 'You won!'
              else
                result = 'You lost!'
              break
            case 'draw':
              result = 'Draw!'
              break      
          }

          if (this.$store.state.gameEntity.winByForfeit)
            result = 'Oponent left.<br/>' + result

          result = '<i>My side: ' + this.mySide + '</i>.<br/>' + result
        }
        else
        {
          if (this.isMyTurn)
            result = '<i>My side: ' + this.mySide + '</i>.<br/>Your turn.'
          else
            result = '<i>My side: ' + this.mySide + "</i>.<br/>Oponent's turn."
        }
      }

      return result
    }
  },

  watch: {
    xDim(newVal, oldVal) {
      if (typeof(newVal) !== 'number') {
        this.xDim = oldVal-1 // force input update
        this.xDim = oldVal
      }
    },

    yDim(newVal, oldVal) {
      if (typeof(newVal) !== 'number') {
        this.yDim = oldVal-1 // force input update
        this.yDim = oldVal
      }
    },

    winSize(newVal, oldVal) {
      if (typeof(newVal) !== 'number') {
        this.winSize = oldVal-1 // force input update
        this.winSize = oldVal
      }
    }
  },

  methods: {
    applyButtonHandler() {
      let state = this.$store.state.gameEntity
      let dimensions = {xDim: this.xDim, yDim: this.yDim, winSize: this.winSize}

      if (!dimensionsAreValid(dimensions) || !dimensionsChanged(state, dimensions))
        return

      this.$emit('sizes-click', dimensions)
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

  #apply-btn {
    margin: 0 0 5% 0;
  }

  #curr-dims {
    display: flex;
    flex-direction: row;
  }

  .margin-bottom {
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
      margin: 10% 0 10% 0;
    }

    #exit-btn {
      margin: 0 0 15% 0;
    }

    #curr-dims {
      flex-direction: column;
    }

    .margin-bottom {
      margin: 0 0 15% 0;
    }
  }
</style>
