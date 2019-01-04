<template>
<div id="game-field">
  <canvas id="canvas" ref="canvas"></canvas>
  <button @click="foo">Draw</button>
</div>
</template>

<script>
import Resources from '../resources/resources.js'
export default {
  data() {
    return {
      message: 'Basic Store Project Template',
      context: null,
      cross: null,
      nought: null,
      clear: null
    }
  },

  mounted() {
    this.context = this.$refs['canvas'].getContext('2d')

    this.$refs['canvas'].width = this.$refs['canvas'].parentElement.clientWidth
    this.$refs['canvas'].height = this.$refs['canvas'].parentElement.clientHeight

    var cross = new Image()
    cross.src = Resources.cross
    this.cross = cross

    var nought = new Image()
    nought.src = Resources.nought
    this.nought = nought

    var clear = new Image()
    clear.src = Resources.clear
    this.clear = clear
  },

  methods: {
    foo: function() {
      var ctx = this.context
      
      ctx.lineWidth = 10

      for (let i = 0; i < 3; i++) {
        for (let j = 0; j < 3; j++) {
          ctx.strokeRect(5+130*i, 5+130*j, 110, 110)          
          ctx.drawImage(this.cross, 10+130*i, 10+130*j) 
        }    
      }
    }
  }
}
</script>

<style>
  #game-field {
    background: sienna;
    width: 66%;
    height: 100%;
    position: absolute;
  }
</style>
