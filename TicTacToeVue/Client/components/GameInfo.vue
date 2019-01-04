<template>
<div id="game-info">
  <h2>{{this.message}}</h2>
  <h2>{{this.$store.state.gameEngine.msg}}</h2>
</div>
</template>

<script>
export default {
  data() {
    return {
      message: 'Game Info',
    }
  }
}
</script>

<style>
  #game-info {
    font-family: Verdana;
    color: forestgreen;
    background: gold;
    width: 34%;
    height: 100%;
    position: absolute;
    left: 66%;
  }
</style>
