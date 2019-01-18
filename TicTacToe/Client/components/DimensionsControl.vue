<template>
<div>
  <h5 class="margin-bottom" id="header">{{header}}</h5>

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
    <button id="apply-btn" class="game-btn" @click="applyButtonHandler">Apply</button>
  </div>
  <div id="curr-dims">
    <p class="margin-bottom">My Current Preferred Dimensions: </p>
    <p>{{$store.state.invites.myPreferredDimensions.xDim}} y:{{$store.state.invites.myPreferredDimensions.yDim}} w:{{$store.state.invites.myPreferredDimensions.winSize}}</p>
  </div>

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

function preferredDimensionsChanged(state, {xDim, yDim, winSize}) {
  if (xDim === state.myPreferredDimensions.xDim && yDim === state.myPreferredDimensions.yDim &&
      winSize === state.myPreferredDimensions.winSize)
    return false
  else
    return true
}

export default {
  data() {
    return {
      xDim: this.$store.state.invites.myPreferredDimensions.xDim,
      yDim: this.$store.state.invites.myPreferredDimensions.yDim,
      winSize: this.$store.state.invites.myPreferredDimensions.winSize,

      limits: {
        minXDim: dimLimits.minXDim,
        minYDim: dimLimits.minYDim,
        minWinSize: dimLimits.minWinSize,

        maxXDim: dimLimits.maxXDim,
        maxYDim: dimLimits.maxYDim,
        maxWinSize: dimLimits.maxWinSize
      },

      header: 'Search games.'
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
      let dimensions = {xDim: this.xDim, yDim: this.yDim, winSize: this.winSize}

      if (!dimensionsAreValid(dimensions) || !preferredDimensionsChanged(this.$store.state.invites, dimensions))
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
    #game-dims {
      flex-direction: column;
    }

    #header {
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

    #curr-dims {
      flex-direction: column;
    }

    .margin-bottom {
      margin: 0 0 15% 0;
    }
  }
</style>
