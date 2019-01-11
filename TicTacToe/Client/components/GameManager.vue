<template>
  <div id="game-manager">
    <game-hub v-if="lookingForGame" @game-created="createGameHandler"></game-hub>
    <template v-else>
      <game-field @cell-clicked="cellClickedHandler" :cellIcons="resources.cellIcons"></game-field>
      <game-info @reset-click="resetGameHandler" @sizes-click="changeSizesHandler"></game-info>
    </template>
  </div>
</template>

<script>
import GameHub from './GameHub.vue'
import GameField from './GameField.vue'
import GameInfo from './GameInfo.vue'

import Resources from '../resources/resources.js'

export default {
  components: {
    'game-hub': GameHub,
    'game-field': GameField,
    'game-info': GameInfo
  },

  data() {
    return {
      lookingForGame: true,

      resources: {
        cellIcons: null
      }
    }
  },

  mounted() {
    this.resources.cellIcons = Resources.getCellIcons()

    this.$store.commit('gameEntity/newGame')
  },

  methods: {
    cellClickedHandler(index) {
      this.$store.commit('gameEntity/makeMove', index)
    },

    resetGameHandler() {
      this.$store.commit('gameEntity/newGame')
    },

    changeSizesHandler(dimensions) {
      this.$store.commit('gameEntity/changeSizes', dimensions)
    },

    createGameHandler() {
      this.lookingForGame = false

      this.$store.commit('gameEntity/newGame')
      //setTimeout(()=>{this.lookingForGame = true}, 5000)
    }
  }
}
</script>

<style>
  #game-manager {
    background: burlywood;
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