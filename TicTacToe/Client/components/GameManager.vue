<template>
  <div id="game-manager">
    <game-field @mouse-down="cellMouseDown" @mouse-up="cellMouseUp" @mouse-leave="cellMouseLeave"></game-field>
    <game-info @reset-click="resetGame" @sizes-click="changeSizes"></game-info>
  </div>
</template>

<script>
import GameField from './GameField.vue'
import GameInfo from './GameInfo.vue'

import {validateDimensions} from '../store/modules/gameEntity.js'

export default {
  components: {
    'game-field': GameField,
    'game-info': GameInfo
  },

  data() {
    return {
      lastPressed: -1
    }
  },

  mounted() {
    this.$store.commit('gameEntity/fillDefault')

    this.$store.watch(
      function(state) {
        return {x: state.gameEntity.xDim, y: state.gameEntity.yDim, gameOver: state.gameEntity.gameOver}
      },
      () => {
        this.$children[0].drawAllCells()
      }
    )
  },

  methods: {
    cellMouseDown: function(index) {
      this.lastPressed = -1
      if (index > -1) {
        this.lastPressed = index
        this.$store.commit('gameEntity/setPressed', index)
        this.$children[0].drawCell(index)
      }
    },

    cellMouseUp: function(index) {
      if (this.lastPressed > -1)
      {
        this.$store.commit('gameEntity/setUnpressed', this.lastPressed)
        this.$children[0].drawCell(this.lastPressed)
      }
      
      if (index > -1 && index === this.lastPressed) {
        this.$store.commit('gameEntity/tryMove', index)
        this.$children[0].drawCell(index)
      }
    },

    cellMouseLeave: function() {
      if (this.lastPressed > -1)
      {
        this.$store.commit('gameEntity/setUnpressed', this.lastPressed)
        this.$children[0].drawCell(this.lastPressed)
      }
      
      this.lastPressed = -1
    },

    resetGame: function() {
      this.$store.commit('gameEntity/fillDefault')
      this.$children[0].drawAllCells()
    },

    changeSizes: function(dimensions) {
      let state = this.$store.state

      let {xDim, yDim, winSize} = dimensions

      if (!validateDimensions(state, dimensions)) return

      this.$store.commit('gameEntity/changeSizes', dimensions)
      this.$children[0].drawAllCells()
    }
  }
}
</script>

<style>
  #game-manager {
    background: teal;
    width: 91%;
    padding-bottom: 130%;
    position: relative;
  }

  @media (orientation: landscape) {
    #game-manager {
      width: 100%;
      padding-bottom: 70%;
    }
  }
</style>