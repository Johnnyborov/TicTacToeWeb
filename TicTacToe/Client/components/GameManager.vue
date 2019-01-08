<template>
  <div id="game-manager">
    <game-field @cell-clicked="cellClicked"></game-field>
    <game-info @reset-click="resetGame" @sizes-click="changeSizes"></game-info>
  </div>
</template>

<script>
import GameField from './GameField.vue'
import GameInfo from './GameInfo.vue'

export default {
  components: {
    'game-field': GameField,
    'game-info': GameInfo
  },

  mounted() {
    this.$store.commit('gameEntity/newGame')
  },

  methods: {
    cellClicked(index) {
      this.$store.commit('gameEntity/makeMove', index)
    },

    resetGame() {
      this.$store.commit('gameEntity/newGame')
    },

    changeSizes(dimensions) {
      this.$store.commit('gameEntity/changeSizes', dimensions)
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