<template>
  <div id="game-manager">
    <game-field @mouse-down="mouseDown" @mouse-up="mouseUp" @mouse-leave="mouseLeave"></game-field>
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

  data() {
    return {
      lastPressed: -1
    }
  },

  mounted() {
    this.$store.commit('gameEngine/fillDefault')
  },

  methods: {
    mouseDown: function(index) {
      this.lastPressed = -1
      if (index > -1) {
        this.lastPressed = index
        this.$store.commit('gameEngine/makePressed', index)
      }
    },

    mouseUp: function(index) {
      if (this.lastPressed > -1)
        this.$store.commit('gameEngine/makeUnpressed', this.lastPressed)
      
      if (index > -1 && index === this.lastPressed) {
        this.$store.dispatch('gameEngine/tryMove', this.lastPressed)
      }
    },

    mouseLeave: function() {
      if (this.lastPressed > -1)
        this.$store.commit('gameEngine/makeUnpressed', this.lastPressed)
      
      this.lastPressed = -1
    }
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