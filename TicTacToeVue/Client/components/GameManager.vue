<template>
  <div id="game-manager">
    <game-field></game-field>
    <game-info></game-info>
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
    this.$store.commit('gameEngine/fillDefault')
  }
}
</script>

<style>
  #game-manager {
    background: teal;
    width: 100%;
    padding-bottom: 66%;
    position: relative;
  }
</style>