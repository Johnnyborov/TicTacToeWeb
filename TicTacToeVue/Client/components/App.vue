<template>
<div id="app">
  <h2 id="h2">{{this.message}}</h2>
  <div id="game-row">
    <div id="game-column">
      <game-manager></game-manager>
    </div>
  </div>
</div>
</template>

<script>
import GameManager from './GameManager.vue'

export default {
  data() {
    return {
      message: 'Basic Store Project Template',
    }
  },

  components: {
    'game-manager': GameManager
  }
}
</script>

<style>
  #app {
    background: slategray;
    font-family: Verdana;
    color: navy;
    display: flex;
    flex-direction: column;
  }

  #game-row {
    background:mediumslateblue;
    display: flex;
    flex-direction: row;
  }

  #game-column {
    background: maroon;
    flex: 0 0 90vmin;
  }
</style>
