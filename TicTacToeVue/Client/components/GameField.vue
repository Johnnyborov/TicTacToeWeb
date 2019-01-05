<template>
<div id="game-field">
  <canvas id="canvas" ref="canvas" @mousedown="this.mouseDown" @mouseup="this.mouseUp"></canvas>
</div>
</template>

<script>
import Resources from '../resources/resources.js'

export default {
  data() {
    return {
      context: null,
      cross: null,
      nought: null,
      clear: null
    }
  },

  created() {
    window.addEventListener("resize", this.resizeHandler)
    window.addEventListener('load', this.loadedHandler)
  },
  
  destroyed() {
    window.removeEventListener("resize", this.resizeHandler)
    window.removeEventListener('load', this.loadedHandler)
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
    mouseDown: function(e) {
      let rect = this.context.canvas.getBoundingClientRect()
      let x = e.clientX - rect.left
      let y = e.clientY - rect.top

      console.log('down',x,y)
    },

    mouseUp: function(e) {
      let rect = this.context.canvas.getBoundingClientRect()
      let x = e.clientX - rect.left
      let y = e.clientY - rect.top

      console.log('up',x,y)
    },

    resizeHandler: function() {
      this.$refs['canvas'].width = this.$refs['canvas'].parentElement.clientWidth
      this.$refs['canvas'].height = this.$refs['canvas'].parentElement.clientHeight

      this.drawCells()
    },

    loadedHandler: function() {
      this.drawCells()
    },

    drawCells: function() {
      let ctx = this.context 

      let halfCellBorderWidth = Math.round(ctx.canvas.width / 40 / 2)
      let cellBorderWidth = 2 * halfCellBorderWidth
      let imgSize = Math.round((ctx.canvas.width - cellBorderWidth*(3 * 2 + (3-1))) / 3)
      let imgStep = (imgSize + 3 * cellBorderWidth)

      ctx.lineWidth = cellBorderWidth

      for (let i = 0; i < 3; i++) {
        for (let j = 0; j < 3; j++) {
          ctx.strokeRect(halfCellBorderWidth+imgStep*j, halfCellBorderWidth+imgStep*i, imgSize+2*halfCellBorderWidth, imgSize+2*halfCellBorderWidth)         

          let cell;
          switch(this.$store.state.gameEngine.cells[i*3+j]) {
            case 0:
              cell = this.clear
              break
            case 1:
              cell = this.cross
              break
            case 2:
              cell = this.nought
              break
            default:
              throw "Invalid cell value"
          }

          ctx.drawImage(cell, cellBorderWidth+imgStep*j, cellBorderWidth+imgStep*i, imgSize, imgSize) 
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
